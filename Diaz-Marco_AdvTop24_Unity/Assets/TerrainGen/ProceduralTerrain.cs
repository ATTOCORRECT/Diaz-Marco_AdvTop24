using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TerrainData
{
    [SerializeField]
    private ComputeShader densityCompute;

    private ComputeBuffer density_buffer;
    [SerializeField] // /----\/----\/----\  must be 18 digits, 3 groups of 6.
    private long seed = 000000000000000000;
    [SerializeField]
    private float squashingFactor;
    [SerializeField]
    private float midHeight;
    [SerializeField]
    //private AnimationCurve height_map = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 seed_position;

    public TerrainData(long seed)
    {
        SetSeed(seed);
    }

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
    
    public ComputeBuffer GetDensities(Vector3Int lattice_size, Vector3 position)
    {
        // setup parameters and copy to compute buffer
        float[] densities = new float[lattice_size.x * lattice_size.y * lattice_size.z];
        density_buffer = new ComputeBuffer(densities.Length, sizeof(float));
        density_buffer.SetData(densities);
        densityCompute.SetBuffer(0, "densities", density_buffer);
        densityCompute.SetVector("localPosition", position);
        densityCompute.SetVector("latticeSize", (Vector3)lattice_size);
        densityCompute.SetFloat("squashingFactor", squashingFactor);
        densityCompute.SetFloat("midHeight", midHeight);
        // run compute shader
        densityCompute.Dispatch(0, 5, 5, 5);

        // copy calculated densities back to densities array
        density_buffer.GetData(densities);

        return density_buffer;
    }

    public void ReleaseBuffer()
    {
        // release buffer from memory
        density_buffer.Release();
    }

    private void GenerateSeedPosition(long seed)
    {
        for (int i = 0; i < 3; i++)
        {
            long value = (seed / (long)Mathf.Pow(10, 6 * i)) % (long)Mathf.Pow(10, 6); // https://www.desmos.com/calculator/0dyvv5iyxr
            seed_position[i] = (int)value;
        }
        seed_position += Vector3.one * 111.111f;
    }

/*    private float SphereSurfaceNoise(Vector3 position, float min_height, float max_height, float noise_scale, int octaves) // noise for flat hilly terrain on the surface of a sphere
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
    }*/
}
