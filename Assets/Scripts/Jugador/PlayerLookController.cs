using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(CameraPerspectiveController))]
public class PlayerLookController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private CameraPerspectiveController perspectives;

    [Tooltip("La Main Camera real, para los sistemas existentes.")]
    [SerializeField] private Transform camaraTransform;

    [SerializeField] private Transform eyeAnchor;
    [SerializeField] private Transform firstPersonTarget;
    [SerializeField] private Transform thirdPersonTarget;

    [Header("Sensibilidad")]
    [SerializeField] private float firstPersonSensitivity = 0.1f;
    [SerializeField] private float thirdPersonSensitivity = 0.3f;

    [Header("Límites verticales")]
    [SerializeField] private float firstPersonVerticalLimit = 80f;
    [SerializeField] private float thirdPersonLookUpLimit = 50f;
    [SerializeField] private float thirdPersonLookDownLimit = 70f;

    private float yaw;
    private float pitch;

    public Transform ViewTransform => camaraTransform;

    public Quaternion MovementRotation =>
        Quaternion.Euler(0f, yaw, 0f);

    public bool IsThirdPerson => perspectives.IsThirdPerson;

    private void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInputReader>();

        if (perspectives == null)
            perspectives = GetComponent<CameraPerspectiveController>();

        yaw = transform.eulerAngles.y;

        if (camaraTransform == null ||
            eyeAnchor == null ||
            firstPersonTarget == null ||
            thirdPersonTarget == null)
        {
            Debug.LogError(
                "Completa las referencias de PlayerLookController.",
                this
            );

            enabled = false;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float sensitivity = IsThirdPerson
            ? thirdPersonSensitivity
            : firstPersonSensitivity;

        yaw = Mathf.Repeat(
            yaw + input.Look.x * sensitivity,
            360f
        );

        pitch -= input.Look.y * sensitivity;

        float minimumPitch = IsThirdPerson
            ? -thirdPersonLookUpLimit
            : -firstPersonVerticalLimit;

        float maximumPitch = IsThirdPerson
            ? thirdPersonLookDownLimit
            : firstPersonVerticalLimit;

        pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Los objetivos mantienen orientación independiente del cuerpo.
        firstPersonTarget.SetPositionAndRotation(
            eyeAnchor.position,
            rotation
        );

        thirdPersonTarget.SetPositionAndRotation(
            eyeAnchor.position,
            rotation
        );
    }
}