using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class SlotInventario
{
    public string nombreObjeto = "Vacío";
    public int cantidad = 0;
    public int capacidadMaxima = 5;
    public bool estaVacio => cantidad <= 0;
}

public class InventarioJugador : MonoBehaviour
{
    [Header("Configuración de Inventario Limitado")]
    [SerializeField] private SlotInventario[] slots = new SlotInventario[3];
    [SerializeField] private SaludJugador saludJugador;

    [Header("Equipamiento Visual")]
    [Tooltip("Objeto 3D del arma sostenida por el jugador (hijo de la cámara o mano).")]
    [SerializeField] private GameObject modeloArmaEnMano;
    [SerializeField] private string nombreItemArma = "Pistola";

    public int SlotSeleccionado { get; private set; } = 0;
    public SlotInventario[] Slots => slots;
    public string ObjetoEquipadoActual => slots[SlotSeleccionado].estaVacio ? string.Empty : slots[SlotSeleccionado].nombreObjeto;
    public bool TieneArmaEquipada => ObjetoEquipadoActual == nombreItemArma;

    public event Action InventarioActualizado;

    private void Awake()
    {
        if (saludJugador == null)
            saludJugador = GetComponent<SaludJugador>();
    }

    private void Start()
    {
        ActualizarVisualEquipamiento();
        InventarioActualizado?.Invoke();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SeleccionarSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SeleccionarSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SeleccionarSlot(2);

        // Tecla Q para usar consumibles (ej. Botiquín) y dejar E libre para recoger objetos
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            UsarObjetoSeleccionado();
        }
    }

    public void SeleccionarSlot(int indice)
    {
        if (indice < 0 || indice >= slots.Length) return;
        SlotSeleccionado = indice;
        ActualizarVisualEquipamiento();
        InventarioActualizado?.Invoke();
    }

    public bool AgregarObjeto(string nombre, int cantidad, int maxPorSlot = 5)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].estaVacio && slots[i].nombreObjeto == nombre && slots[i].cantidad < slots[i].capacidadMaxima)
            {
                slots[i].cantidad = Mathf.Min(slots[i].cantidad + cantidad, slots[i].capacidadMaxima);
                ActualizarVisualEquipamiento();
                InventarioActualizado?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].estaVacio)
            {
                slots[i].nombreObjeto = nombre;
                slots[i].capacidadMaxima = maxPorSlot;
                slots[i].cantidad = Mathf.Min(cantidad, maxPorSlot);
                ActualizarVisualEquipamiento();
                InventarioActualizado?.Invoke();
                return true;
            }
        }

        return false;
    }

    public bool ConsumirDelSlot(int indice, int cantidad = 1)
    {
        if (indice < 0 || indice >= slots.Length) return false;
        if (slots[indice].cantidad < cantidad) return false;

        slots[indice].cantidad -= cantidad;
        if (slots[indice].cantidad <= 0)
        {
            slots[indice].cantidad = 0;
            slots[indice].nombreObjeto = "Vacío";
        }

        ActualizarVisualEquipamiento();
        InventarioActualizado?.Invoke();
        return true;
    }

    public void UsarObjetoSeleccionado()
    {
        SlotInventario actual = slots[SlotSeleccionado];
        if (actual.estaVacio) return;

        if (actual.nombreObjeto == "Botiquin" && saludJugador != null)
        {
            if (ConsumirDelSlot(SlotSeleccionado, 1))
            {
                saludJugador.RecibirDano(-30f);
            }
        }
    }

    private void ActualizarVisualEquipamiento()
    {
        if (modeloArmaEnMano != null)
            modeloArmaEnMano.SetActive(TieneArmaEquipada);
    }
}