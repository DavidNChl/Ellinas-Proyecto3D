using System;
using UnityEngine;
using Unity.Cinemachine;

public enum PlayerPerspective
{
    FirstPerson,
    ThirdPerson
}

[DefaultExecutionOrder(-150)]
[RequireComponent(typeof(PlayerInputReader))]
public class CameraPerspectiveController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private CinemachineCamera firstPersonCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;

    [Header("Inicio")]
    [SerializeField]
    private PlayerPerspective initialPerspective =
        PlayerPerspective.FirstPerson;

    public PlayerPerspective CurrentPerspective { get; private set; }

    public bool IsThirdPerson =>
        CurrentPerspective == PlayerPerspective.ThirdPerson;

    public event Action<PlayerPerspective> PerspectiveChanged;

    private void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInputReader>();

        CurrentPerspective = initialPerspective;

        if (firstPersonCamera == null ||
            thirdPersonCamera == null ||
            firstPersonCamera == thirdPersonCamera)
        {
            Debug.LogError(
                "Asigna dos cámaras Cinemachine distintas.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        input.PerspectiveToggleRequested += TogglePerspective;
    }

    private void OnDisable()
    {
        if (input != null)
            input.PerspectiveToggleRequested -= TogglePerspective;
    }

    private void Start()
    {
        ApplyPerspective();
    }

    private void TogglePerspective()
    {
        SetPerspective(IsThirdPerson
            ? PlayerPerspective.FirstPerson
            : PlayerPerspective.ThirdPerson);
    }

    public void SetPerspective(PlayerPerspective perspective)
    {
        if (CurrentPerspective == perspective)
            return;

        CurrentPerspective = perspective;
        ApplyPerspective();
    }

    private void ApplyPerspective()
    {
        firstPersonCamera.gameObject.SetActive(!IsThirdPerson);
        thirdPersonCamera.gameObject.SetActive(IsThirdPerson);

        PerspectiveChanged?.Invoke(CurrentPerspective);
    }
}