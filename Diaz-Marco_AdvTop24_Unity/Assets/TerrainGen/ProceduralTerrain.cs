using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProceduralTerrain
{   
    [SerializeField] // /----\/----\/----\  must be 18 digits, 3 groups of 6.
    private long seed = 000000000000000000;
    [SerializeField]
    private float min_height;
    [SerializeField]
    private float max_height;
    [SerializeField]
    private AnimationCurve height_map = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 seed_position;

    public ProceduralTerrain(long seed)
    {
        SetSeed(seed);
    }

    // Debug Methods (I didnt know gizmos existed)
    public static void DrawPoint(Vector3 position, Color color) // draw a point at a position // this is debug stuff to be removed
    {
        Debug.DrawRay(position - Vector3.right   / 10, Vector3.right   / 5, color);
        Debug.DrawRay(position - Vector3.up      / 10, Vector3.up      / 5, color);
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
    public void UpdateSeed()
    {
        GenerateSeedPosition(seed);
    }
    public void SetSeed(long seed)
    {
        this.seed = seed;
        GenerateSeedPosition(seed);
    }
    
    private void GenerateSeedPosition(long seed)
    {
        for (int i = 0; i < 3; i++)
        {
            long value = (seed / (long)Mathf.Pow(10, 6 * i)) % (long)Mathf.Pow(10, 6); // https://www.desmos.com/calculator/0dyvv5iyxr
            seed_position[i] = (int)value;
            Debug.Log(value);
        }
        seed_position += Vector3.one * 111.111f;
    }

    public float GetDensity(Vector3Int position)
    {
        float density = 0;
        //density += Noise3D(position + seed_position / 10f, 64, 2);/// 8;
        //density += SurfaceNoise(position, 16, 40, 4);
        density += SphereSurfaceNoise(position, min_height, max_height, 4, 4);
        //if (position.y < 1) density += 1;
        //density -= position.y / 16f;
        return density;
    }

    private float SphereSurfaceNoise(Vector3 position, float min_height, float max_height, float noise_scale, int octaves) // noise for flat hilly terrain on the surface of a sphere
    {
        float noise = Noise3D((position).normalized + seed_position / 10f, noise_scale, octaves);
        float t = (noise + 1) / 2f;
        float surface_height = Mathf.Lerp(min_height, max_height, height_map.Evaluate(t));

        float radius = (position).magnitude;

        float density = (radius - surface_height) / (max_height - min_height) * 2;

        return density;

    }

    private float SurfaceNoise(Vector3 position, float max_height, float noise_scale, int octaves) // noise for flat hilly terrain [deprecated]
    {
        float noise = Noise2D(new Vector2(position.x, position.z), noise_scale, octaves);
        float t = (noise + 1) / 2f;
        float surfaceHeight = Mathf.Lerp(0, max_height, t);

        return (position.y - surfaceHeight) / ((max_height / 2f) * 1 / 0.5f);
    }

    private float Noise2D(Vector2 position, float noise_scale, int octaves)
    {
        position *= 1 / noise_scale; // scale noise

        float noiseSum = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++) // add together octaves to make fractal noise
        {
            noiseSum += PerlinNoise2D(position * frequency) * amplitude;

            frequency *= 2;

            amplitude *= 0.5f;
        }

        return Mathf.Clamp(noiseSum, -1, 1); // clamp just incase, although it shouldnt go beyond these bounds regardless
    }

    private float PerlinNoise2D(Vector2 position)
    {
        return Mathf.PerlinNoise(position.x, position.y) * 2 - 1; // returns 2D perlin scaled to a range of -1 to 1
    }

    private float Noise3D(Vector3 position, float noise_scale, int octaves)
    {
        position *= 1 / noise_scale; // scale noise

        float noise = 0;
        for (int i = 0; i < octaves; i++) // add together octaves to make fractal noise
        {
            noise += PerlinNoise3D(position * Mathf.Pow(2, i + 1)) / Mathf.Pow(2, i + 1);
        }

        return Mathf.Clamp(noise, -1, 1); // clamp just incase, although it shouldnt go beyond these bounds regardless
    }

    private float PerlinNoise3D(Vector3 position)
    {
        // https://discussions.unity.com/t/3d-perlin-noise/134957

        float x = position.x;
        float y = position.y;
        float z = position.z;

        x += 10000; // prevent mirroring along x, i have no idea why it does
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
