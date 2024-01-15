using UnityEngine;

public class DiscoFloorGenerator : MonoBehaviour
{
    public GameObject platformPrefab1; // Assign your first platform prefab in the Inspector
    public GameObject platformPrefab2; // Assign your second platform prefab in the Inspector
    public int width = 10; // Width of the floor
    public int depth = 10; // Depth of the floor
    public float tileSize = 1f; // Size of each tile

    void Start()
    {
        GenerateFloor();
    }

    void GenerateFloor()
    {
        // Calculate the offset to center the floor around (0, 0, 0)
        float offsetX = (width % 2 == 0) ? tileSize / 2f : 0;
        float offsetZ = (depth % 2 == 0) ? tileSize / 2f : 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Calculate the position for each tile with offset for centering around (0, 0, 0)
                Vector3 position = new Vector3((x - width / 2) * tileSize + offsetX, -0.04f, (z - depth / 2) * tileSize + offsetZ);

                // Alternate between two prefabs for the chessboard effect
                GameObject selectedPrefab = (x + z) % 2 == 0 ? platformPrefab1 : platformPrefab2;

                // Instantiate the selected platform prefab
                Instantiate(selectedPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
