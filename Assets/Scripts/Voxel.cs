// Add other properties you might need for your voxels, such as:
// public Color color;
// public Material material;
// public bool isActive; // To indicate if the voxel is solid or empty
using UnityEngine;

public class Voxel
{
    public Vector3 position;
    public Vector3 dimensions;
    public VoxelType type;
}

public enum VoxelType
{
    Air,
    Dirt,
    Granite,
    Limestone,
    Marble,
    Sandstone,
    Grass
    // Feel free to add even more types later!
}