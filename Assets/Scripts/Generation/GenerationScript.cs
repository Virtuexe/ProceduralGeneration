using System;
using TMPro;
using UnityEngine;
namespace Generation {
	public class GenerationScript : MonoBehaviour {
		public int corridorComplexityMin;
		public int corridorComplexityMax;

		private int chunkGeneration;
		private int chunk;
		private Vector3Int coordinates;

		CustomRandom rand;
		public void GenerateChunk(Vector3Int locationGeneration) {
            chunkGeneration = Layers.generation.GetIndex(locationGeneration);
			chunk = Layers.generation.GetIndex(locationGeneration, Layers.hierarchy[0]);
			coordinates = ChunkArray.GetCoordinates(Layers.generation.LocationToCoordinates(locationGeneration));
			rand = new CustomRandom(GenerationProp.seed, new int[] { coordinates.x, coordinates.y, coordinates.z });

            for (int z = 0; z < GenerationProp.tileAmmount.z; z++) {
                for (int y = 0; y < GenerationProp.tileAmmount.y; y++) {
                    for (int x = 0; x < GenerationProp.tileAmmount.x; x++) {
                        for (int d = 0; d < 3; d++) {
                            ChunkArray.sides[chunkGeneration, x, y, z, d] = false;
                        }
                    }
                }
            }
            for (int z = 0; z < GenerationProp.tileAmmount.z; z++) {
				for (int y = 0; y < GenerationProp.tileAmmount.y; y++) {
					for (int x = 0; x < GenerationProp.tileAmmount.x; x++) {
						for (int d = 0; d < 3; d++) {
							int lol = rand.random.Next(0, 6);
							if (lol == 1)
								ChunkArray.sides[chunkGeneration, x, y, z, d] = true;
							if (d == 0)
								ChunkArray.sides[chunkGeneration, x, y, z, d] = true;
						}
					}
				}
			}
			ChunkArray.genereted[chunkGeneration] = true;
		}
		//void PointsOfInterest()
		//{
		//    int amount = rand.random.Next(corridorComplexityMin, corridorComplexityMax + 1);
		//    manager.chunks.items[chunk.x, chunk.y, chunk.z].points = new Vector3Int[amount];
		//    for (int i = 0; i < amount; i++)
		//    {
		//        manager.chunks.items[chunk.x, chunk.y, chunk.z].points[i] = new Vector3Int(rand.random.Next(0, GenerationProp.tileAmmount.x), rand.random.Next(0, GenerationProp.tileAmmount.y), rand.random.Next(0, GenerationProp.tileAmmount.z));
		//    }
		//}
		//void ConnectPointsOfInterest()
		//{
		//    Vector3Int last = manager.chunks.items[chunk.x, chunk.y, chunk.z].points[0];
		//    for (int i = 1; i < manager.chunks.items[chunk.x, chunk.y, chunk.z].points.Length; i++)
		//    {
		//        Path(last, manager.chunks.items[chunk.x, chunk.y, chunk.z].points[i]);
		//        last = manager.chunks.items[chunk.x, chunk.y, chunk.z].points[i];
		//    }
		//}
		//GENERATION TOOLS
		//public void Path(Vector3Int start, Vector3Int end)
		//{
		//    int[] axisIndex = { 0, 1, 2 };
		//    System.Random rnd = new System.Random();
		//    for (int i = axisIndex.Length - 1; i > 0; i--)
		//    {
		//        int j = rnd.Next(i + 1);
		//        int temp = axisIndex[i];
		//        axisIndex[i] = axisIndex[j];
		//        axisIndex[j] = temp;
		//    }
		//    Vector3Int v1 = new Vector3Int(start.x, start.y, start.z);
		//    v1[axisIndex[0]] = end[axisIndex[0]];
		//    Vector3Int v2 = v1;
		//    v2[axisIndex[1]] = end[axisIndex[1]];
		//    Line(start, v1);
		//    Line(v1, v2);
		//    Line(v2, end);
		//}
		public void Fill(Vector3Int start, Vector3Int end, bool side) {
			// Calculate the distance between start and end coordinates
			Vector3Int distance = end - start;
			Vector3Int direction = new Vector3Int(
				distance.x < 0 ? -1 : 1,
				distance.y < 0 ? -1 : 1,
				distance.z < 0 ? -1 : 1);
			Vector3Int corner = start.x + start.y + start.z >= end.x + end.y + end.z ? start : end;
			// Create direction positions
			Direction directionX = new Direction(direction.x, 0, 0);
			Direction directionY = new Direction(0, direction.y, 0);
			Direction directionZ = new Direction(0, 0, direction.z);

            // Iterate through the coordinates
            for (int x = start.x; x * direction.x <= end.x * direction.x; x += direction.x) {
                for (int y = start.y; y * direction.y <= end.y * direction.y; y += direction.y) {
					for (int z = start.z; z * direction.z <= end.z * direction.z; z += direction.z) {
						// Skip iteration if any coordinate is out of bounds
						if (x >= GenerationProp.tileAmmount.x || y >= GenerationProp.tileAmmount.y || z >= GenerationProp.tileAmmount.z)
							continue;
						if (x <= -1 || y <= -1 || z <= -1)
							continue;
						// Modify sides based on the direction
						if (x != corner.x)
							ChunkArray.sides[chunkGeneration, x, y, z, directionX.Side] = side;

						if (y != corner.y)
							ChunkArray.sides[chunkGeneration, x, y, z, directionY.Side] = side;

						if (z != corner.z)
							ChunkArray.sides[chunkGeneration, x, y, z, directionZ.Side] = side;
					}
				}
            }
        }
       

		public void Line(Vector3Int start, int distance, Direction positition, bool side) {
			for (int i = 0; i < distance; i++) {
				Vector3Int coordinates = start + (i * positition.RelValue) + positition.Tile;
				ChunkArray.sides[chunkGeneration, coordinates.x, coordinates.y, coordinates.z, positition.Side] = side;
			}
		}
	}
}

