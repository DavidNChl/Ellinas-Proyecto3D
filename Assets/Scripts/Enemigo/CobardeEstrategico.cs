using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class StrategicCowardAI : MonoBehaviour
{
    enum State { Patrol, Attack, Flee }
    State state = State.Patrol;

    [Header("Referencias")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform[] safePoints; // Puntos seguros para huir
    [SerializeField] Transform player;

    [Header("Salud")]
    [SerializeField] private SaludEnemigo salud;

    [Header("Parámetros de Movimiento y Detección")]
    [SerializeField] float patrolSpeed = 3f;
    [SerializeField] float combatSpeed = 6f;
    [SerializeField] float fleeSpeed = 7.5f;
    [SerializeField] float detectionRange = 10f;

    NavMeshAgent agent;
    Renderer rend;
    int currentWaypoint;
    Transform currentSafePoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        float d = Vector3.Distance(transform.position, player.position);
        bool isLowHealth = salud != null && salud.PorcentajeSalud <= 0.5f;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                // Si detecta al jugador, decide si atacar o huir según su salud
                if (d < detectionRange)
                {
                    if (isLowHealth)
                        StartFleeing();
                    else
                        SetState(State.Attack);
                }
                break;

            case State.Attack:
                // Transición crítica: si la salud baja del 50% en combate, huye
                if (isLowHealth)
                {
                    StartFleeing();
                    break;
                }

                agent.speed = combatSpeed;
                agent.SetDestination(player.position);

                // Si el jugador escapa lejos, vuelve a patrullar
                if (d > detectionRange * 1.5f)
                {
                    SetState(State.Patrol);
                }
                break;

            case State.Flee:
                agent.speed = fleeSpeed;
                if (currentSafePoint != null)
                {
                    agent.SetDestination(currentSafePoint.position);

                    // Al llegar al punto seguro y estar lejos del jugador, vuelve a vigilar
                    if (!agent.pathPending && agent.remainingDistance < 0.8f && d > detectionRange)
                    {
                        SetState(State.Patrol);
                    }
                }
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void StartFleeing()
    {
        currentSafePoint = GetFarthestSafePoint();
        SetState(State.Flee);
    }

    // Busca el punto seguro más alejado del jugador para evitar huir hacia él
    Transform GetFarthestSafePoint()
    {
        if (safePoints.Length == 0) return null;

        Transform bestPoint = safePoints[0];
        float maxDist = -1f;

        foreach (Transform point in safePoints)
        {
            float distToPlayer = Vector3.Distance(point.position, player.position);
            if (distToPlayer > maxDist)
            {
                maxDist = distToPlayer;
                bestPoint = point;
            }
        }
        return bestPoint;
    }

    void SetState(State next)
    {
        state = next;
        // Verde = Patrulla, Rojo = Ataque, Cian = Huyendo
        rend.material.color =
            next == State.Attack ? Color.red :
            next == State.Flee ? Color.cyan :
            Color.green;
    }
}