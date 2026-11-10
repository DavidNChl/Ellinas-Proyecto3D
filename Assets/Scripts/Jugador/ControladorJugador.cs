using UnityEngine;
using UnityEngine.InputSystem; // Nuevo Input System

[RequireComponent(typeof(CharacterController))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6.0f;
    public float fuerzaSalto = 1.2f;
    public float gravedad = -9.81f;

    [Header("Cámara")]
    public Transform camaraTransform;
    public float sensibilidadMouse = 0.1f;
    public float limiteCamaraVertical = 80.0f;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private bool estaEnSuelo;
    private float rotacionX = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ManejarRotacion();
        ManejarMovimiento();
    }

    void ManejarRotacion()
    {
        if (camaraTransform == null || Mouse.current == null) return;

        // Lectura del nuevo Input System para el ratón
        Vector2 deltaMouse = Mouse.current.delta.ReadValue() * sensibilidadMouse;

        rotacionX -= deltaMouse.y;
        rotacionX = Mathf.Clamp(rotacionX, -limiteCamaraVertical, limiteCamaraVertical);
        camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        transform.Rotate(Vector3.up * deltaMouse.x);
    }

    void ManejarMovimiento()
    {
        estaEnSuelo = controller.isGrounded;
        if (estaEnSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        // Lectura del nuevo Input System para teclado (WASD / Flechas)
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
        }

        Vector3 mover = transform.right * x + transform.forward * z;
        controller.Move(mover.normalized * velocidad * Time.deltaTime);

        // Salto con barra espaciadora
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && estaEnSuelo)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
        }

        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }
}