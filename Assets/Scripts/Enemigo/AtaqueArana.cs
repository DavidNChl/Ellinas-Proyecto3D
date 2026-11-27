using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AtaqueArana : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AranaAnimationController animController;

    [Header("Configuración de Ataque")]
    [SerializeField] private float rangoAtaque = 2.2f;
    [SerializeField] private float alcanceGolpe = 2.8f;
    [SerializeField] private float danoPorGolpe = 25f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;
    [Tooltip("Segundos desde que inicia la animación hasta que las patas golpean (ej. frame 25 de 60fps = 0.42s).")]
    [SerializeField] private float retardoImpacto = 0.42f;

    private Transform jugador;
    private SaludJugador saludJugador;
    private float temporizadorAtaque;
    private bool golpeYaAplicadoEnEsteAtaque;

    public bool PuedeAtacar { get; set; } = true;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animController == null)
            animController = GetComponent<AranaAnimationController>();
    }

    private void Start()
    {
        ControladorJugador controlador = FindFirstObjectByType<ControladorJugador>();
        if (controlador != null)
        {
            jugador = controlador.transform;
            saludJugador = controlador.GetComponentInParent<SaludJugador>();
        }

        if (saludJugador == null)
            Debug.LogError("AtaqueArana: No se encontró SaludJugador en el objeto del jugador.", this);
    }

    private void Update()
    {
        if (jugador == null || saludJugador == null) return;

        temporizadorAtaque -= Time.deltaTime;
        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (PuedeAtacar && distancia <= rangoAtaque && !agent.isStopped && temporizadorAtaque <= 0f)
        {
            IniciarAtaque();
        }
    }

    private void IniciarAtaque()
    {
        temporizadorAtaque = tiempoEntreAtaques;
        golpeYaAplicadoEnEsteAtaque = false;

        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(direccion);

        if (animController != null)
            animController.ReproducirAtaque();

        // Respaldo sincronizado por tiempo por si el Animator está en un hijo y no dispara el Animation Event
        StartCoroutine(AplicarDanoConRetardo());
    }

    private IEnumerator AplicarDanoConRetardo()
    {
        yield return new WaitForSeconds(retardoImpacto);
        AplicarDanoGolpe();
    }

    // Puede ser llamado tanto por el Animation Event como por la corrutina (solo aplicará daño una vez por ataque)
    public void AplicarDanoGolpe()
    {
        if (golpeYaAplicadoEnEsteAtaque || jugador == null || saludJugador == null) return;
        if (agent.isStopped) return;

        float distanciaActual = Vector3.Distance(transform.position, jugador.position);
        if (distanciaActual <= alcanceGolpe)
        {
            golpeYaAplicadoEnEsteAtaque = true;
            saludJugador.RecibirDano(danoPorGolpe);
        }
    }
}