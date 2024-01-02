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
	public Vector3Int GetCoordinates(Vector3Int location) {
		return location + this.coordinates - prop.layers.hierarchy[0].size;
	}
	public Vector3Int GetLocation(Vector3Int coordinates){
		return coordinates - this.coordinates;
	}
	public void MoveChunks(Vector3Int distanceToMove) {
		Debug.Log("moving: " + distanceToMove);
		coordinates += distanceToMove;
		distanceToMove = -distanceToMove;
		int sign;
		int length;
		Vector3Int shiftingLocation = Vector3Int.zero;
		Vector3Int prependingLocation = Vector3Int.zero;
		Vector3Int direction = Vector3Int.zero;

		Vector3Int location = Vector3Int.zero;
		for (int layer = 0; layer < prop.layers.hierarchy.Length; layer++) {
			for (int dimension = 0; dimension <= 2; dimension++) {
				length = prop.layers.hierarchy[layer].Length[dimension];
				sign = (distanceToMove[dimension] >> 31) | 1;
				shiftingLocation[dimension] = (sign + 1) / 2 * (length - 1);							// 0 for negative, length - 1 for non-negative
				prependingLocation[dimension] = (1 - sign) / 2 * length + (sign + 1) / 2 * -1;			// length for negative, -1 for non-negative
				direction[dimension] = sign;															// -1 for negative, 1 for non-negative
			}
			for (location.x = shiftingLocation.x; location.x != prependingLocation.x; location.x -= direction.x) {
				for (location.y = shiftingLocation.y; location.y != prependingLocation.y; location.y -= direction.y) {
					for (location.z = shiftingLocation.z; location.z != prependingLocation.z; location.z -= direction.z) {
						if (prop.layers.hierarchy[layer].IsLocationIncluded(location + distanceToMove)) {
							prop.layers.hierarchy[layer].MoveChunk(location, location + distanceToMove);
						}
						else {
							prop.layers.hierarchy[layer].CallDestroyChunk(location);
						}
						if(!prop.layers.hierarchy[layer].IsLocationIncluded(location - distanceToMove)) {
							prop.layers.hierarchy[layer].CallCreateChunk(location);
						}
					}
				}
			}
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
	public Layer layer;

	public Chunks(Layer layer) {
		this.layer = layer;
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
		rendered[index] = true;
	}
}

