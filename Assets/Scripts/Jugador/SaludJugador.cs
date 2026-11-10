using UnityEngine;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud")]
    public float saludMaxima = 100f;
    private float saludActual;

    void Start()
    {
        saludActual = saludMaxima;
    }

    public void RecibirDano(float cantidad)
    {
        saludActual -= cantidad;
        Debug.Log("Salud del jugador: " + saludActual);

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    /// Se activa automáticamente cuando el CharacterController del jugador toca a otro objeto
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemigo"))
        {
            RecibirDano(20f * Time.deltaTime); // Hace daño continuo mientras estén pegados
        }
    }

    void Morir()
    {
        Debug.Log("¡El jugador ha muerto!");
        // Aquí puedes reiniciar el nivel o mostrar la pantalla de Game Over
        gameObject.SetActive(false);
    }
}