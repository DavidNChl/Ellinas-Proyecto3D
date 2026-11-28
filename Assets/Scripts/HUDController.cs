using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private Image[] fondosSlots;
    [SerializeField] private TextMeshProUGUI[] textosSlots;

    [Header("Colores de Selección")]
    [SerializeField] private Color colorNormal = new Color(0.15f, 0.15f, 0.15f, 0.75f);
    [SerializeField] private Color colorSeleccionado = new Color(0.9f, 0.7f, 0.2f, 0.95f);

    private void OnEnable()
    {
        if (inventario != null)
            inventario.InventarioActualizado += ActualizarHUD;
    }

    private void OnDisable()
    {
        if (inventario != null)
            inventario.InventarioActualizado -= ActualizarHUD;
    }

    private void Start()
    {
        ActualizarHUD();
    }

    private void ActualizarHUD()
    {
        if (inventario == null) return;

        SlotInventario[] slots = inventario.Slots;

        for (int i = 0; i < fondosSlots.Length && i < slots.Length; i++)
        {
            if (fondosSlots[i] != null)
                fondosSlots[i].color = (i == inventario.SlotSeleccionado) ? colorSeleccionado : colorNormal;

            if (textosSlots[i] != null)
            {
                textosSlots[i].text = slots[i].estaVacio
                    ? $"[{i + 1}]\nVacío"
                    : $"[{i + 1}] {slots[i].nombreObjeto}\nx{slots[i].cantidad}";
            }
        }
    }
}