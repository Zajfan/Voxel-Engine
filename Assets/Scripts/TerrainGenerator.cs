using UnityEngine;

public class TerrainGenerator
{
    // Add FBM parameters here as well
    public int octaves = 4;
    public float lacunarity = 2.0f;
    public float persistence = 0.5f;

    public void GenerateTerrain(Voxel[,,] voxels, Vector3Int chunkPosition, int chunkSize)
    {
        // Generate terrain using Perlin noise with FBM for detail
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float height = 0;
                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < octaves; i++)
                {
                    height += Mathf.PerlinNoise(
                        (chunkPosition.x * chunkSize + x) * frequency * 0.1f,
                        (chunkPosition.z * chunkSize + z) * frequency * 0.1f
                    ) * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                height *= 32f;
                height = Mathf.Clamp(height, 0, chunkSize - 1);

                for (int y = 0; y < height; y++)
                {
                    // Assign voxel types based on height (adjust thresholds as needed)
                    VoxelType type = VoxelType.Air;
                    if (y < height * 0.2f)
                        type = VoxelType.Granite; // Deepest layer is granite
                    else if (y < height * 0.4f)
                        type = VoxelType.Limestone;
                    else if (y < height * 0.6f)
                        type = VoxelType.Marble;
                    else if (y < height * 0.8f)
                        type = VoxelType.Sandstone;
                    else if (y < height)
                        type = VoxelType.Dirt;
                    else if (y == height)
                        type = VoxelType.Grass;

                    voxels[x, y, z] = new Voxel { position = new Vector3(x, y, z), dimensions = Vector3.one, type = type };
                }
            }
        }
    }
}