using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class Chunk : MonoBehaviour
{
    // Chunk Mesh Generation Variables
    [SerializeField]
    private ComputeShader marchingCubesCompute;

    private ComputeBuffer triangle_buffer;
    private ComputeBuffer triangle_count_buffer;
    private ComputeBuffer densities_buffer;

    private bool generatedBefore = false;

    private TerrainData procedural_terrain;

    private static readonly Vector3Int chunk_grid_size = new Vector3Int(32, 32, 32); // dimensions of a chunk in meters

    private Vector3Int chunk_lattice_size; // the lattice of the chunk grid (think fence posts vs fences)
    private int chunk_lattice_row, chunk_lattice_slice, chunk_lattice_volume; // shorthands for components of the grid

    private Mesh mesh;
    // -----

    // Chunk Material Variables
    private Material chunk_material;
    private Texture2D surface_color_map;

    public void GenerateChunk()
    {
        if (!generatedBefore)
        {
            generatedBefore = true;

            chunk_lattice_size = chunk_grid_size + Vector3Int.one;

            mesh = new Mesh();
            gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            Utils.GetGridParts(chunk_lattice_size, out chunk_lattice_row, out chunk_lattice_slice, out chunk_lattice_volume);
        }

        GenerateMesh();
    }

    public void SetChunkProceduralTerrain(ref TerrainData procedural_terrain)
    {
        this.procedural_terrain = procedural_terrain;
    }

    public void SetChunkMaterial(Material material)
    {
        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    public static Vector3Int GetChunkSize()
    {
        return chunk_grid_size;
    }

    private void GenerateMesh()
    {
        MarchingCubes();
        // there used to be more here
    }

    private void MarchingCubes()
    {

        // setup parameters and copy to compute buffer
        int max_triangle_count = (chunk_grid_size.x * chunk_grid_size.y * chunk_grid_size.z) * 5;
        triangle_buffer = new ComputeBuffer(max_triangle_count, sizeof(float) * 3 * 3, ComputeBufferType.Append);

        triangle_count_buffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        densities_buffer = new ComputeBuffer(chunk_lattice_volume, sizeof(float));

        float[] densities = procedural_terrain.GetDensities(chunk_lattice_size, transform.localPosition);

        densities_buffer.SetData(densities);

        triangle_buffer.SetCounterValue(0);
        marchingCubesCompute.SetBuffer(0, "densities", densities_buffer);
        marchingCubesCompute.SetBuffer(0, "triangles", triangle_buffer);
        marchingCubesCompute.SetVector("grid_size", (Vector3)chunk_grid_size);

        // run compute shader
        marchingCubesCompute.Dispatch(0, 4, 4, 4);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangle_buffer, triangle_count_buffer, 0);
        int[] triangle_count_array = new int[] { 0 };
        triangle_count_buffer.GetData(triangle_count_array);
        int number_of_triangles = triangle_count_array[0];

        // Get triangle data from shader
        Triangle[] triangles = new Triangle[max_triangle_count];
        triangle_buffer.GetData(triangles, 0, 0, number_of_triangles);

        // Setup arrays for population
        int number_of_vertices = number_of_triangles * 3;

        Vector3[] vertices = new Vector3[number_of_vertices];
        int[] mesh_triangles = new int[number_of_vertices];
        Color[] vertex_colors = new Color[number_of_vertices];

        
        for (int i = 0; i < number_of_triangles; i++) // all triangles
        {
            for (int j = 0; j < 3; j++) // all 3 vertices within a triangle
            {
                mesh_triangles[i * 3 + j] = i * 3 + j;
                /* sets it equal to its index. (i have no shared verticies
                since unity doesnt have split normals (that i know of)) */

                vertices[i * 3 + j] = triangles[i][j];
            }

            Vector3 Vertex0 = vertices[i * 3 + 0];
            Vector3 Vertex1 = vertices[i * 3 + 1];
            Vector3 Vertex2 = vertices[i * 3 + 2];

            Vector3 triangle_position = (Vertex0 + Vertex1 + Vertex2) * (1f / 3f); // average position of all 3 vertices

            Vector3 triangle_world_position = transform.TransformPoint(triangle_position);
            Vector3 triangle_planet_position = transform.parent.InverseTransformPoint(triangle_world_position); // position of this triangle in Planet Space

            Vector3 normal = Vector3.Cross(Vertex1 - Vertex0, Vertex2 - Vertex0).normalized;
            float gradient = Mathf.Clamp01(1 - (Vector3.Dot(normal, triangle_planet_position.normalized) * 2 - 1)); // Gradient of terrain at this position

            float elevation = Mathf.Clamp01(Utils.Remap(triangle_planet_position.magnitude, procedural_terrain.SeaLevel, procedural_terrain.SurfaceMaxHeight, 0, 1));

            Color color = new Color(gradient, elevation, 0, 1);

            // set color of this triangle 
            vertex_colors[i * 3 + 0] = color;
            vertex_colors[i * 3 + 1] = color;
            vertex_colors[i * 3 + 2] = color;
        }

        // update mesh with triangle data
        mesh.Clear(); // clear mesh for regeneration

        mesh.vertices = vertices;
        mesh.triangles = mesh_triangles;
        mesh.colors = vertex_colors;

        mesh.RecalculateNormals();

        // release buffers from memory
        triangle_buffer.Release();
        triangle_count_buffer.Release();
        densities_buffer.Release();
    }

    struct Triangle
    {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
}
