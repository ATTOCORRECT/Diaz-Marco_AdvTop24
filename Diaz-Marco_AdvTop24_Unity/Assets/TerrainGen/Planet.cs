using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{

    [SerializeField]
    private bool reload;
    [SerializeField]
    private bool remove_and_add_chunks;
    [SerializeField]
    private Object chunk_prefab;
    [SerializeField]
    ProceduralTerrain procedural_terrain;
    [SerializeField]
    private Vector3Int area_size;
    

    void Awake()
    {
        procedural_terrain = new ProceduralTerrain(0);
        RemoveAndAddChunks();
    }

    private void Update()
    {
        if (reload)
        {
            reload = false;
            procedural_terrain.UpdateSeed();
            ReloadChunks();
        }

        if (remove_and_add_chunks)
        {
            remove_and_add_chunks = false;
            RemoveAndAddChunks();
        }
    }

    void ReloadChunks()
    {
        Chunk[] chunk_scripts = gameObject.GetComponentsInChildren<Chunk>();

        foreach(Chunk chunk_script in chunk_scripts)
        {
            //chunk_script.SetChunkProceduralTerrain(ref procedural_terrain);
            chunk_script.GenerateChunk();
        }
    }

    void RemoveAndAddChunks()
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

    public void ClearChildren() // https://stackoverflow.com/a/46359133 
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
