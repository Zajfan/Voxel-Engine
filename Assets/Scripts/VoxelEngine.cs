using UnityEngine;

public class VoxelEngine : MonoBehaviour
{
    public int worldSize = 16;
    public int chunkSize = 16;

    private Chunk[,,] chunks;

    void Start()
    {
        chunks = new Chunk[worldSize, worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    CreateChunk(x, y, z);
                }
            }
        }
    }

    void CreateChunk(int x, int y, int z)
    {
        GameObject chunkObject = new GameObject("Chunk (" + x + ", " + y + ", " + z + ")");
        chunkObject.transform.parent = transform;
        chunkObject.transform.position = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.chunkPosition = new Vector3Int(x, y, z);
        chunk.chunkSize = chunkSize;

        chunks[x, y, z] = chunk;
    }
}