using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(GeneratorManagerScript))]
[RequireComponent(typeof(MeshScript))]
[RequireComponent(typeof(ChunkScript))]
[RequireComponent(typeof(GenerationScript))]
[RequireComponent(typeof(GameEventsScript))]
[RequireComponent(typeof(PathFindingScript))]
public class GenerationProp : MonoBehaviour
{
    public Layers layers;
    public Vector3Int test;

    public ChunkArray chunkArray;

    public Vector3 tileSize = new Vector3(3, 5, 3);
    public float wallThickness = 0.2f;
    public Vector3Int tileAmmount = new Vector3Int(10, 1, 10);
    public Vector3Int chunkPathDistance = new Vector3Int(1, 0, 1);
    public int mapPathDistanceInt { get { return (chunkPathDistance.x * 2 + 1) * (chunkPathDistance.y * 2 + 1) * (chunkPathDistance.z * 2 + 1); } }

    public int seed;

	//Scripts
	GeneratorManagerScript manager;

	//calculations
	public Vector3 chunkSize
    {
        get
        {
            return new Vector3(tileAmmount.x * tileSize.x, tileAmmount.y * tileSize.y, tileAmmount.z * tileSize.z);
        }
    }
    public Vector3Int mapCenter
    {
        get
        {
            return (layers.render.Length - Vector3Int.one) / 2;
        }
    }
    public Vector3Int mapRenderDistantStart
    {
        get
        {
            return mapCenter - layers.render.size;
        }
    }
    public Vector3Int mapRenderDistantEnd
    {
        get
        {
            return mapCenter + layers.render.size;
        }
    }
    private void Awake()
    {
        layers.Init();

        chunkArray = new ChunkArray(this);
        manager = gameObject.GetComponent<GeneratorManagerScript>();
        MeshScript mesh = gameObject.GetComponent<MeshScript>();
        ChunkScript chunk = gameObject.GetComponent<ChunkScript>();
        GenerationScript generation = gameObject.GetComponent<GenerationScript>();
        GameEventsScript gameEvent = gameObject.GetComponent<GameEventsScript>();
        PathFindingScript pathFinding = gameObject.GetComponent<PathFindingScript>();
        //manager
        manager.prop = this;
        manager.meshS = mesh;
        manager.chunkS = chunk;
        manager.generationS = generation;
        //mesh
        //chunk
        chunk.prop = this;
        chunk.chunkArray = chunkArray;
        //generation
        generation.prop = this;
        //gameEvent
        manager.gameEventS = gameEvent;
        //pathFinding
        pathFinding.chunkArray = chunkArray;

        //Init
        manager.Init();
    }
	private void Update()
	{
        manager.Tick();
	}
	public int GetIndex(Vector3Int centerCoordinates, Vector3Int targetItemCoordinates, Vector3Int sizeFromCenter)
    {
		sizeFromCenter = sizeFromCenter * 2 + Vector3Int.one;

		Vector3Int targetCellLocation = targetItemCoordinates - centerCoordinates;

		return targetCellLocation.x
                + (targetCellLocation.y * sizeFromCenter.x)
                + (targetCellLocation.z * sizeFromCenter.x * sizeFromCenter.y);
    }
	public Vector3Int GetTargetItemCoordinates(Vector3Int centerCoordinates, int locationIndex, Vector3Int sizeFromCenter)
	{
        sizeFromCenter = sizeFromCenter * 2 + Vector3Int.one;

        Vector3Int targetCellLocation = Vector3Int.zero;
		targetCellLocation.x = locationIndex % sizeFromCenter.x; //26 % 3 = 2
		targetCellLocation.y = (locationIndex / sizeFromCenter.x) % sizeFromCenter.y; //2 % 3 = 2
		targetCellLocation.z = locationIndex / (sizeFromCenter.x * sizeFromCenter.y); //26 / 9 = 2

		return targetCellLocation + centerCoordinates; 
	}
	public (Vector3Int outer,Vector3Int inner) FixOutOfBounds(Vector3Int outerCoordinates, Vector3Int innerCoordinates, Vector3Int innerBounds)
    {
		outerCoordinates += new Vector3Int(innerCoordinates.x / innerBounds.x, innerCoordinates.y / innerBounds.y, innerCoordinates.z / innerBounds.z);
        innerCoordinates -= new Vector3Int(innerCoordinates.x / innerBounds.x, innerCoordinates.y / innerBounds.y, innerCoordinates.z / innerBounds.z) * innerBounds;
        return (outerCoordinates, innerCoordinates);
	}
	public bool GetSide(int chunk, Vector3Int side, Position pos)
	{
		Vector3Int coordinates = chunkArray.chunks.coordinates[chunk];
		//swap back,down,left to front,up,right
		side += pos.Tile;
		if (side.x < 0)
		{
			coordinates.x--;
		}
		if (side.y < 0)
		{
			coordinates.y--;
		}
		if (side.z < 0)
		{
			coordinates.z--;
		}
		coordinates += new Vector3Int(side.x / tileAmmount.x, side.y / tileAmmount.y, side.z / tileAmmount.z);
		side.x = (side.x % tileAmmount.x + tileAmmount.x) % tileAmmount.x;
		side.y = (side.y % tileAmmount.y + tileAmmount.y) % tileAmmount.y;
		side.z = (side.z % tileAmmount.z + tileAmmount.z) % tileAmmount.z;
		Vector3Int location = chunkArray.GetLocation(coordinates);
		return chunkArray.chunksGeneration.sides[chunkArray.chunksGeneration.layer.GetIndex(location), side.x, side.y, side.z, pos.Side];
	}
	void FixCoordinates(ref Vector3Int chunk, ref Vector3Int tile)
    {
        Vector3Int globalCoordinates = new Vector3Int(chunk.x * tileAmmount.x + tile.x, chunk.y * tileAmmount.y + tile.y, chunk.z * tileAmmount.z + tile.z);
        chunk = new Vector3Int(globalCoordinates.x / tileAmmount.x, globalCoordinates.y / tileAmmount.y, globalCoordinates.z / tileAmmount.z);
        tile = new Vector3Int(globalCoordinates.x % tileAmmount.x, globalCoordinates.y % tileAmmount.y, globalCoordinates.z % tileAmmount.z);
    }
}
