using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class Chunk : MonoBehaviour
{
    public bool reload;

    public AnimationCurve height_map;

    // Chunk Variables
    Vector3Int chunk_grid_size = new Vector3Int(32, 32, 32); // dimensions of a chunk in meters
    int chunk_lattice_row, chunk_lattice_slice, chunk_lattice_volume; // shorthands for components of the grid

    Vector3Int chunk_lattice_size; // the lattice of the chunk grid (think fence posts vs fences)
    int chunk_grid_row, chunk_grid_slice, chunk_grid_volume;

    List<List<ArrayList>> cells;

    float[] densities;
    // -----

    // Tables
    readonly int[][] TRIANGULATION_TABLE = {
    new int[]{-1},
    new int[]{ 0, 8, 3, -1 },
    new int[]{ 0, 1, 9, -1 },
    new int[]{ 1, 8, 3, 9, 8, 1, -1 },
    new int[]{ 1, 2, 10, -1 },
    new int[]{ 0, 8, 3, 1, 2, 10, -1 },
    new int[]{ 9, 2, 10, 0, 2, 9, -1 },
    new int[]{ 2, 8, 3, 2, 10, 8, 10, 9, 8, -1 },
    new int[]{ 3, 11, 2, -1 },
    new int[]{ 0, 11, 2, 8, 11, 0, -1 },
    new int[]{ 1, 9, 0, 2, 3, 11, -1 },
    new int[]{ 1, 11, 2, 1, 9, 11, 9, 8, 11, -1 },
    new int[]{ 3, 10, 1, 11, 10, 3, -1 },
    new int[]{ 0, 10, 1, 0, 8, 10, 8, 11, 10, -1 },
    new int[]{ 3, 9, 0, 3, 11, 9, 11, 10, 9, -1 },
    new int[]{ 9, 8, 10, 10, 8, 11, -1 },
    new int[]{ 4, 7, 8, -1 },
    new int[]{ 4, 3, 0, 7, 3, 4, -1 },
    new int[]{ 0, 1, 9, 8, 4, 7, -1 },
    new int[]{ 4, 1, 9, 4, 7, 1, 7, 3, 1, -1 },
    new int[]{ 1, 2, 10, 8, 4, 7, -1 },
    new int[]{ 3, 4, 7, 3, 0, 4, 1, 2, 10, -1 },
    new int[]{ 9, 2, 10, 9, 0, 2, 8, 4, 7, -1 },
    new int[]{ 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1 },
    new int[]{ 8, 4, 7, 3, 11, 2, -1 },
    new int[]{ 11, 4, 7, 11, 2, 4, 2, 0, 4, -1 },
    new int[]{ 9, 0, 1, 8, 4, 7, 2, 3, 11, -1 },
    new int[]{ 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1 },
    new int[]{ 3, 10, 1, 3, 11, 10, 7, 8, 4, -1 },
    new int[]{ 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1 },
    new int[]{ 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1 },
    new int[]{ 4, 7, 11, 4, 11, 9, 9, 11, 10, -1 },
    new int[]{ 9, 5, 4, -1 },
    new int[]{ 9, 5, 4, 0, 8, 3, -1 },
    new int[]{ 0, 5, 4, 1, 5, 0, -1 },
    new int[]{ 8, 5, 4, 8, 3, 5, 3, 1, 5, -1 },
    new int[]{ 1, 2, 10, 9, 5, 4, -1 },
    new int[]{ 3, 0, 8, 1, 2, 10, 4, 9, 5, -1 },
    new int[]{ 5, 2, 10, 5, 4, 2, 4, 0, 2, -1 },
    new int[]{ 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1 },
    new int[]{ 9, 5, 4, 2, 3, 11, -1 },
    new int[]{ 0, 11, 2, 0, 8, 11, 4, 9, 5, -1 },
    new int[]{ 0, 5, 4, 0, 1, 5, 2, 3, 11, -1 },
    new int[]{ 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1 },
    new int[]{ 10, 3, 11, 10, 1, 3, 9, 5, 4, -1 },
    new int[]{ 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1 },
    new int[]{ 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1 },
    new int[]{ 5, 4, 8, 5, 8, 10, 10, 8, 11, -1 },
    new int[]{ 9, 7, 8, 5, 7, 9, -1 },
    new int[]{ 9, 3, 0, 9, 5, 3, 5, 7, 3, -1 },
    new int[]{ 0, 7, 8, 0, 1, 7, 1, 5, 7, -1 },
    new int[]{ 1, 5, 3, 3, 5, 7, -1 },
    new int[]{ 9, 7, 8, 9, 5, 7, 10, 1, 2, -1 },
    new int[]{ 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1 },
    new int[]{ 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1 },
    new int[]{ 2, 10, 5, 2, 5, 3, 3, 5, 7, -1 },
    new int[]{ 7, 9, 5, 7, 8, 9, 3, 11, 2, -1 },
    new int[]{ 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1 },
    new int[]{ 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1 },
    new int[]{ 11, 2, 1, 11, 1, 7, 7, 1, 5, -1 },
    new int[]{ 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1 },
    new int[]{ 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 },
    new int[]{ 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 },
    new int[]{ 11, 10, 5, 7, 11, 5, -1 },
    new int[]{ 10, 6, 5, -1 },
    new int[]{ 0, 8, 3, 5, 10, 6, -1 },
    new int[]{ 9, 0, 1, 5, 10, 6, -1 },
    new int[]{ 1, 8, 3, 1, 9, 8, 5, 10, 6, -1 },
    new int[]{ 1, 6, 5, 2, 6, 1, -1 },
    new int[]{ 1, 6, 5, 1, 2, 6, 3, 0, 8, -1 },
    new int[]{ 9, 6, 5, 9, 0, 6, 0, 2, 6, -1 },
    new int[]{ 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1 },
    new int[]{ 2, 3, 11, 10, 6, 5, -1 },
    new int[]{ 11, 0, 8, 11, 2, 0, 10, 6, 5, -1 },
    new int[]{ 0, 1, 9, 2, 3, 11, 5, 10, 6, -1 },
    new int[]{ 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1 },
    new int[]{ 6, 3, 11, 6, 5, 3, 5, 1, 3, -1 },
    new int[]{ 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1 },
    new int[]{ 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1 },
    new int[]{ 6, 5, 9, 6, 9, 11, 11, 9, 8, -1 },
    new int[]{ 5, 10, 6, 4, 7, 8, -1 },
    new int[]{ 4, 3, 0, 4, 7, 3, 6, 5, 10, -1 },
    new int[]{ 1, 9, 0, 5, 10, 6, 8, 4, 7, -1 },
    new int[]{ 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1 },
    new int[]{ 6, 1, 2, 6, 5, 1, 4, 7, 8, -1 },
    new int[]{ 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1 },
    new int[]{ 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1 },
    new int[]{ 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 },
    new int[]{ 3, 11, 2, 7, 8, 4, 10, 6, 5, -1 },
    new int[]{ 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1 },
    new int[]{ 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1 },
    new int[]{ 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 },
    new int[]{ 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1 },
    new int[]{ 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 },
    new int[]{ 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 },
    new int[]{ 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1 },
    new int[]{ 10, 4, 9, 6, 4, 10, -1 },
    new int[]{ 4, 10, 6, 4, 9, 10, 0, 8, 3, -1 },
    new int[]{ 10, 0, 1, 10, 6, 0, 6, 4, 0, -1 },
    new int[]{ 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1 },
    new int[]{ 1, 4, 9, 1, 2, 4, 2, 6, 4, -1 },
    new int[]{ 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1 },
    new int[]{ 0, 2, 4, 4, 2, 6, -1 },
    new int[]{ 8, 3, 2, 8, 2, 4, 4, 2, 6, -1 },
    new int[]{ 10, 4, 9, 10, 6, 4, 11, 2, 3, -1 },
    new int[]{ 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1 },
    new int[]{ 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1 },
    new int[]{ 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 },
    new int[]{ 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1 },
    new int[]{ 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 },
    new int[]{ 3, 11, 6, 3, 6, 0, 0, 6, 4, -1 },
    new int[]{ 6, 4, 8, 11, 6, 8, -1 },
    new int[]{ 7, 10, 6, 7, 8, 10, 8, 9, 10, -1 },
    new int[]{ 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1 },
    new int[]{ 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1 },
    new int[]{ 10, 6, 7, 10, 7, 1, 1, 7, 3, -1 },
    new int[]{ 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1 },
    new int[]{ 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 },
    new int[]{ 7, 8, 0, 7, 0, 6, 6, 0, 2, -1 },
    new int[]{ 7, 3, 2, 6, 7, 2, -1 },
    new int[]{ 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1 },
    new int[]{ 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 },
    new int[]{ 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 },
    new int[]{ 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1 },
    new int[]{ 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 },
    new int[]{ 0, 9, 1, 11, 6, 7, -1 },
    new int[]{ 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1 },
    new int[]{ 7, 11, 6, -1 },
    new int[]{ 7, 6, 11, -1 },
    new int[]{ 3, 0, 8, 11, 7, 6, -1 },
    new int[]{ 0, 1, 9, 11, 7, 6, -1 },
    new int[]{ 8, 1, 9, 8, 3, 1, 11, 7, 6, -1 },
    new int[]{ 10, 1, 2, 6, 11, 7, -1 },
    new int[]{ 1, 2, 10, 3, 0, 8, 6, 11, 7, -1 },
    new int[]{ 2, 9, 0, 2, 10, 9, 6, 11, 7, -1 },
    new int[]{ 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1 },
    new int[]{ 7, 2, 3, 6, 2, 7, -1 },
    new int[]{ 7, 0, 8, 7, 6, 0, 6, 2, 0, -1 },
    new int[]{ 2, 7, 6, 2, 3, 7, 0, 1, 9, -1 },
    new int[]{ 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1 },
    new int[]{ 10, 7, 6, 10, 1, 7, 1, 3, 7, -1 },
    new int[]{ 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1 },
    new int[]{ 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1 },
    new int[]{ 7, 6, 10, 7, 10, 8, 8, 10, 9, -1 },
    new int[]{ 6, 8, 4, 11, 8, 6, -1 },
    new int[]{ 3, 6, 11, 3, 0, 6, 0, 4, 6, -1 },
    new int[]{ 8, 6, 11, 8, 4, 6, 9, 0, 1, -1 },
    new int[]{ 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1 },
    new int[]{ 6, 8, 4, 6, 11, 8, 2, 10, 1, -1 },
    new int[]{ 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1 },
    new int[]{ 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1 },
    new int[]{ 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 },
    new int[]{ 8, 2, 3, 8, 4, 2, 4, 6, 2, -1 },
    new int[]{ 0, 4, 2, 4, 6, 2, -1 },
    new int[]{ 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1 },
    new int[]{ 1, 9, 4, 1, 4, 2, 2, 4, 6, -1 },
    new int[]{ 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1 },
    new int[]{ 10, 1, 0, 10, 0, 6, 6, 0, 4, -1 },
    new int[]{ 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 },
    new int[]{ 10, 9, 4, 6, 10, 4, -1 },
    new int[]{ 4, 9, 5, 7, 6, 11, -1 },
    new int[]{ 0, 8, 3, 4, 9, 5, 11, 7, 6, -1 },
    new int[]{ 5, 0, 1, 5, 4, 0, 7, 6, 11, -1 },
    new int[]{ 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1 },
    new int[]{ 9, 5, 4, 10, 1, 2, 7, 6, 11, -1 },
    new int[]{ 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1 },
    new int[]{ 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1 },
    new int[]{ 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 },
    new int[]{ 7, 2, 3, 7, 6, 2, 5, 4, 9, -1 },
    new int[]{ 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1 },
    new int[]{ 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1 },
    new int[]{ 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 },
    new int[]{ 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1 },
    new int[]{ 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 },
    new int[]{ 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 },
    new int[]{ 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1 },
    new int[]{ 6, 9, 5, 6, 11, 9, 11, 8, 9, -1 },
    new int[]{ 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1 },
    new int[]{ 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1 },
    new int[]{ 6, 11, 3, 6, 3, 5, 5, 3, 1, -1 },
    new int[]{ 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1 },
    new int[]{ 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 },
    new int[]{ 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 },
    new int[]{ 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1 },
    new int[]{ 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1 },
    new int[]{ 9, 5, 6, 9, 6, 0, 0, 6, 2, -1 },
    new int[]{ 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 },
    new int[]{ 1, 5, 6, 2, 1, 6, -1 },
    new int[]{ 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 },
    new int[]{ 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1 },
    new int[]{ 0, 3, 8, 5, 6, 10, -1 },
    new int[]{ 10, 5, 6, -1 },
    new int[]{ 11, 5, 10, 7, 5, 11, -1 },
    new int[]{ 11, 5, 10, 11, 7, 5, 8, 3, 0, -1 },
    new int[]{ 5, 11, 7, 5, 10, 11, 1, 9, 0, -1 },
    new int[]{ 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1 },
    new int[]{ 11, 1, 2, 11, 7, 1, 7, 5, 1, -1 },
    new int[]{ 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1 },
    new int[]{ 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1 },
    new int[]{ 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 },
    new int[]{ 2, 5, 10, 2, 3, 5, 3, 7, 5, -1 },
    new int[]{ 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1 },
    new int[]{ 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1 },
    new int[]{ 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 },
    new int[]{ 1, 3, 5, 3, 7, 5, -1 },
    new int[]{ 0, 8, 7, 0, 7, 1, 1, 7, 5, -1 },
    new int[]{ 9, 0, 3, 9, 3, 5, 5, 3, 7, -1 },
    new int[]{ 9, 8, 7, 5, 9, 7, -1 },
    new int[]{ 5, 8, 4, 5, 10, 8, 10, 11, 8, -1 },
    new int[]{ 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1 },
    new int[]{ 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1 },
    new int[]{ 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 },
    new int[]{ 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1 },
    new int[]{ 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 },
    new int[]{ 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 },
    new int[]{ 9, 4, 5, 2, 11, 3, -1 },
    new int[]{ 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1 },
    new int[]{ 5, 10, 2, 5, 2, 4, 4, 2, 0, -1 },
    new int[]{ 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 },
    new int[]{ 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1 },
    new int[]{ 8, 4, 5, 8, 5, 3, 3, 5, 1, -1 },
    new int[]{ 0, 4, 5, 1, 0, 5, -1 },
    new int[]{ 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1 },
    new int[]{ 9, 4, 5, -1 },
    new int[]{ 4, 11, 7, 4, 9, 11, 9, 10, 11, -1 },
    new int[]{ 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1 },
    new int[]{ 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1 },
    new int[]{ 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 },
    new int[]{ 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1 },
    new int[]{ 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 },
    new int[]{ 11, 7, 4, 11, 4, 2, 2, 4, 0, -1 },
    new int[]{ 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1 },
    new int[]{ 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1 },
    new int[]{ 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 },
    new int[]{ 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 },
    new int[]{ 1, 10, 2, 8, 7, 4, -1 },
    new int[]{ 4, 9, 1, 4, 1, 7, 7, 1, 3, -1 },
    new int[]{ 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1 },
    new int[]{ 4, 0, 3, 7, 4, 3, -1 },
    new int[]{ 4, 8, 7, -1 },
    new int[]{ 9, 10, 8, 10, 11, 8, -1 },
    new int[]{ 3, 0, 9, 3, 9, 11, 11, 9, 10, -1 },
    new int[]{ 0, 1, 10, 0, 10, 8, 8, 10, 11, -1 },
    new int[]{ 3, 1, 10, 11, 3, 10, -1 },
    new int[]{ 1, 2, 11, 1, 11, 9, 9, 11, 8, -1 },
    new int[]{ 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1 },
    new int[]{ 0, 2, 11, 8, 0, 11, -1 },
    new int[]{ 3, 2, 11, -1 },
    new int[]{ 2, 3, 8, 2, 8, 10, 10, 8, 9, -1 },
    new int[]{ 9, 10, 2, 0, 9, 2, -1 },
    new int[]{ 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1 },
    new int[]{ 1, 10, 2, -1 },
    new int[]{ 1, 3, 8, 9, 1, 8, -1 },
    new int[]{ 0, 9, 1, -1 },
    new int[]{ 0, 3, 8, -1 },
    new int[]{-1}
    }; // surely theres a better way to do this

    readonly int[][] EDGE_TABLE = {
    new int[]{ 0, 1},
    new int[]{ 1, 2},
    new int[]{ 2, 3},
    new int[]{ 3, 0},
    new int[]{ 4, 5},
    new int[]{ 5, 6},
    new int[]{ 6, 7},
    new int[]{ 7, 4},
    new int[]{ 0, 4},
    new int[]{ 1, 5},
    new int[]{ 2, 6},
    new int[]{ 3, 7}
    };
    // ------
    void Awake()
    {
        chunk_lattice_size = chunk_grid_size + Vector3Int.one;

        GetGridParts(chunk_lattice_size, out chunk_lattice_row, out chunk_lattice_slice, out chunk_lattice_volume);
        GetGridParts(chunk_grid_size   , out chunk_grid_row   , out chunk_grid_slice   , out chunk_grid_volume   );

        GenerateChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (reload)
        {
            //reload = false;
            GenerateChunk();
        }

        DrawCube(transform.position, chunk_grid_size, Color.white);
    }

    public void GenerateChunk()
    {
        GenerateDensities();

        GenerateCellData();

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < chunk_grid_volume; i++)
        {
            // get triangulation index for this cell
            int triangulation_index = 0;
            for (int j = 0; j < 8; j++)
            {
                float cell_vertex_density = (float)cells[i][j][1];
                if (cell_vertex_density < 0) triangulation_index |= 1 << j;
            }

            // generate triangulation for this cell
            List<Vector3> triangle_vertices = new List<Vector3>();
            int[] triangulation = TRIANGULATION_TABLE[triangulation_index];
            for (int j = 0; j < triangulation.Length - 1; j++)
            {
                int edge_index = triangulation[j];

                int vertex_index_A = EDGE_TABLE[edge_index][0];
                int vertex_index_B = EDGE_TABLE[edge_index][1];

                ArrayList vertex_data_A = cells[i][vertex_index_A];
                ArrayList vertex_data_B = cells[i][vertex_index_B];

                float bias = (0 - (float)vertex_data_A[1]) / ((float)vertex_data_B[1] - (float)vertex_data_A[1]);

                triangle_vertices.Add(Vector3.Lerp((Vector3Int)cells[i][vertex_index_A][0], (Vector3Int)cells[i][vertex_index_B][0], bias));
            }

            AppendTriangles(triangle_vertices, vertices, normals, triangles);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        //mesh_renderer.material = mesh_material;
    }

    private void GenerateDensities()
    {
        densities = new float[chunk_lattice_volume];
        for (int i = 0; i < chunk_lattice_volume; i++)
        {
            Vector3Int point_position = GridPosition(i, chunk_lattice_size) + Vector3Int.RoundToInt(transform.localPosition);

            densities[i] = GetDensity(point_position);
        }
    }

    private void GenerateCellData()
    {
        cells = new List<List<ArrayList>>();

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
    }

    private void AppendTriangles(List<Vector3> triangle_vertices, List<Vector3> vertices, List<Vector3> normals, List<int> triangles)
    {
        for (int i = 0; i < triangle_vertices.Count / 3; i++)
        {
            Vector3 v1 = triangle_vertices[i * 3    ];
            Vector3 v3 = triangle_vertices[i * 3 + 1];
            Vector3 v2 = triangle_vertices[i * 3 + 2];

            AppendTriangle(v1, v2, v3, vertices, normals, triangles);
        }
    }

    private void AppendTriangle(Vector3 v1, Vector3 v2, Vector3 v3, List<Vector3> vertices, List<Vector3> normals, List<int> triangles) //change for acutal mesh
    {
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(triangles.Count);
        triangles.Add(triangles.Count);
        triangles.Add(triangles.Count);

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        //Debug.DrawLine(v1, v2, Color.white, float.PositiveInfinity);
        //Debug.DrawLine(v2, v3, Color.white, float.PositiveInfinity);
        //Debug.DrawLine(v3, v1, Color.white, float.PositiveInfinity);
    }

    private Vector3Int GridPosition(int index, Vector3Int grid_size) // from an index get a position in a grid
    {
        int x =  index                                % grid_size.x;
        int y = (index /  grid_size.x)                % grid_size.y;
        int z = (index / (grid_size.x * grid_size.y)) % grid_size.z;
        return new Vector3Int(x, y, z);
    }

    private void GetGridParts(Vector3Int grid_size, out int grid_row, out int grid_slice, out int grid_volume) // split a grid into its components for easier math
    {
        grid_row    = grid_size.x;
        grid_slice  = grid_size.x * grid_size.y;
        grid_volume = grid_size.x * grid_size.y * grid_size.z;
    }

    private void DrawPoint(Vector3 position, Color color) // draw a point at a position // this is debug stuff to be removed
    {
        Debug.DrawRay(position - Vector3.right   / 10, Vector3.right   / 5, color);
        Debug.DrawRay(position - Vector3.up      / 10, Vector3.up      / 5, color);
        Debug.DrawRay(position - Vector3.forward / 10, Vector3.forward / 5, color);
    }

    private void DrawUnitCube(Vector3 position, float scale, Color color) // draw a 1x1 cube, scale makes it bigger and smaller from is center // this is debug stuff to be removed
    {
        DrawCube(position + Vector3.one * ((1 - scale) / 2f), Vector3.one - Vector3.one * (1 - scale), color);
    }

    private void DrawCube(Vector3 position, Vector3 size, Color color)
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

    private float GetDensity(Vector3Int position)
    {
        float density = 0;
        density += Noise3D(position, 40, 2) / 2;
        //density += SurfaceNoise(position, 16, 40, 4);
        density += SphereSurfaceNoise(position, 6, 14, 4, 4);
        //if (position.y < 1) density += 1;
        //density -= position.y / 16f;
        return density;
    }

    private float SphereSurfaceNoise(Vector3 position, float min_height, float max_height, float noise_scale, int octaves)
    {
        Vector3 sphere_position = chunk_grid_size/2 + transform.localPosition;

        float noise = Noise3D((position - sphere_position).normalized + sphere_position, noise_scale, octaves);
        float t = (noise + 1) / 2f;
        float surface_height = Mathf.Lerp(min_height, max_height, height_map.Evaluate(t));

        float radius = (position - sphere_position).magnitude;

        float density = (radius - surface_height) / (max_height - min_height) * 2;

        return density;

    }

    private float SurfaceNoise(Vector3 position, float max_height, float noise_scale, int octaves)
    {
        float noise = Noise2D(new Vector2(position.x, position.z), noise_scale, octaves);

        float surfaceHeight = Mathf.Lerp(0, max_height, (noise + 1) / 2f);
 
        return (position.y - surfaceHeight) / ((max_height / 2f) * 1 / 0.5f);
    }

    private float Noise2D(Vector2 position, float noise_scale, int octaves)
    {
        position *= 1 / noise_scale;

        float noise = 0;
        for (int i = 0; i < octaves; i++)
        {
            noise += PerlinNoise2D(position * Mathf.Pow(2, i)) / Mathf.Pow(2, i);
        }

        return Mathf.Clamp(noise, -1, 1);
    }

    private float PerlinNoise2D(Vector2 position)
    {
        return Mathf.PerlinNoise(position.x, position.y) * 2 - 1;
    }

    private float Noise3D(Vector3 position, float noise_scale, int octaves)
    {

        position *= 1 / noise_scale;
        
        float noise = 0;
        for (int i = 0; i < octaves; i++)
        {
            noise += PerlinNoise3D(position * Mathf.Pow(2, i)) / Mathf.Pow(2, i);
        }

        return Mathf.Clamp(noise, -1, 1);
    }

    private static float PerlinNoise3D(Vector3 position)
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

        return ((xy * xz * yz * yx * zx * zy) * 2 - 1);
    }

    private static float perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }
}
