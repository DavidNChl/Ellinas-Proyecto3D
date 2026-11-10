using UnityEngine;

public class ManejadorHorda : MonoBehaviour
{
    public static ManejadorHorda Instancia { get; private set; }

    [Header("Configuración de Rabia")]
    public int bajasParaModoRabia = 3;
    public int bajasActuales = 0;
    public bool modoRabiaActivado = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void RegistrarMuerteEnemigo()
    {
        bajasActuales++;
        if (bajasActuales >= bajasParaModoRabia && !modoRabiaActivado)
        {
            modoRabiaActivado = true;
            Debug.Log("¡Los enemigos se han enfurecido! Ahora atacarán sin importar si los miras.");
        }
    }
}