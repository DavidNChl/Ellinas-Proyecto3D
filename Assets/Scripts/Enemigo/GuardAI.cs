using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour
{
    enum State { Patrol, Chase, Search }
    State state = State.Patrol;

    [Header("Referencias")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform player;

    [Header("Parámetros")]
    [SerializeField] float patrolSpeed = 3f;
    [SerializeField] float chaseSpeed = 6f;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float searchWait = 3f;

    NavMeshAgent agent;
    Renderer rend;
    int currentWaypoint;
    Vector3 lastKnownPosition;
    float searchTimer;

    void Awake()
    {
        // Cachear componentes una sola vez
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        float d = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (d < detectionRange)
                    SetState(State.Chase);
                break;

            case State.Chase:
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                // Margen de histéresis (1.5x) para evitar parpadeo en el borde
                if (d > detectionRange * 1.5f)
                {
                    lastKnownPosition = player.position;
                    SetState(State.Search);
                }
                break;

            case State.Search:
                agent.speed = patrolSpeed;
                agent.SetDestination(lastKnownPosition);
                if (d < detectionRange)
                {
                    SetState(State.Chase);
                    break;
                }
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    searchTimer += Time.deltaTime;
                    if (searchTimer >= searchWait)
                        SetState(State.Patrol);
                }
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);

        // Verificar pathPending evita que salte waypoints en un solo frame
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void SetState(State next)
    {
        state = next;
        searchTimer = 0f;

        // Feedback visual según el estado
        rend.material.color =
            next == State.Chase ? Color.red :
            next == State.Search ? Color.yellow :
            Color.green;
    }

    // Dibujar radios de detección en la vista de Escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange * 1.5f);
    }
}