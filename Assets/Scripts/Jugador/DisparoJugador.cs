using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private Animator animatorArma;
    [SerializeField] private Camera cam;

    [Header("Configuración de Arma")]
    [SerializeField] private float danoDisparo = 25f;
    [SerializeField] private float alcance = 100f;
    [SerializeField] private LayerMask capasImpactables;

    private static readonly int DispararHash = Animator.StringToHash("Disparar");

    private void Awake()
    {
        if (input == null) input = GetComponentInParent<PlayerInputReader>();
        if (inventario == null) inventario = GetComponentInParent<InventarioJugador>();
        if (cam == null) cam = GetComponent<Camera>() ?? Camera.main;
    }

    private void OnEnable()
    {
        if (input != null) input.FireRequested += Disparar;
    }

    private void OnDisable()
    {
        if (input != null) input.FireRequested -= Disparar;
    }

    private void Disparar()
    {
        // Solo dispara si el slot seleccionado actualmente contiene el arma
        if (inventario == null || !inventario.TieneArmaEquipada) return;
        if (cam == null) return;

        if (animatorArma != null)
            animatorArma.SetTrigger(DispararHash);

        Ray rayo = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(rayo, out RaycastHit hit, alcance, capasImpactables, QueryTriggerInteraction.Ignore))
        {
            SaludEnemigo enemigo = hit.collider.GetComponentInParent<SaludEnemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(danoDisparo);
            }
        }
    }
}