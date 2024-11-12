using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    [SerializeField]
    private Object chunk_prefab;

    [SerializeField]
    private Material OceanMaterial;

    [SerializeField]
    TerrainData procedural_terrain;

    [SerializeField]
    private Vector3Int area_size;

    [SerializeField]
    private Texture3D DensityMap;

    void Start()
    {
        // procedural_terrain = new TerrainData(Random.Range(0,1000000) + Random.Range(0, 1000000) * 1000000 + Random.Range(0, 1000000) * 1000000000000);
        GeneratePlanet();
    }

    private void Update()
    {

    }

    [ContextMenu("GeneratePlanet")]
    private void GeneratePlanet()
    {
        GenerateDensityMap();
        SetupMaterials();
        RemoveAndAddChunks();
    }

    private void GenerateDensityMap()
    {
        Vector3Int lattice_size = (Chunk.GetChunkSize() * area_size) + Vector3Int.one;

        Vector3Int corner = -(area_size * Chunk.GetChunkSize() / 2);

        DensityMap = new Texture3D(lattice_size.x, lattice_size.y, lattice_size.z, TextureFormat.R8, 0);

        float[] densities = procedural_terrain.GetDensities(lattice_size, corner);

        Utils.GetGridParts(lattice_size, out int lattice_volume);

        for (int i = 0; i < lattice_volume; i++)
        {
            Vector3Int position = Utils.GridPosition(i, lattice_size);

            float density = Mathf.Clamp01((densities[i] + 1) / 2f);

            Color color = new Color(density, 0, 0);

            DensityMap.SetPixel(position.x, position.y, position.z, color);
        }

        DensityMap.Apply();
    }

    private void SetupMaterials()
    {
        OceanMaterial.SetTexture("_Density_Map", DensityMap);
    }

    private void ReloadChunks()
    {
        Chunk[] chunk_scripts = gameObject.GetComponentsInChildren<Chunk>();

        foreach(Chunk chunk_script in chunk_scripts)
        {
            chunk_script.GenerateChunk();
        }
    }

    private void RemoveAndAddChunks() 
    {
        ClearChildren();

        Utils.GetGridParts(area_size, out int area_volume);

        for (int i = 0; i < area_volume; i++)
        {
            Vector3 position = Utils.GridPosition(i, area_size) * Chunk.GetChunkSize();
            position -= (Vector3)(area_size * Chunk.GetChunkSize()) / 2f; // center chunks

            Object chunk = Instantiate(chunk_prefab, position, Quaternion.identity, transform); // add chunks
            Chunk chunk_script = chunk.GetComponent<Chunk>();

            chunk_script.SetChunkProceduralTerrain(ref procedural_terrain);
            chunk_script.GenerateChunk();
        }
    }

    private void ClearChildren() // https://stackoverflow.com/a/46359133 
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
