using MyArrays;
using UnityEngine;
namespace Generation {
	public class GenerationScript : MonoBehaviour {
		public int corridorComplexityMin;
		public int corridorComplexityMax;

		private int chunkGeneration;
		private int chunk;
		private Vector3Int coordinates;
		private CustomRandom rand = new CustomRandom();

		public void GenerateChunk(Vector3Int locationGeneration) {
            chunkGeneration = Layers.generation.LayerLocationToIndex(locationGeneration);
			chunk = Layers.generation.GetIndex(locationGeneration, Layers.hierarchy[0]);
			coordinates = ChunkArray.GetCoordinates(Layers.generation.LayerLocationToLayerCoordinates(locationGeneration));

			for (int z = 0; z < GenerationProp.tileAmmount.z; z++) {
				for (int y = 0; y < GenerationProp.tileAmmount.y; y++) {
					for (int x = 0; x < GenerationProp.tileAmmount.x; x++) {
						for (int d = 0; d < 3; d++) {
							ChunkArray.sides[chunkGeneration, x, y, z, d] = true;
						}
					}
				}
			}
			Set3<int> generationDetailOffset;
			for(generationDetailOffset.x = -Layers.generation.lowerLayerSize.x; generationDetailOffset.x <= Layers.generation.lowerLayerSize.x; generationDetailOffset.x++) {
				for (generationDetailOffset.y = -Layers.generation.lowerLayerSize.y; generationDetailOffset.y <= Layers.generation.lowerLayerSize.y; generationDetailOffset.y++) {
					for (generationDetailOffset.z = -Layers.generation.lowerLayerSize.z; generationDetailOffset.z <= Layers.generation.lowerLayerSize.z; generationDetailOffset.z++) {
						Set3<int> offsetedLocationGeneration = new Set3<int>();
						for (int i = 0; i < 3; i++) {
							offsetedLocationGeneration[i] = locationGeneration[i] + generationDetailOffset[i];
						}
						int generationDetailIndex = Layers.generationDetail.LayerLocationToIndex((Layers.generation.LayerLocationToOtherLayerLocation(new Vector3Int(offsetedLocationGeneration.x, offsetedLocationGeneration.y, offsetedLocationGeneration.z), Layers.generationDetail)));
						for(int room = 0; room < ChunkArray.roomsAmount[generationDetailIndex]; room++) {
							Fill(ChunkArray.rooms[generationDetailIndex, room, 0], ChunkArray.rooms[generationDetailIndex, room, 1], false);
						}
					}
				}
			}
			Layers.generation.created[locationGeneration.x, locationGeneration.y, locationGeneration.z] = true;
		}
		public void Fill(Vector3Int start, Vector3Int end, bool side) {
			// Calculate the distance between start and end coordinates
			Vector3Int distance = end - start;
			Vector3Int direction = new Vector3Int(
				distance.x < 0 ? -1 : 1,
				distance.y < 0 ? -1 : 1,
				distance.z < 0 ? -1 : 1);
			Vector3Int corner = new Vector3Int(start.x >= end.x ? start.x : end.x, start.y >= end.y ? start.y : end.y, start.z >= end.z ? start.z : end.z) ;
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
						if (x != corner.x) {
							ChunkArray.sides[chunkGeneration, x, y, z, directionX.Side] = side;
						}
						if (y != corner.y) {
							ChunkArray.sides[chunkGeneration, x, y, z, directionY.Side] = side;
						}
						if (z != corner.z) {
							ChunkArray.sides[chunkGeneration, x, y, z, directionZ.Side] = side;
						}
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

