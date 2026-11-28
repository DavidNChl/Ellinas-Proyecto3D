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
    [SerializeField] float aiUpdateInterval = 0.2f; // 5 evaluaciones/seg

    NavMeshAgent agent;
    Renderer rend;
    MaterialPropertyBlock propBlock;
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    int currentWaypoint;
    Vector3 lastKnownPosition;
    float searchTimer;
    float aiTimer;
    float detectionRangeSqr;
    float loseRangeSqr;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();

        detectionRangeSqr = detectionRange * detectionRange;
        float loseRange = detectionRange * 1.5f;
        loseRangeSqr = loseRange * loseRange;
        aiTimer = Random.Range(0f, aiUpdateInterval);
    }

    void Update()
    {
        if (player == null) return;

        if (state == State.Search && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchWait)
            {
                SetState(State.Patrol);
                return;
            }
        }

        aiTimer -= Time.deltaTime;
        if (aiTimer > 0f) return;
        aiTimer = aiUpdateInterval;

        // sqrMagnitude evita calcular raíces cuadradas cada frame
        float dSqr = (transform.position - player.position).sqrMagnitude;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (dSqr < detectionRangeSqr)
                    SetState(State.Chase);
                break;

            case State.Chase:
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                if (dSqr > loseRangeSqr)
                {
                    lastKnownPosition = player.position;
                    SetState(State.Search);
                }
                break;

            case State.Search:
                agent.speed = patrolSpeed;
                agent.SetDestination(lastKnownPosition);
                if (dSqr < detectionRangeSqr)
                    SetState(State.Chase);
                break;
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void SetState(State next)
    {
        state = next;
        searchTimer = 0f;

        if (rend != null)
        {
            // MaterialPropertyBlock cambia color sin clonar materiales ni romper batching
            Color c = next == State.Chase ? Color.red :
                      next == State.Search ? Color.yellow :
                      Color.green;
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor(BaseColorId, c);
            rend.SetPropertyBlock(propBlock);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange * 1.5f);
    }
}