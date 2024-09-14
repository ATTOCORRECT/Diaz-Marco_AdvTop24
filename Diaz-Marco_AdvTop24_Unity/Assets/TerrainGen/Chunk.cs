using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    Vector3Int chunk_size = new Vector3Int(16, 16, 16); // dimensions of a chunk in meters
    Vector3Int chunk_grid_size; // the gridpoints of a chunk grid (think fence posts vs fences)

    void Start()
    {
        chunk_grid_size = chunk_size + Vector3Int.one;

        GetGridParts(chunk_grid_size, out int chunk_grid_row, out int chunk_grid_slice, out int chunk_grid_volume);

        for (int i = 0; i < chunk_grid_volume; i++)
        {
            Vector3Int point_position = GridPosition(i, chunk_grid_size);

            float density = GetDensity(point_position);

            Debug.Log(density);
            Color color = Color.Lerp(Color.black, Color.white, density);
            if (density > 0) DrawPoint(point_position, Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3Int GridPosition(int index, Vector3Int grid_size) // from an index get a position in a grid
    {
        int x =  index                                % grid_size.x;
        int y = (index /  grid_size.x)                % grid_size.y;
        int z = (index / (grid_size.x * grid_size.y)) % grid_size.z;
        return new Vector3Int(x, y, z);
    }

    void GetGridParts(Vector3Int grid_size, out int grid_row, out int grid_slice, out int grid_volume) // split a grid into its components for easier math
    {
        grid_row    = grid_size.x;
        grid_slice  = grid_size.x * grid_size.y;
        grid_volume = grid_size.x * grid_size.y * grid_size.z;
    }

    void DrawPoint(Vector3 position, Color color)
    {
        Debug.DrawRay(position - Vector3.right   / 10, Vector3.right   / 5, color, float.PositiveInfinity);
        Debug.DrawRay(position - Vector3.up      / 10, Vector3.up      / 5, color, float.PositiveInfinity);
        Debug.DrawRay(position - Vector3.forward / 10, Vector3.forward / 5, color, float.PositiveInfinity);
    }

    float GetDensity(Vector3Int position)
    {
        return Mathf.Clamp(Noise(position, 0.1f, 1), -1, 1);
    }

    float Noise(Vector3 position, float scale, int octaves)
    {
        position *= scale;

        float noise = 0;
        for (int i = 0; i < octaves; i++)
        {
            noise += PerlinNoise3D(position * Mathf.Pow(2, i)) / Mathf.Pow(2, i);
        }

        return noise;
    }

    public static float PerlinNoise3D(Vector3 position)
    { 
        // https://discussions.unity.com/t/3d-perlin-noise/134957

        float x = position.x;
        float y = position.y;
        float z = position.z;

        y += 1;
        z += 2;
        float xy = perlin3DFixed(x, y);
        float xz = perlin3DFixed(x, z);
        float yz = perlin3DFixed(y, z);
        float yx = perlin3DFixed(y, x);
        float zx = perlin3DFixed(z, x);
        float zy = perlin3DFixed(z, y);

        return (((xy * xz * yz * yx * zx * zy) * 2) - 1);
    }

    static float perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }
}
