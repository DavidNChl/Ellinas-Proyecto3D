
using UnityEngine;

public class OptimizadorFaroles : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Distancias")]
    public float distanciaActivacion = 70f;
    public float intervaloRevision = 0.5f;

    private Light[] luces;
    private float temporizador;

    void Start()
    {
        luces = GetComponentsInChildren<Light>(true);
    }

    void Update()
    {
        if (jugador == null || luces == null)
            return;

        temporizador -= Time.deltaTime;
        if (temporizador > 0f)
            return;

        temporizador = intervaloRevision;

        foreach (Light luz in luces)
        {
            if (luz == null)
                continue;

            float distancia = Vector3.Distance(
                jugador.position, luz.transform.position);

            bool activar = distancia <= distanciaActivacion;

            if (luz.enabled != activar)
                luz.enabled = activar;
        }
    }
}
