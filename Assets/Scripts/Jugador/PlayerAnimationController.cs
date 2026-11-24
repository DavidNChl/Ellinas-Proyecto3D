using UnityEngine;

[RequireComponent(typeof(ControladorJugador))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ControladorJugador controlador;
    [SerializeField] private Animator animator;

    [Header("Suavizado")]
    [Tooltip("Suaviza la transición entre Idle, Walk y Run para que no sea brusca.")]
    [SerializeField] private float tiempoSuavizado = 0.1f;

    // Cacheamos el Hash para evitar GC Alloc en cada frame
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        if (controlador == null)
            controlador = GetComponent<ControladorJugador>();

        // Busca el Animator en este objeto o en el modelo 3D hijo
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró un componente Animator en el jugador.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Usamos el sobrecargado de SetFloat con dampTime para transiciones fluidas
        animator.SetFloat(SpeedHash, controlador.VelocidadActual, tiempoSuavizado, Time.deltaTime);
    }
}