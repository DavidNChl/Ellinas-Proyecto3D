using UnityEngine;

public class SaludEnemigo : MonoBehaviour
{
    [Header("Salud del Enemigo")]
    public float saludMaxima = 50f;
    private float saludActual;

    void Start()
    {
        saludActual = saludMaxima;
    }

    public void RecibirDano(float cantidad)
    {
        saludActual -= cantidad;
        Debug.Log(gameObject.name + " recibió daño. Salud restante: " + saludActual);

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Notifica al gestor de la horda para sumar la baja
        if (ManejadorHorda.Instancia != null)
        {
            ManejadorHorda.Instancia.RegistrarMuerteEnemigo();
        }

        Destroy(gameObject);
    }
}