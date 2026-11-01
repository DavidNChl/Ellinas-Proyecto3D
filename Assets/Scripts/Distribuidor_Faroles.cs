
using UnityEngine;

public class DistribuirFaroles : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabFarol;

    [Header("Recorrido")]
    public Transform[] puntos;
    public float distanciaEntreFaroles = 20f;
    public float alturaExtra = 0f;
    public float separacionLateral = 0f;

    [ContextMenu("Generar faroles")]
    public void GenerarFaroles()
    {
        if (prefabFarol == null || puntos == null ||
            puntos.Length < 2)
        {
            Debug.LogWarning("Asigna el prefab y al menos 2 puntos.");
            return;
        }

        // Eliminar la generación anterior
        Transform anterior = transform.Find("Faroles_Generados");
        if (anterior != null)
        {
            for (int i = anterior.childCount - 1; i >= 0; i--)
                DestroyImmediate(anterior.GetChild(i).gameObject);

            DestroyImmediate(anterior.gameObject);
        }

        GameObject contenedor = new GameObject("Faroles_Generados");
        contenedor.transform.SetParent(transform, false);

        float acumulado = 0f;
        float siguiente = 0f;

        for (int i = 0; i < puntos.Length - 1; i++)
        {
            Vector3 a = puntos[i].position;
            Vector3 b = puntos[i + 1].position;

            Vector3 horizontal = b - a;
            horizontal.y = 0f;

            float longitud = horizontal.magnitude;
            if (longitud < 0.01f) continue;

            Vector3 direccion = horizontal.normalized;
            Vector3 lateral = Vector3.Cross(
                Vector3.up, direccion).normalized;

            while (siguiente <= acumulado + longitud)
            {
                float t = (siguiente - acumulado) / longitud;
                Vector3 pos = Vector3.Lerp(a, b, t);
                pos += lateral * separacionLateral;

                Terrain terreno = Terrain.activeTerrain;
                if (terreno != null)
                {
                    pos.y = terreno.SampleHeight(pos)
                        + terreno.transform.position.y
                        + alturaExtra;
                }
                else
                {
                    pos.y += alturaExtra;
                }

                // Orientar el farol siguiendo el recorrido
                GameObject farol = Instantiate(
                    prefabFarol, pos,
                    Quaternion.LookRotation(direccion),
                    contenedor.transform);

                siguiente += Mathf.Max(1f, distanciaEntreFaroles);
            }

            acumulado += longitud;
        }

        Debug.Log("Faroles generados: " +
                  contenedor.transform.childCount);
    }
}
