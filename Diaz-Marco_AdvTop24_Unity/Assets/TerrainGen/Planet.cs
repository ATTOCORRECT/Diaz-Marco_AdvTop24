using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    [SerializeField]
    bool reload;

    [SerializeField]
    private Object ChunkPrefab;

    [SerializeField]
    private Object OceanSphere;

    [SerializeField]
    private Material LandMaterial;

    [SerializeField]
    private Material OceanMaterial;

    [SerializeField]
    TerrainData ProceduralTerrain;

    [SerializeField]
    private Vector3Int AreaSize;

    [SerializeField]
    private Texture3D DensityMap;

    private void Awake()
    {
        ResetDensityMap();
        SetupMaterials();
        ConfigureOcean();
        RemoveAndAddChunks();
    }

    private void Start()
    {
        // ProceduralTerrain = new TerrainData(Random.Range(0,1000000) + Random.Range(0, 1000000) * 1000000 + Random.Range(0, 1000000) * 1000000000000);
        GeneratePlanet();
    }

    private void Update()
    {
        if (reload)
        {
            FastReload();
        }
    }

    [ContextMenu("GeneratePlanet")]
    private void GeneratePlanet()
    {
        GenerateDensityMap();
        SetupMaterials();
        ConfigureOcean();
        RemoveAndAddChunks();
    }
    private void FastReload()
    {
        ConfigureOcean();
        ReloadChunks();
    }

    private void GenerateDensityMap()
    {
        Vector3Int lattice_size = (Chunk.GetChunkSize() * AreaSize) + Vector3Int.one;

        Vector3Int corner = -(AreaSize * Chunk.GetChunkSize() / 2);

        DensityMap = new Texture3D(lattice_size.x, lattice_size.y, lattice_size.z, TextureFormat.R8, 0);

        float[] densities = ProceduralTerrain.GetDensities(lattice_size, corner);

        Utils.GetGridParts(lattice_size, out int lattice_volume);

        for (int i = 0; i < lattice_volume; i++)
        {
            Vector3Int position = Utils.GridPosition(i, lattice_size);

            float density = Mathf.Clamp01((densities[i] + 1) / 2f);

            Color color = new Color(density, 0, 0);

            DensityMap.SetPixel(position.x, position.y, position.z, color);
        }

        DensityMap.Apply();

        OceanMaterial.SetTexture("_Density_Map", DensityMap);
    }

    private void ResetDensityMap()
    {
        Vector3Int lattice_size = (Chunk.GetChunkSize() * AreaSize) + Vector3Int.one;
        DensityMap = new Texture3D(lattice_size.x, lattice_size.y, lattice_size.z, TextureFormat.R8, 0);
        OceanMaterial.SetTexture("_Density_Map", DensityMap);
    }

    private void SetupMaterials()
    {
        OceanSphere.GetComponent<MeshRenderer>().material = OceanMaterial;
    }

    private void ConfigureOcean()
    {
        float scale = ProceduralTerrain.SeaLevel;
        OceanSphere.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
        OceanMaterial.SetFloat("_Planet_Size", AreaSize.x * Chunk.GetChunkSize().x);
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

        Utils.GetGridParts(AreaSize, out int area_volume);

        for (int i = 0; i < area_volume; i++)
        {
            Vector3 position = Utils.GridPosition(i, AreaSize) * Chunk.GetChunkSize();
            position -= (Vector3)(AreaSize * Chunk.GetChunkSize()) / 2f; // center chunks

            Object chunk = Instantiate(ChunkPrefab, position, Quaternion.identity, transform); // add chunks
            Chunk chunk_script = chunk.GetComponent<Chunk>();

            chunk_script.SetChunkProceduralTerrain(ref ProceduralTerrain);
            chunk_script.SetChunkMaterial(LandMaterial);
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
            //make sure not to delete the ocean
            if (child != OceanSphere) DestroyImmediate(child.gameObject);
        }
    }
}
