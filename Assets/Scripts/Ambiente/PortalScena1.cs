using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class CambioEscena : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre exacto de la escena a la que quieres ir")]
    public string nombreEscenaDestino;

    [Tooltip("Tag del objeto que activa el portal (por ejemplo: Player)")]
    public string tagJugador = "Player";

    // Se ejecuta cuando un objeto entra en el collider configurado como Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró es el jugador
        if (other.CompareTag(tagJugador))
        {
            CambiarDeEscena();
        }
    }

    void CambiarDeEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogWarning("No se ha asignado ningún nombre de escena en el portal.");
        }
    }
}