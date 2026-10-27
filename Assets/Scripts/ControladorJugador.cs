using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ControladorJugador3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminata = 3.5f;
    [SerializeField] private float velocidadCarrera = 7.0f;
    [SerializeField] private float fuerzaSalto = 1.2f;
    [SerializeField] private float gravedad = -9.81f;

    [Header("Cámara & Rotación")]
    [SerializeField] private Transform camaraTransform;
    [SerializeField] private float sensibilidadMouse = 0.1f;
    [SerializeField] private float limiteCamaraVertical = 80.0f;

    // Componentes y estado
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocidadVertical;
    private bool estaEnSuelo;
    private float rotacionX = 0.0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        // Busca el Animator en el modelo hijo (la malla del personaje)
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Bloquear y ocultar el cursor para vista en primera/tercera persona
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ManejarRotacion();
        ManejarMovimientoYAnimacion();
    }

    private void ManejarRotacion()
    {
        if (camaraTransform == null || Mouse.current == null) return;

        // Lectura del nuevo Input System
        Vector2 deltaMouse = Mouse.current.delta.ReadValue() * sensibilidadMouse;

        // Pitch (Mirar arriba/abajo en la cámara)
        rotacionX -= deltaMouse.y;
        rotacionX = Mathf.Clamp(rotacionX, -limiteCamaraVertical, limiteCamaraVertical);
        camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // Yaw (Girar el cuerpo del personaje a los lados)
        transform.Rotate(Vector3.up * deltaMouse.x);
    }

    private void ManejarMovimientoYAnimacion()
    {
        // 1. Verificación de Suelo
        estaEnSuelo = controller.isGrounded;
        if (estaEnSuelo && velocidadVertical.y < 0f)
        {
            velocidadVertical.y = -2f; // Mantiene al personaje pegado al terreno
        }

        // 2. Lectura de Teclado
        float x = 0f;
        float z = 0f;
        bool estaCorriendo = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;

            estaCorriendo = Keyboard.current.leftShiftKey.isPressed && (x != 0 || z != 0);
        }

        // 3. Cálculo de Dirección y Velocidad (Relativo a la vista del personaje)
        Vector3 direccionInput = new Vector3(x, 0, z).normalized;
        Vector3 mover = transform.right * direccionInput.x + transform.forward * direccionInput.z;

        float velocidadActual = estaCorriendo ? velocidadCarrera : velocidadCaminata;
        controller.Move(mover * velocidadActual * Time.deltaTime);

        // 4. Conexión con las Animaciones (Blend Tree)
        if (animator != null)
        {
            float targetSpeed = 0f;
            if (direccionInput.magnitude > 0.1f)
            {
                targetSpeed = estaCorriendo ? 1.0f : 0.5f; // 0.5 = Walk, 1.0 = Run
            }

            // Transición suave del parámetro "Speed"
            animator.SetFloat("Speed", targetSpeed, 0.1f, Time.deltaTime);
        }

        // 5. Salto (Fórmula física exacta despejada)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && estaEnSuelo)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
        }

        // 6. Gravedad acumulada
        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }
}