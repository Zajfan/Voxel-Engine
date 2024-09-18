using UnityEngine;
using System.Collections.Generic;

public class ErosionSimulator
{
    public float erosionStrength = 0.01f;
    public float depositionRate = 0.5f;
    public float evaporationRate = 0.01f;

    public void SimulateErosion(Voxel[,,] voxels, int chunkSize)
    {
        List<Chunk.WaterDroplet> droplets = new List<Chunk.WaterDroplet>();
        for (int i = 0; i < 100; i++)
        {
            droplets.Add(new Chunk.WaterDroplet
            {
                position = new Vector3(Random.Range(0, chunkSize), chunkSize, Random.Range(0, chunkSize)),
                velocity = Vector3.zero,
                waterAmount = 1.0f,
                sedimentAmount = 0.0f
            });
        }

        for (int i = 0; i < droplets.Count; i++)
        {
            Chunk.WaterDroplet droplet = droplets[i];

            // Calculate Flow Direction
            Vector3Int dropletVoxelPos = new Vector3Int(
                Mathf.FloorToInt(droplet.position.x),
                Mathf.FloorToInt(droplet.position.y),
                Mathf.FloorToInt(droplet.position.z)
            );

            Vector3Int steepestDescent = Vector3Int.zero;
            float steepestSlope = 0;
            for (int nx = -1; nx <= 1; nx++)
            {
                for (int ny = -1; ny <= 1; ny++)
                {
                    for (int nz = -1; nz <= 1; nz++)
                    {
                        if (nx == 0 && ny == 0 && nz == 0 ||
                            !IsVoxelInChunkBounds(dropletVoxelPos.x + nx, dropletVoxelPos.y + ny, dropletVoxelPos.z + nz, chunkSize))
                            continue;

                        float neighborHeight = GetVoxelHeight(voxels, dropletVoxelPos.x + nx, dropletVoxelPos.y + ny, dropletVoxelPos.z + nz, chunkSize);
                        float slope = (GetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, chunkSize) - neighborHeight) /
                                      Vector3Int.Distance(Vector3Int.zero, new Vector3Int(nx, ny, nz));

                        if (slope > steepestSlope)
                        {
                            steepestSlope = slope;
                            steepestDescent = new Vector3Int(nx, ny, nz);
                        }
                    }
                }
            }

            // Erosion
            if (steepestSlope > 0)
            {
                float amountToErode = Mathf.Min(droplet.waterAmount * erosionStrength, GetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, chunkSize));
                droplet.sedimentAmount += amountToErode;
                SetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, GetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, chunkSize) - amountToErode, chunkSize);
            }

            // Deposition
            if (droplet.sedimentAmount > 0 && steepestSlope < 0.1f)
            {
                float amountToDeposit = droplet.sedimentAmount * depositionRate;
                droplet.sedimentAmount -= amountToDeposit;
                SetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, GetVoxelHeight(voxels, dropletVoxelPos.x, dropletVoxelPos.y, dropletVoxelPos.z, chunkSize) + amountToDeposit, chunkSize);
            }

            // Evaporation
            droplet.waterAmount -= evaporationRate;

            // Move Droplet
            droplet.velocity += (Vector3)steepestDescent * steepestSlope;
            droplet.position += droplet.velocity;

            // Remove droplet if it's out of bounds or has no water left
            if (!IsVoxelInChunkBounds(Mathf.FloorToInt(droplet.position.x), Mathf.FloorToInt(droplet.position.y), Mathf.FloorToInt(droplet.position.z), chunkSize) ||
                droplet.waterAmount <= 0)
            {
                droplets.RemoveAt(i);
                i--;
            }
        }
    }

    // Helper functions to get and set voxel heights 
    float GetVoxelHeight(Voxel[,,] voxels, int x, int y, int z, int chunkSize)
    {
        if (IsVoxelInChunkBounds(x, y, z, chunkSize) && voxels[x, y, z] != null)
            return y + 1;
        else
            return 0;
    }

    void SetVoxelHeight(Voxel[,,] voxels, int x, int y, int z, float newHeight, int chunkSize)
    {
        if (IsVoxelInChunkBounds(x, y, z, chunkSize))
        {
            int newY = Mathf.FloorToInt(newHeight);
            if (newY >= 0 && newY < chunkSize)
            {
                if (voxels[x, y, z] != null)
                    voxels[x, y, z] = null;

                if (newY > 0)
                    voxels[x, newY - 1, z] = new Voxel { position = new Vector3(x, newY - 1, z), dimensions = Vector3.one };
            }
        }
    }

    bool IsVoxelInChunkBounds(int x, int y, int z, int chunkSize)
    {
        return x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize;
    }
}