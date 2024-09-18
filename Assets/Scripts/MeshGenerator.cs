using UnityEngine;
using System.Collections.Generic;

public class MeshGenerator
{
    private Dictionary<VoxelType, Material> voxelMaterials = new Dictionary<VoxelType, Material>();

    public MeshGenerator()
    {
        voxelMaterials[VoxelType.Air] = null;
        voxelMaterials[VoxelType.Dirt] = Resources.Load<Material>("Materials/DirtMaterial");
        voxelMaterials[VoxelType.Granite] = Resources.Load<Material>("Materials/GraniteMaterial");
        voxelMaterials[VoxelType.Limestone] = Resources.Load<Material>("Materials/LimestoneMaterial");
        voxelMaterials[VoxelType.Marble] = Resources.Load<Material>("Materials/MarbleMaterial");
        voxelMaterials[VoxelType.Sandstone] = Resources.Load<Material>("Materials/SandstoneMaterial");
        voxelMaterials[VoxelType.Grass] = Resources.Load<Material>("Materials/GrassMaterial");
    }

    public Mesh GenerateMesh(Voxel[,,] voxels, int chunkSize)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int>[] submeshTriangles = new List<int>[voxelMaterials.Count];
        for (int i = 0; i < submeshTriangles.Length; i++)
        {
            submeshTriangles[i] = new List<int>();
        }

        for (int d = 0; d < 6; d++)
        {
            bool[,] faceMask = new bool[chunkSize, chunkSize];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        if (voxels[x, y, z] != null && !faceMask[x, z])
                        {
                            int quadWidth = 1, quadHeight = 1;
                            while (x + quadWidth < chunkSize && voxels[x + quadWidth, y, z] != null && !faceMask[x + quadWidth, z]) quadWidth++;
                            while (z + quadHeight < chunkSize && voxels[x, y, z + quadHeight] != null && !faceMask[x, z + quadHeight]) quadHeight++;

                            CreateQuad(d, x, y, z, quadWidth, quadHeight, ref vertices, ref submeshTriangles, voxels);

                            for (int i = 0; i < quadWidth; i++)
                                for (int j = 0; j < quadHeight; j++)
                                    faceMask[x + i, z + j] = true;
                        }
                    }
                }
            }
        }

        void CreateQuad(int direction, int x, int y, int z, int width, int height, ref List<Vector3> vertices, ref List<int> triangles)
    {
        int vertexIndex = vertices.Count;

        // Top Face
        if (direction == 0)
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + width, y + 1, z));
            vertices.Add(new Vector3(x + width, y + 1, z + height));
            vertices.Add(new Vector3(x, y + 1, z + height));
        }
        // Bottom Face
        else if (direction == 1)
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + width, y, z));
            vertices.Add(new Vector3(x + width, y, z + height));
            vertices.Add(new Vector3(x, y, z + height));
        }
        // Left Face
        else if (direction == 2)
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + height));
            vertices.Add(new Vector3(x, y + height, z + height));
            vertices.Add(new Vector3(x, y + height, z));
        }
        // Right Face
        else if (direction == 3)
        {
            vertices.Add(new Vector3(x + width, y, z + height));
            vertices.Add(new Vector3(x + width, y + height, z + height));
            vertices.Add(new Vector3(x + width, y + height, z));
            vertices.Add(new Vector3(x + width, y, z));
        }
        // Front Face
        else if (direction == 4)
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + width, y, z));
            vertices.Add(new Vector3(x + width, y + height, z));
            vertices.Add(new Vector3(x, y + height, z));
        }
        // Back Face
        else if (direction == 5)
        {
            vertices.Add(new Vector3(x + width, y, z + height));
            vertices.Add(new Vector3(x, y, z + height));
            vertices.Add(new Vector3(x, y + height, z + height));
            vertices.Add(new Vector3(x + width, y + height, z + height));
        }

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
    }
        // Get the voxel type and corresponding submesh index
        VoxelType voxelType = voxels[x, y, z].type;
        int submeshIndex = (int)voxelType;

        // Add triangles to the appropriate submesh
        submeshTriangles[submeshIndex].Add(vertexIndex);
        submeshTriangles[submeshIndex].Add(vertexIndex + 1);
        submeshTriangles[submeshIndex].Add(vertexIndex + 2);
        submeshTriangles[submeshIndex].Add(vertexIndex + 2);
        submeshTriangles[submeshIndex].Add(vertexIndex + 3);
        submeshTriangles[submeshIndex].Add(vertexIndex);
    }