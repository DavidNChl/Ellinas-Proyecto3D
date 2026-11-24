using UnityEngine;

public class BakeTreeObstacles : MonoBehaviour
{
    [Tooltip("Ancho del agujero alrededor de cada tronco")]
    [SerializeField] private float trunkDiameter = 1.4f;

    [ContextMenu("1. Crear Obstaculos Temporales")]
    public void CreateObstacles()
    {
        ClearObstacles();
        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;
        GameObject container = new GameObject("Temp_TreeObstacles");

        foreach (TreeInstance tree in data.treeInstances)
        {
            Vector3 worldPos = Vector3.Scale(tree.position, data.size) + terrain.transform.position;
            GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cyl.transform.position = worldPos;
            cyl.transform.localScale = new Vector3(trunkDiameter, 5f, trunkDiameter);
            cyl.transform.parent = container.transform;
        }
    }

    [ContextMenu("2. Borrar Obstaculos Temporales")]
    public void ClearObstacles()
    {
        GameObject existing = GameObject.Find("Temp_TreeObstacles");
        if (existing != null) DestroyImmediate(existing);
    }
}