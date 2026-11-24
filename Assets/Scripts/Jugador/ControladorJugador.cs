using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerLookController))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private PlayerLookController vista;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3.5f;
    [SerializeField] private float velocidadCarrera = 7f;
    [SerializeField] private float velocidadGiro = 900f;

    [Header("Salto")]
    [Tooltip("Altura aproximada del salto en metros.")]
    [SerializeField] private float fuerzaSalto = 1.35f;
    [SerializeField] private float gravedad = -20f;

    private CharacterController controller;
    private float velocidadVertical;

    public bool EstaEnSuelo => controller.isGrounded;
    public bool EstaCorriendo { get; private set; }
    public float IntensidadMovimiento { get; private set; }
    public float VelocidadActual =>
    IntensidadMovimiento <= 0.01f ? 0f : (EstaCorriendo ? velocidadCarrera : velocidad);
    
    // Compatibilidad temporal con EnemigoSigiloso.
    public Transform camaraTransform =>
        vista != null ? vista.ViewTransform : null;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (input == null)
            input = GetComponent<PlayerInputReader>();

        if (vista == null)
            vista = GetComponent<PlayerLookController>();
    }

    private void Update()
    {
        ActualizarMovimiento();
    }

    private void ActualizarMovimiento()
    {
        bool enSuelo = controller.isGrounded;

        if (enSuelo && velocidadVertical < 0f)
            velocidadVertical = -2f;

        Vector2 entrada = input.Movement;
        IntensidadMovimiento = entrada.magnitude;

        EstaCorriendo =
            input.SprintHeld &&
            entrada.sqrMagnitude > 0.01f;

        Transform camara = vista.ViewTransform;

        if (camara == null)
            return;

        // Dirección de la cámara proyectada sobre el suelo.
        // Mirar arriba o abajo no añade movimiento vertical.
        Vector3 adelante = Vector3.ProjectOnPlane(
            camara.forward,
            Vector3.up
        );

        if (adelante.sqrMagnitude < 0.001f)
        {
            adelante = vista.MovementRotation * Vector3.forward;
        }

        adelante.Normalize();

        Vector3 derecha = Vector3.Cross(
            Vector3.up,
            adelante
        ).normalized;

        Vector3 direccion =
            adelante * entrada.y +
            derecha * entrada.x;

        if (vista.IsThirdPerson)
        {
            if (direccion.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(direccion),
                    velocidadGiro * Time.deltaTime
                );
            }
        }
        else
        {
            transform.rotation = vista.MovementRotation;
        }

        if (input.JumpPressed && enSuelo)
        {
            velocidadVertical = Mathf.Sqrt(
                fuerzaSalto * -2f * gravedad
            );
        }

        velocidadVertical += gravedad * Time.deltaTime;

        float velocidadActual = EstaCorriendo
            ? velocidadCarrera
            : velocidad;

        Vector3 movimiento =
            direccion * velocidadActual +
            Vector3.up * velocidadVertical;

        CollisionFlags colisiones = controller.Move(
            movimiento * Time.deltaTime
        );

        if ((colisiones & CollisionFlags.Above) != 0 &&
            velocidadVertical > 0f)
        {
            velocidadVertical = 0f;
        }
    }

    private void OnDisable()
    {
        velocidadVertical = 0f;
        EstaCorriendo = false;
        IntensidadMovimiento = 0f;
    }
}