using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AranaAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Sincronización de Pasos")]
    [Tooltip("Velocidad a la que se ve natural el ciclo de caminata sin que las patas patinen.")]
    [SerializeField] private float velocidadBaseCaminata = 2.5f;
    [SerializeField] private float suavizadoTransicion = 0.1f;

    private static readonly int SpeedHash = Animator.StringToHash("speed");
    private static readonly int AnimSpeedHash = Animator.StringToHash("animSpeed");

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Falta el componente Animator en Spider_Enemy o sus hijos.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Si el agente está detenido (ej. congelado al mirarlo en EnemigoSigiloso), velocidad es 0
        float velocidadActual = agent.isStopped ? 0f : agent.velocity.magnitude;

        animator.SetFloat(SpeedHash, velocidadActual, suavizadoTransicion, Time.deltaTime);

        // Ajusta la cadencia de las patas proporcionalmente a la velocidad del agente
        float multiplicador = velocidadActual > 0.1f
            ? Mathf.Clamp(velocidadActual / velocidadBaseCaminata, 0.75f, 2.5f)
            : 1f;

        animator.SetFloat(AnimSpeedHash, multiplicador);
    }
}