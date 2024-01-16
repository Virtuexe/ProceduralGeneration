using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Generation {
	public class PathFindingScript : MonoBehaviour {
		[HideInInspector]
		int chunk;
		Vector3Int coordinates;
		public void Generate(Vector3Int pathLocation) {
			//this.chunk = ChunkArray.;
			//this.coordinates = ChunkArray.GetCoordinates()
			//Vector3Int tile = new Vector3Int();
			//for (tile.x = 0; tile.x < prop.tileSize.x; tile += Vector3Int.right)
			//	for (tile.y = 0; tile.y < prop.tileSize.x; tile += Vector3Int.up)
			//		for (tile.z = 0; tile.z < prop.tileSize.z; tile += Vector3Int.forward)
			//		{
			//			WriteTable(tile);
			//		}

		}
		private void WriteTable(Vector3Int tile) {
			for (int i = 0; i < 6; i++) {
				var coordinatesSide = GenerationProp.FixOutOfBounds(ChunkArray.coordinates, tile + Direction.Directions[i].RelValue + Direction.Directions[i].Tile, GenerationProp.tileAmmount);
				if (!ChunkArray.sides[Layers.generation.GetIndex(coordinatesSide.outer), coordinatesSide.inner.x, coordinatesSide.inner.y, coordinatesSide.inner.z, Direction.Directions[i].Side]) {
					ShareSet(tile, 1, i);
				}

			}
		}
		public void ShareSet(Vector3Int v, int dis, int pos) {
			//var coordinatesCell = prop.FixOutOfBounds(ChunkArray.coordinates, v + Position.Directions[pos].RelValue, prop.tileAmmount);
			//int chunk2 = ChunkArray.chunksGeneration.layer.GetIndex(coordinatesCell.outer);
			//int chunk2Location = prop.GetIndex(ChunkArray.chunks.coordinates[chunk], ChunkArray.chunks.coordinates[chunk2], prop.chunkPathDistance);
			//int chunkLocation = prop.GetIndex(ChunkArray.chunks.coordinates[chunk2], ChunkArray.chunks.coordinates[chunk], prop.chunkPathDistance);
			//Vector3Int v2 = coordinatesCell.inner;

			//ChunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, v2.x, v2.y, v2.z].Set(dis, pos);
			//ChunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, v.x, v.y, v.z].Set(dis, pos);

			//for (int x = 0; x < prop.tileSize.x; x++)
			//	for (int y = 0; y < prop.tileSize.x; y++)
			//		for (int z = 0; z < prop.tileSize.z; z++)
			//		{
			//			if (ChunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].distance < ChunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance)
			//				ChunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].Set(ChunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance + 1, pos);
			//			else if (ChunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].distance == ChunkArray.paths[chunk2, v2.x, v2.y, v2.z, chunkLocation, x, y, z].distance)
			//				ChunkArray.paths[chunk, v.x, v.y, v.z, chunk2Location, x, y, z].AddPosition(pos);
			//		}
		}
	}
}
