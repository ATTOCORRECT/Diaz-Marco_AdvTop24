using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain
{
    private int seed;
    private Vector3 seed_position;

    public ProceduralTerrain(int seed)
    {
        this.seed = seed;
        seed_position = 
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

    // Terrain Gen Methods
    private Vector3 GenerateSeedPosition(int seed)
    {
        // https://www.desmos.com/calculator/0dyvv5iyxr
        return Vector3.zero;
    }



    public float GetDensity(Vector3Int position)
    {
        float density = 0;
        //density += Noise3D(position, 40, 2) / 8;
        //density += SurfaceNoise(position, 16, 40, 4);
        density += SphereSurfaceNoise(position, 1, 28, 4, 4);
        //if (position.y < 1) density += 1;
        //density -= position.y / 16f;
        return density;
    }

    private float SphereSurfaceNoise(Vector3 position, float min_height, float max_height, float noise_scale, int octaves)
    {
        float noise = Noise3D((position).normalized + seed_position, noise_scale, octaves);
        float t = (noise + 1) / 2f;
        float surface_height = Mathf.Lerp(min_height, max_height, t);// height_map.Evaluate(t));

        float radius = (position).magnitude;

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

    private float PerlinNoise3D(Vector3 position)
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

    private float perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }
}
