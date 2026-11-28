using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyVisibility : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CameraPerspectiveController perspectiveController;
    [SerializeField] private Renderer[] renderersCuerpo;
    [Tooltip("Arrastra aquí ArmaEnMano para que nunca se oculte en Primera Persona.")]
    [SerializeField] private Transform raizArma;

    [Header("Sincronización con Cinemachine")]
    [Tooltip("Tiempo en segundos que tarda en ocultarse al pasar a Primera Persona (ajústalo al Default Blend de tu CinemachineBrain).")]
    [SerializeField] private float retrasoOcultar = 0.25f;

    private Coroutine rutinaVisibilidad;

    private void Awake()
    {
        if (perspectiveController == null)
            perspectiveController = GetComponent<CameraPerspectiveController>();

        if (renderersCuerpo == null || renderersCuerpo.Length == 0)
        {
            Renderer[] todos = GetComponentsInChildren<Renderer>(true);
            List<Renderer> filtrados = new List<Renderer>();

            foreach (Renderer r in todos)
            {
                // Si el renderer pertenece al arma o sus hijos, no lo añadimos a la lista de ocultar
                if (raizArma != null && (r.transform == raizArma || r.transform.IsChildOf(raizArma)))
                    continue;

                filtrados.Add(r);
            }

            renderersCuerpo = filtrados.ToArray();
        }
    }
    private void OnEnable(){
        if (perspectiveController != null)
        perspectiveController.PerspectiveChanged += ActualizarVisibilidad;

    }



    private void OnDisable(){
        if (perspectiveController != null)
        perspectiveController.PerspectiveChanged -= ActualizarVisibilidad;
    }



    private void Start(){
        if (perspectiveController != null)
        {

        // Al iniciar la escena se aplica sin retraso para evitar parpadeos en el primer frame

        AplicarEstadoRenderers(perspectiveController.CurrentPerspective == PlayerPerspective.ThirdPerson);

        }

    }



    private void ActualizarVisibilidad(PlayerPerspective nuevaPerspectiva)
    {

        if (rutinaVisibilidad != null)

            StopCoroutine(rutinaVisibilidad);



        if (nuevaPerspectiva == PlayerPerspective.ThirdPerson)
        {
            // Al volver a Tercera Persona aparece al instante mientras la cámara se aleja
            AplicarEstadoRenderers(true);
        }
        else
        {
            // Al pasar a Primera Persona espera a que la cámara se acerque antes de ocultarlo

            rutinaVisibilidad = StartCoroutine(OcultarConRetraso());
        }
    }



    private IEnumerator OcultarConRetraso()

    {

        yield return new WaitForSeconds(retrasoOcultar);

        AplicarEstadoRenderers(false);

        rutinaVisibilidad = null;

    }



    private void AplicarEstadoRenderers(bool mostrar)

    {

        foreach (Renderer rend in renderersCuerpo)

        {

        if (rend != null)

        rend.enabled = mostrar;

        }

    }

}