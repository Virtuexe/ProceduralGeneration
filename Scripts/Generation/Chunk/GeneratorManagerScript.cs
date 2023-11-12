using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Switch;
using static ChunkScript;

[RequireComponent(typeof(MeshScript),typeof(ChunkScript),typeof(RoomScript))]
public class GeneratorManagerScript : MonoBehaviour
{
    public GameObject player;

    public List<GenerationChunk> chunks = new List<GenerationChunk>();

    public float tileHeight;
    public float tileWidth;

    public float wallThickness;

    public int chunkTilesHeight;
    public int chunkTilesWidth;

    public int seed;
    private void Awake()
    {

    }
    private void Update()
    {
        Vector3 playerDistance = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z);
        Vector3 cellSize = new Vector3(tileWidth*chunkTilesWidth, tileHeight * chunkTilesHeight, tileWidth * chunkTilesWidth);
        Vector3Int playerChunk = new Vector3Int((int)Math.Floor(playerDistance.x / cellSize.x), (int)Math.Floor(playerDistance.y / cellSize.y), (int)Math.Floor(playerDistance.z / cellSize.z));

        var chunkScript = GetComponent<ChunkScript>();
        chunkScript.manager = this;
        for (int y = playerChunk.y - 0; y <= playerChunk.y + 0; y++)
        {
            for (int z = playerChunk.z - 0; z <= playerChunk.z + 0; z++)
            {
                for (int x = playerChunk.x - 0; x <= playerChunk.x + 0; x++)
                {
                    if (!chunks.Any(v => v.coordinates == new Vector3Int(x, y, z)))
                    {
                        var newChunk = new GenerationChunk(new Vector3Int(x, y, z), null);
                        chunks.Add(newChunk);
                        StartCoroutine(chunkScript.GenerateChunk(newChunk));
                    }

                }
            }
        }
    }
}
public struct Position
{

    public Position(Vector3Int value) { Value = value;
        Vector3Int reverse = new Vector3Int(1, 1, 1);
        ValueX = new Vector3Int(0, 0, 0); ValueY = new Vector3Int(0, 0, 0); ValueZ = new Vector3Int(0, 0, 0);
        if (Value.x != 0) { reverse = new Vector3Int(0, 1, 1); ValueX = new Vector3Int(0, 0, 1); ValueY = new Vector3Int(0, 1, 0); ValueZ = new Vector3Int(1, 0, 0); }
        else if (Value.y != 0) { reverse = new Vector3Int(1, 0, 1); ValueX = new Vector3Int(1, 0, 0); ValueY = new Vector3Int(0, 0, 1); ValueZ = new Vector3Int(0, 1, 0); }
        else if (Value.z != 0) { reverse = new Vector3Int(1, 1, 0); ValueX = new Vector3Int(0, 1, 0); ValueY = new Vector3Int(1, 0, 0); ValueZ = new Vector3Int(0, 0, 1); }
        ValueReverse = reverse;
    }
    public Vector3Int Value { get; private set; }
    public Vector3Int ValueReverse;
    public Vector3Int ValueX;
    public Vector3Int ValueY;
    public Vector3Int ValueZ;
    public static Position Zero { get { return new Position(new Vector3Int(0,0,0)); } }
    public static Position Top { get { return new Position(new Vector3Int(0, 1, 0)); } }
    public static Position Buttom { get { return new Position(new Vector3Int(0, -1, 0)); } }
    public static Position Front { get { return new Position(new Vector3Int(0, 0, 1)); } }
    public static Position Back { get { return new Position(new Vector3Int(0, 0, -1)); } }
    public static Position Right { get { return new Position(new Vector3Int(1, 0, 0)); } }
    public static Position Left { get { return new Position(new Vector3Int(-1, 0, 0)); } }

    
}