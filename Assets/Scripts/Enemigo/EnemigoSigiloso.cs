using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoSigiloso : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadAcecho = 2.5f;
    public float velocidadAtaque = 5.0f;
    [Range(0f, 100f)] public float probabilidadAcechar = 50f;

    [Header("Detección Visión")]
    public float distanciaMaximaRayo = 30f;
    public LayerMask capasObstaculos;

    [Header("Optimización")]
    [SerializeField] private float intervaloActualizacionIA = 0.2f; // 5 veces/seg en lugar de 60

    private NavMeshAgent agent;
    private Camera camaraJugador;
    private Transform camaraTransform;
    private Transform jugadorTransform;
    private Collider enemigoCollider;
    private bool esAcechador = false;
    private float temporizadorIA;

    // Arreglo estático reutilizable: elimina los 256 B de GC Alloc por frame
    private static readonly Plane[] planosFrustumCache = new Plane[6];
    private static int ultimoFrameFrustum = -1;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemigoCollider = GetComponent<Collider>();
    }

    void Start()
    {
        ControladorJugador jugador = FindFirstObjectByType<ControladorJugador>();

        if (jugador != null)
        {
            jugadorTransform = jugador.transform;
            Transform vistaJugador = jugador.camaraTransform;

            if (vistaJugador != null)
                camaraJugador = vistaJugador.GetComponent<Camera>();

            if (camaraJugador == null)
                camaraJugador = Camera.main;

            if (camaraJugador != null)
                camaraTransform = camaraJugador.transform;
        }

        esAcechador = Random.Range(0f, 100f) <= probabilidadAcechar;
        // Desfase aleatorio inicial para repartir la carga de Raycast/NavMesh entre frames
        temporizadorIA = Random.Range(0f, intervaloActualizacionIA);
    }

    void Update()
    {
        if (jugadorTransform == null || camaraJugador == null) return;

        temporizadorIA -= Time.deltaTime;
        if (temporizadorIA > 0f) return;
        temporizadorIA = intervaloActualizacionIA;

        bool enRabia = ManejadorHorda.Instancia != null && ManejadorHorda.Instancia.modoRabiaActivado;

        if (enRabia)
        {
            agent.isStopped = false;
            agent.speed = velocidadAtaque;
            agent.SetDestination(jugadorTransform.position);
            return;
        }

        if (!esAcechador)
        {
            if (!agent.isStopped) agent.isStopped = true;
            return;
        }

        if (EsVistoPorJugador())
        {
            if (!agent.isStopped) agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.speed = velocidadAcecho;
            agent.SetDestination(jugadorTransform.position);
        }
    }

    bool EsVistoPorJugador()
    {
        // Calcular los planos de cámara máximo 1 vez por frame para TODOS los enemigos (0 B GC Alloc)
        if (ultimoFrameFrustum != Time.frameCount)
        {
            GeometryUtility.CalculateFrustumPlanes(camaraJugador, planosFrustumCache);
            ultimoFrameFrustum = Time.frameCount;
        }

        Bounds bounds = enemigoCollider != null ? enemigoCollider.bounds : new Bounds(transform.position, Vector3.one);

        if (!GeometryUtility.TestPlanesAABB(planosFrustumCache, bounds))
            return false;

        Vector3 origenCamara = camaraTransform.position;
        Vector3 direccionHaciaEnemigo = transform.position - origenCamara;

        if (Physics.Raycast(origenCamara, direccionHaciaEnemigo.normalized, out RaycastHit hit, distanciaMaximaRayo, capasObstaculos))
        {
            if (hit.transform != transform && !hit.transform.IsChildOf(transform))
                return false;
        }

        return true;
    }
}