using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3Int GridPosition(int index, Vector3Int grid_size) // from an index get a position in a grid
    {
        int x =  index                                % grid_size.x;
        int y = (index /  grid_size.x)                % grid_size.y;
        int z = (index / (grid_size.x * grid_size.y)) % grid_size.z;
        return new Vector3Int(x, y, z);
    }

    public static void GetGridParts(Vector3Int grid_size, out int grid_row, out int grid_slice, out int grid_volume) // split a grid into its components for easier math
    {
        grid_row    = grid_size.x;
        grid_slice  = grid_size.x * grid_size.y;
        grid_volume = grid_size.x * grid_size.y * grid_size.z;
    }

    public static void GetGridParts(Vector3Int grid_size, out int grid_volume) // split a grid into its components for easier math (overload for only volume)
    {
        grid_volume = grid_size.x * grid_size.y * grid_size.z;
    }

    // Debug Methods (I didnt know gizmos existed)
    public static void DrawPoint(Vector3 position, Color color) // draw a point at a position // this is debug stuff to be removed
    {
        Debug.DrawRay(position - Vector3.right / 10, Vector3.right / 5, color);
        Debug.DrawRay(position - Vector3.up / 10, Vector3.up / 5, color);
        Debug.DrawRay(position - Vector3.forward / 10, Vector3.forward / 5, color);
    }

    public static void DrawUnitCube(Vector3 position, float scale, Color color) // draw a 1x1 cube, scale makes it bigger and smaller from is center // this is debug stuff to be removed
    {
        DrawCube(position + Vector3.one * ((1 - scale) / 2f), Vector3.one - Vector3.one * (1 - scale), color);
    }

    public static void DrawCube(Vector3 position, Vector3 size, Color color)
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
    // ----------

}
