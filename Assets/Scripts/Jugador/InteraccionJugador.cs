using UnityEngine;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera cam;
    [SerializeField] private InventarioJugador inventario;

    [Header("Configuración")]
    [SerializeField] private float distanciaRecoger = 3.0f;
    [SerializeField] private LayerMask capasInteractuables;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (inventario == null) inventario = GetComponentInParent<InventarioJugador>();
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;
        if (inventario == null) return;

        ObjetoRecogible itemObjetivo = BuscarObjetoCercano();
        if (itemObjetivo != null)
        {
            bool recogido = inventario.AgregarObjeto(
                itemObjetivo.nombreObjeto,
                itemObjetivo.cantidad,
                itemObjetivo.capacidadMaximaEnSlot
            );

            if (recogido)
            {
                Destroy(itemObjetivo.gameObject);
            }
        }
    }

    private ObjetoRecogible BuscarObjetoCercano()
    {
        // 1. Primero intenta con el rayo desde la mira (ideal para 1ª persona o apuntado directo)
        if (cam != null)
        {
            float distanciaCamara = Vector3.Distance(cam.transform.position, transform.position);
            Ray rayo = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit[] impactos = Physics.RaycastAll(rayo, distanciaRecoger + distanciaCamara, capasInteractuables, QueryTriggerInteraction.Collide);
            System.Array.Sort(impactos, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in impactos)
            {
                if (hit.transform.IsChildOf(transform.root)) continue;

                ObjetoRecogible item = hit.collider.GetComponentInParent<ObjetoRecogible>();
                if (item != null) return item;
            }
        }

        // 2. Respaldo por proximidad alrededor del jugador (ideal para 3ª persona)
        Vector3 origenJugador = inventario.transform.position + Vector3.up * 0.8f;
        Collider[] cercanos = Physics.OverlapSphere(origenJugador, distanciaRecoger, capasInteractuables, QueryTriggerInteraction.Collide);

        ObjetoRecogible masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider col in cercanos)
        {
            if (col.transform.IsChildOf(transform.root)) continue;

            ObjetoRecogible item = col.GetComponentInParent<ObjetoRecogible>();
            if (item != null)
            {
                float dist = Vector3.Distance(origenJugador, col.transform.position);
                if (dist < menorDistancia)
                {
                    menorDistancia = dist;
                    masCercano = item;
                }
            }
        }

        return masCercano;
    }
}