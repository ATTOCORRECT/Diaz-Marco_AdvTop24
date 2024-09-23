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

}
