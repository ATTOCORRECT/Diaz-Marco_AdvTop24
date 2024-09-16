using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    Vector3Int chunk_grid_size = new Vector3Int(8, 8, 8); // dimensions of a chunk in meters
    int chunk_lattice_row, chunk_lattice_slice, chunk_lattice_volume; // shorthands for components of the grid

    Vector3Int chunk_lattice_size; // the lattice of the chunk grid (think fence posts vs fences)
    int chunk_grid_row, chunk_grid_slice, chunk_grid_volume;

    List<List<ArrayList>> cells = new List<List<ArrayList>>();

    float[] densities;

    int[] triangulation_indicies;


    public Gradient gradient;

    void Start()
    {
        chunk_lattice_size = chunk_grid_size + Vector3Int.one;

        GetGridParts(chunk_lattice_size, out chunk_lattice_row, out chunk_lattice_slice, out chunk_lattice_volume);
        GetGridParts(chunk_grid_size   , out chunk_grid_row   , out chunk_grid_slice   , out chunk_grid_volume   );

        densities = new float[chunk_lattice_volume];

        for (int i = 0; i < chunk_lattice_volume; i++)
        {
            Vector3Int point_position = GridPosition(i, chunk_lattice_size);

            densities[i] = GetDensity(point_position);

            //Debug.Log(density);
            //Color color = gradient.Evaluate((density + 1) / 2f);
            //if (density > 0) DrawPoint(point_position, color);
        }

        for (int i = 0; i < chunk_grid_volume; i++)
        {
            Vector3Int point_position = GridPosition(i, chunk_grid_size);

            int chunk_lattice_index = point_position.x + (point_position.y * chunk_lattice_row) + (point_position.z * chunk_lattice_slice);

            List<ArrayList> cell_vertex_data = new List<ArrayList>();

            cell_vertex_data.Add(new ArrayList() { point_position                          , densities[chunk_lattice_index                                              ] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(1, 0, 0), densities[chunk_lattice_index + 1                                          ] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(1, 1, 0), densities[chunk_lattice_index + 1 + chunk_lattice_row                      ] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(0, 1, 0), densities[chunk_lattice_index     + chunk_lattice_row                      ] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(0, 0, 1), densities[chunk_lattice_index                         + chunk_lattice_slice] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(1, 0, 1), densities[chunk_lattice_index + 1                     + chunk_lattice_slice] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(1, 1, 1), densities[chunk_lattice_index + 1 + chunk_lattice_row + chunk_lattice_slice] });
            cell_vertex_data.Add(new ArrayList() { point_position + new Vector3Int(0, 1, 1), densities[chunk_lattice_index     + chunk_lattice_row + chunk_lattice_slice] });

            cells.Add(cell_vertex_data);
        }

        triangulation_indicies = new int[chunk_grid_volume];
        for (int i = 0; i < chunk_grid_volume; i++)
        {
            triangulation_indicies[i] = 0;
            for (int j = 0; j < 8; j++)
            {
                float cell_vertex_density = (float)cells[i][j][1];
                if (cell_vertex_density > 0) triangulation_indicies[i] |= 1 << j;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < chunk_grid_volume; i++)
        {
            Vector3Int point_position = GridPosition(i, chunk_grid_size);
            //triangulation_indicies[i] / 255f
            Color color = Color.HSVToRGB(triangulation_indicies[i] / 255f, 1, 1);
            if (triangulation_indicies[i] != 0) DrawUnitCube(point_position, color);
        }
    }

    void BreakVector3IntIntoComponents(Vector3Int v, out int x, out int y, out int z)
    {
        x = v.x;
        y = v.y;
        z = v.z;
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
        Debug.DrawRay(position - Vector3.right   / 10, Vector3.right   / 5, color);
        Debug.DrawRay(position - Vector3.up      / 10, Vector3.up      / 5, color);
        Debug.DrawRay(position - Vector3.forward / 10, Vector3.forward / 5, color);
    }

    void DrawUnitCube(Vector3 position, Color color)
    {
        DrawCube(position + Vector3.one / 100f, Vector3.one * 49f / 50f, color);
    }

    void DrawCube(Vector3 position, Vector3 size, Color color)
    {
        Debug.DrawRay(position, new Vector3(size.x, 0, 0), color);
        Debug.DrawRay(position, new Vector3(0, size.y, 0), color);
        Debug.DrawRay(position, new Vector3(0, 0, size.z), color);

        Debug.DrawRay(position + new Vector3(size.x, 0, 0), new Vector3(0, size.y, 0), color);
        Debug.DrawRay(position + new Vector3(size.x, 0, 0), new Vector3(0, 0, size.z), color);

        Debug.DrawRay(position + new Vector3(0, size.y, 0), new Vector3(size.x, 0, 0), color);
        Debug.DrawRay(position + new Vector3(0, size.y, 0), new Vector3(0, 0, size.z), color);

        Debug.DrawRay(position + new Vector3(0, 0, size.z), new Vector3(size.x, 0, 0), color);
        Debug.DrawRay(position + new Vector3(0, 0, size.z), new Vector3(0, size.y, 0), color);

        Debug.DrawRay(position + size, new Vector3(-size.x, 0, 0), color);
        Debug.DrawRay(position + size, new Vector3(0, -size.y, 0), color);
        Debug.DrawRay(position + size, new Vector3(0, 0, -size.z), color);
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
