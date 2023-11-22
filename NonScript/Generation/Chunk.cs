using System;
using UnityEngine;

[System.Serializable]

public struct Path
{
	public bool set { get; private set; }
	public int distance { get; private set; }
	public int positions { get; private set; }
	public Path(int distance, int positions)
	{
		this.distance = distance;
		this.positions = positions;
		this.set = false;
	}
	public void Set(int distance, int position)
	{
		this.set = true;
		this.distance = distance;
		this.positions = Position.Directions[position].Binary;
	}
	public void AddPosition(int position)
	{
		this.positions += Position.Directions[position].Binary;
	}
}
public class ChunkArray
{
	GenerationProp prop;

	public Chunks chunks;
	public ChunksGeneration chunksGeneration;
	public ChunksRender chunksRender;

	//[chunk,x,y,z,chunk2,x2,y2,z2]
	public Path[,,,,,,,] paths;

	public Vector3Int coordinates;
	public ChunkArray(GenerationProp prop)
	{
		chunks = new Chunks(prop.layers.hierarchy[0]);
		chunksGeneration = new ChunksGeneration(prop.layers.generation, prop);
		chunksRender = new ChunksRender(prop.layers.render);
		this.prop = prop;
	}
	public Vector3Int GetLocation(int i)
	{
		return chunks.coordinates[i] - coordinates;
	}
	public Vector3Int GetLocation(Vector3Int c)
	{
		return c - coordinates;
	}
	public void MoveChunk(Vector3Int location, Vector3Int targetLocation)
	{
		for(int layer = 0; layer < prop.layers.hierarchy.Length; layer++) {
			if (!prop.layers.hierarchy[layer].IsLocationIncluded(location) || prop.layers.hierarchy[layer].IsLocationIncluded(targetLocation)) {
				break;
			}
			prop.layers.hierarchy[layer].MoveChunk(location, targetLocation);
		}
	}
	public void CreateChunk(Vector3Int location) {
		for (int layer = 0; layer < prop.layers.hierarchy.Length; layer++) {
			if (!prop.layers.hierarchy[layer].IsLocationIncluded(location)) {
				break;
			}
			prop.layers.hierarchy[layer].CallCreateChunk(location);
		}
	}
	public void DestroyChunk(Vector3Int location) {
		for (int layer = 0; layer < prop.layers.hierarchy.Length; layer++) {
			if (!prop.layers.hierarchy[layer].IsLocationIncluded(location)) {
				break;
			}
			prop.layers.hierarchy[layer].CallDestroyChunk(location);
		}
	}
}
public delegate void CreateChunkFunction(int index);
public delegate void DestroyChunkFunction(int index);
public struct Chunks
{
	public Vector3Int[] coordinates;
	public int[] currentLayer;

	public Layer layer;

	public Chunks(Layer layer) {
		coordinates = new Vector3Int[layer.LengthInt];
		currentLayer = new int[layer.LengthInt];

		this.layer = layer;
		layer.SetCreateChunk = CreateChunk;
	}
	private void CreateChunk(int index) {
		currentLayer[index] = 0;
	}
}
public struct ChunksGeneration{
	public Layer layer;

	public bool[,,,,] sides;
	public bool[,,,] grid;
	public bool[] genereted;

	public ChunksGeneration(Layer layer, GenerationProp prop){
		sides = new bool[layer.LengthInt, prop.tileAmmount.x, prop.tileAmmount.y, prop.tileAmmount.z, 3];
		grid = new bool[layer.LengthInt, prop.tileAmmount.x, prop.tileAmmount.y, prop.tileAmmount.z];
		genereted = new bool[layer.LengthInt];

		this.layer = layer;
	}

}
public struct ChunksRender{
	public Layer layer;

	public GameObject[] gameObject;
	public bool[] rendered;
	public bool[] destroy;
	public ChunksRender(Layer layer){
		gameObject = new GameObject[layer.LengthInt];
		rendered = new bool[layer.LengthInt];
		destroy = new bool[layer.LengthInt];

		this.layer = layer;
		layer.SetCreateChunk = CreateChunk;
		layer.SetDestroyChunk = DestroyChunk;
	}
	private void CreateChunk(int index) {
		rendered[index] = false;
	}
	private void DestroyChunk(int index) {
		destroy[index] = true;
	}
}

