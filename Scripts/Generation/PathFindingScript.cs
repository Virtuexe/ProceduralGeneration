using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PathFindingScript : MonoBehaviour
{
	[HideInInspector]
	public ChunkArray chunkArray;
	[HideInInspector]
	public GenerationProp prop;

	int chunk;
	Vector3Int coordinates;
	public void Generate(Vector3Int pathLocation)
	{
		//this.chunk = chunkArray.;
		//this.coordinates = chunkArray.GetCoordinates()
		//Vector3Int tile = new Vector3Int();
		//for (tile.x = 0; tile.x < prop.tileSize.x; tile += Vector3Int.right)
		//	for (tile.y = 0; tile.y < prop.tileSize.x; tile += Vector3Int.up)
		//		for (tile.z = 0; tile.z < prop.tileSize.z; tile += Vector3Int.forward)
		//		{
		//			WriteTable(tile);
		//		}

	}
	private void WriteTable(Vector3Int tile)
	{
		for (int i = 0; i < 6; i++)
		{
			var coordinatesSide = prop.FixOutOfBounds(chunkArray.coordinates, tile + Position.Directions[i].RelValue + Position.Directions[i].Tile, prop.tileAmmount);
			if (!chunkArray.chunksGeneration.sides[chunkArray.chunksGeneration.layer.GetIndex(coordinatesSide.outer), coordinatesSide.inner.x, coordinatesSide.inner.y, coordinatesSide.inner.z, Position.Directions[i].Side])
			{
				ShareSet(tile, 1, i);
			}

		}
	}
	public void ShareSet(Vector3Int v, int dis, int pos)
	{
		//var coordinatesCell = prop.FixOutOfBounds(chunkArray.coordinates, v + Position.Directions[pos].RelValue, prop.tileAmmount);
		//int chunk2 = chunkArray.chunksGeneration.layer.GetIndex(coordinatesCell.outer);
		//int chunk2Location = prop.GetIndex(chunkArray.chunks.coordinates[chunk], chunkArray.chunks.coordinates[chunk2], prop.chunkPathDistance);
		//int chunkLocation = prop.GetIndex(chunkArray.chunks.coordinates[chunk2], chunkArray.chunks.coordinates[chunk], prop.chunkPathDistance);
		//Vector3Int v2 = coordinatesCell.inner;

		//chunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, v2.x, v2.y, v2.z].Set(dis, pos);
		//chunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, v.x, v.y, v.z].Set(dis, pos);

		//for (int x = 0; x < prop.tileSize.x; x++)
		//	for (int y = 0; y < prop.tileSize.x; y++)
		//		for (int z = 0; z < prop.tileSize.z; z++)
		//		{
		//			if (chunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].distance < chunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance)
		//				chunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].Set(chunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance + 1, pos);
		//			else if (chunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].distance == chunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance)
		//				chunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].AddPosition(pos);
		//		}
	}
}
