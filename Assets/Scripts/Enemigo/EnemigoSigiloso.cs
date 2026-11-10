using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoSigiloso : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadAcecho = 2.5f;
    public float velocidadAtaque = 5.0f;
    [Range(0f, 100f)] public float probabilidadAcechar = 50f; // Probabilidad de actuar agresivo al inicio

    [Header("Detección Visión")]
    public float distanciaMaximaRayo = 30f;
    public LayerMask capasObstaculos; // Capas que tapan la visión (Paredes, Obstáculos)

    private NavMeshAgent agent;
    private Camera camaraJugador;
    private Transform jugadorTransform;
    private bool esAcechador = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Busca automáticamente al jugador por la cámara del ControladorJugador
        ControladorJugador jugador = FindFirstObjectByType<ControladorJugador>();
        if (jugador != null)
        {
            jugadorTransform = jugador.transform;
            camaraJugador = jugador.camaraTransform.GetComponent<Camera>();
        }

        // Determina si este enemigo individual intentará acercarse
        esAcechador = Random.Range(0f, 100f) <= probabilidadAcechar;
    }

    void Update()
    {
        if (jugadorTransform == null || camaraJugador == null) return;

        bool enRabia = ManejadorHorda.Instancia != null && ManejadorHorda.Instancia.modoRabiaActivado;

        // 1. MODO RABIA: Reanudar movimiento, aumentar velocidad y perseguir sin parar
        if (enRabia)
        {
            agent.isStopped = false; // <-- ESTO REANUDA EL MOVIMIENTO
            agent.speed = velocidadAtaque;
            agent.SetDestination(jugadorTransform.position);
            return;
        }

        // 2. Si no es un enemigo acechador activo, permanece inmóvil
        if (!esAcechador)
        {
            agent.isStopped = true;
            return;
        }

        // 3. Evaluar si el jugador lo está mirando
        bool estaSiendoMirado = EsVistoPorJugador();

        if (estaSiendoMirado)
        {
            // Se congela mientras lo miran
            agent.isStopped = true;
        }
        else
        {
            // Avanza sigilosamente hacia el jugador
            agent.isStopped = false;
            agent.speed = velocidadAcecho;
            agent.SetDestination(jugadorTransform.position);
        }
    }

    bool EsVistoPorJugador()
    {
        // A) Comprobar si el enemigo entra en el cono (frustum) de la cámara
        Plane[] planos = GeometryUtility.CalculateFrustumPlanes(camaraJugador);
        Bounds bounds = GetComponent<Collider>() != null ? GetComponent<Collider>().bounds : new Bounds(transform.position, Vector3.one);

        if (!GeometryUtility.TestPlanesAABB(planos, bounds))
        {
            return false; // Está fuera de la pantalla del jugador
        }

        // B) Comprobar mediante Raycast si hay paredes u obstáculos en medio
        Vector3 direccionHaciaEnemigo = transform.position - camaraJugador.transform.position;
        if (Physics.Raycast(camaraJugador.transform.position, direccionHaciaEnemigo.normalized, out RaycastHit hit, distanciaMaximaRayo, capasObstaculos))
        {
            if (hit.transform != transform && !hit.transform.IsChildOf(transform))
            {
                return false; // Hay un objeto/pared tapando al enemigo
            }
        }

        return true; // Está dentro de pantalla y sin obstáculos enfrente
    }

    // Llamar a este método desde tu sistema de armas/combate al matar al enemigo
    public void Morir()
    {
        if (ManejadorHorda.Instancia != null)
        {
            ManejadorHorda.Instancia.RegistrarMuerteEnemigo();
        }
        Destroy(gameObject);
    }
}