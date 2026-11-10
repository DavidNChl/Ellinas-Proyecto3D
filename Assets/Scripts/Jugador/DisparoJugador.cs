using UnityEngine;
using UnityEngine.InputSystem;

public class DisparoJugador : MonoBehaviour
{
    [Header("Configuración de Arma")]
    public float danoDisparo = 25f;
    public float alcance = 100f;
    public LayerMask capasImpactables; // Capa de los enemigos y mapa

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        // Detecta el clic izquierdo del ratón con el nuevo Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Disparar();
        }
    }

    void Disparar()
    {
        Ray rayo = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Centro exacto de la pantalla
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, alcance, capasImpactables))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            // Busca el componente de salud en el objeto alcanzado
            SaludEnemigo enemigo = hit.transform.GetComponent<SaludEnemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(danoDisparo);
            }
        }
    }
}