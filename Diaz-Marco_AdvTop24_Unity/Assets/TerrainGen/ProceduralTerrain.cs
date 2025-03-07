using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class TerrainData
{
    [SerializeField]
    private ComputeShader densityCompute;
    private ComputeBuffer density_buffer;

    //[SerializeField] // /----\/----\/----\  must be 18 digits, 3 groups of 6.
    //private long seed = 000000000000000000;

    [Header("Noise Settings")]

    [SerializeField]
    private AnimationCurve PeaksAndValleys;

    [SerializeField]
    private AnimationCurve Continentalness;

    [SerializeField]
    private AnimationCurve Erosion;

    [SerializeField]
    public float SurfaceMinHeight;

    [SerializeField]
    public float SeaLevel;

    [SerializeField]
    public float SurfaceMaxHeight;

    [SerializeField]
    private float Scale;

    private Vector3 seed_position;

    public TerrainData(long seed)
    {
        SetSeed(seed);
    }

    // Terrain Gen Methods
    public void UpdateSeed()
    {
        //GenerateSeedPosition(seed);
    }
    public void SetSeed(long seed)
    {
        //this.seed = seed;
        GenerateSeedPosition(seed);
    }
    
    public float[] GetDensities(Vector3Int lattice_size, Vector3 position)
    {
        float[] peaks_and_valleys_noise = generateNoise(lattice_size, position, 2, 2 * Scale, 2, 0.5f);

        float[] continentalness_noise = generateNoise(lattice_size, position, 4, 1 * Scale, 2, 0.5f);

        float[] erosion_noise = generateNoise(lattice_size, position, 2, 1 * Scale, 2, 0.5f);

        float[] densities = new float[lattice_size.x * lattice_size.y * lattice_size.z];

        for (int i = 0; i < (lattice_size.x * lattice_size.y * lattice_size.z); i++)
        {
            Vector3Int cell_position = Utils.GridPosition(i, lattice_size);

            float radius = (cell_position + position).magnitude;

            float t = 0;

            float peaks_and_valleys_height = PeaksAndValleys.Evaluate((peaks_and_valleys_noise[i] + 1) / 2f);

            float continentalness_height = Continentalness.Evaluate((continentalness_noise[i] + 1) / 2f);

            float erosion_factor = Erosion.Evaluate((erosion_noise[i] + 1) / 2f);


            t += ((peaks_and_valleys_height * 2 - 1) * erosion_factor + 1) / 2 * 0.5f;

            t += ((continentalness_height * 2 - 1) * Mathf.Lerp(erosion_factor, 1, 0.1f) + 1) / 2 * 0.5f;

            float surface_height = Mathf.Lerp(SurfaceMinHeight, SurfaceMaxHeight, t);

            densities[i] = (radius - surface_height) / (SurfaceMinHeight - SurfaceMaxHeight) * 2;
        }

        return densities;
    }

    public float[] generateNoise(Vector3Int lattice_size, Vector3 position, int octaves, float scale, float lacunarity, float persistence)
    {
        // setup parameters and copy to compute buffer
        float[] noise = new float[lattice_size.x * lattice_size.y * lattice_size.z];
        density_buffer = new ComputeBuffer(noise.Length, sizeof(float));
        density_buffer.SetData(noise);
        densityCompute.SetBuffer(0, "height_map", density_buffer);
        densityCompute.SetVector("local_position", position);
        densityCompute.SetVector("lattice_size", (Vector3)lattice_size);
        densityCompute.SetInt("octaves", octaves);
        densityCompute.SetFloat("scale", scale);
        densityCompute.SetFloat("lacunarity", lacunarity);
        densityCompute.SetFloat("persistence", persistence);
        //densityCompute.SetFloat("squashing_factor", squashingFactor);
        //densityCompute.SetFloat("mid_height", VolumetricMidHeight);
        densityCompute.SetFloat("min_height", SurfaceMinHeight);
        densityCompute.SetFloat("max_height", SurfaceMaxHeight);
        //densityCompute.SetFloat("influence", SurfaceInfluence);

        // get the dispatch size (how many instances ill dispatch simultaniously)
        Vector3Int dispatch_size = Vector3Int.zero;

        dispatch_size.x = (int)Mathf.Ceil(lattice_size.x / 8f); // each have a size of 8x8x8 so i divide my space by 8 rounded up to get the number in that dimension
        dispatch_size.y = (int)Mathf.Ceil(lattice_size.y / 8f);
        dispatch_size.z = (int)Mathf.Ceil(lattice_size.z / 8f);

        // run compute shader
        densityCompute.Dispatch(0, dispatch_size.x, dispatch_size.y, dispatch_size.z);

        // copy calculated densities back to densities array
        density_buffer.GetData(noise);

        // release buffer from memory
        density_buffer.Release();

        return noise;
    }



    private void GenerateSeedPosition(long seed) // not currently in use
    {
        for (int i = 0; i < 3; i++)
        {
            long value = (seed / (long)Mathf.Pow(10, 6 * i)) % (long)Mathf.Pow(10, 6); // https://www.desmos.com/calculator/0dyvv5iyxr
            seed_position[i] = (int)value;
        }
        seed_position += Vector3.one * 111.111f;
    }
}
