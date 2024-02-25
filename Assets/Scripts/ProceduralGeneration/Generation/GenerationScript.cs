using MyArrays;
using System;
using UnityEngine;
namespace Generation {
	public static class GenerationScript {
		private static int chunkGeneration;
		private static int chunk;
		private static Vector3Int generationDetailLocation;
		private static int generationDetailIndex;
		private static Vector3Int coordinates;
		private static CustomRandom rand = new CustomRandom();
		public static unsafe void GenerateChunk(Vector3Int locationGeneration) {
			chunkGeneration = Layers.generation.LayerLocationToIndex(locationGeneration);
			chunk = Layers.generation.GetIndex(locationGeneration, Layers.hierarchy[0]);
			generationDetailLocation = Layers.generation.LayerLocationToOtherLayerLocation(locationGeneration, Layers.generationDetail);
			generationDetailIndex = Layers.generationDetail.LayerLocationToIndex(generationDetailLocation);
			coordinates = ChunkArray.GetCoordinates(Layers.generation.LayerLocationToLayerCoordinates(locationGeneration));

			for (int z = 0; z < GenerationProp.tileAmount.z; z++) {
				for (int y = 0; y < GenerationProp.tileAmount.y; y++) {
					for (int x = 0; x < GenerationProp.tileAmount.x; x++) {
						for (int d = 0; d < 3; d++) {
							ChunkArray.sides[chunkGeneration, x, y, z, d] = true;
						}
					}
				}
			}
			Layers.generationDetail.radius.LoopRadius((generationDetailOffset) => {
				Set3<int> offsetedLocationGeneration = new Set3<int>();
				for (int i = 0; i < 3; i++) {
					offsetedLocationGeneration[i] = locationGeneration[i] + generationDetailOffset[i];
				}
				int generationDetailIndex = Layers.generationDetail.LayerLocationToIndex((Layers.generation.LayerLocationToOtherLayerLocation(new Vector3Int(offsetedLocationGeneration.x, offsetedLocationGeneration.y, offsetedLocationGeneration.z), Layers.generationDetail)));
				for (int room = 0; room < ChunkArray.roomsAmount[generationDetailIndex]; room++) {
					Vector3Int roomOrigin = ChunkArray.roomOrigins[generationDetailIndex, room] + (new Vector3Int(generationDetailOffset.x, generationDetailOffset.y, generationDetailOffset.z) * GenerationProp.tileAmount);
					Fill(roomOrigin, roomOrigin + ChunkArray.roomSizes[generationDetailIndex, room], false, true);
				}
			});
			Ranges selecetedRooms = new Ranges(ChunkArray.roomsAmount[generationDetailIndex]);
			for (int room = 0; room < ChunkArray.roomsAmount[generationDetailIndex]; room++) {
				rand.
				Layers.generationDetail.radius.LoopRadius((generationDetailOffset) => { 
				});
			} 
						
			//for (int thisRoom = 0; thisRoom < ChunkArray.roomsAmount[generationDetailIndex]; thisRoom++) {
			//	for (generationDetailOffset.x = -Layers.generationDetail.layerSize.x; generationDetailOffset.x <= Layers.generationDetail.layerSize.x; generationDetailOffset.x++) {
			//		for (generationDetailOffset.y = -Layers.generationDetail.layerSize.y; generationDetailOffset.y <= Layers.generationDetail.layerSize.y; generationDetailOffset.y++) {
			//			for (generationDetailOffset.z = -Layers.generationDetail.layerSize.z; generationDetailOffset.z <= Layers.generationDetail.layerSize.z; generationDetailOffset.z++) {
			//				Set3<int> offsetedLocationGeneration = new Set3<int>();
			//				for (int i = 0; i < 3; i++) {
			//					offsetedLocationGeneration[i] = locationGeneration[i] + generationDetailOffset[i];
			//				}
			//				int generationDetailIndex = Layers.generationDetail.LayerLocationToIndex((Layers.generation.LayerLocationToOtherLayerLocation(new Vector3Int(offsetedLocationGeneration.x, offsetedLocationGeneration.y, offsetedLocationGeneration.z), Layers.generationDetail)));
			//				for (int room = 0; room < ChunkArray.roomsAmount[generationDetailIndex]; room++) {
			//					if (generationDetailIndex == GenerationScript.generationDetailIndex && thisRoom == room) {
			//						continue;
			//					}
			//					Vector3Int pointSource = ChunkArray.roomOrigins[GenerationScript.generationDetailIndex, thisRoom] + (ChunkArray.roomSizes[GenerationScript.generationDetailIndex, thisRoom] / 2);
			//					Vector3Int roomOrigin = ChunkArray.roomOrigins[generationDetailIndex, room] + (new Vector3Int(generationDetailOffset.x, generationDetailOffset.y, generationDetailOffset.z) * GenerationProp.tileAmount);
			//					Vector3Int pointDestination = roomOrigin + (ChunkArray.roomSizes[generationDetailIndex, room] / 2);
			//					Fill(pointSource, new Vector3Int(pointSource.x, pointSource.y, pointDestination.z), false, true);
			//					Fill(new Vector3Int(pointSource.x, pointSource.y, pointDestination.z), new Vector3Int(pointSource.x, pointDestination.y, pointDestination.z), false, true);
			//					Fill(new Vector3Int(pointSource.x, pointDestination.y, pointDestination.z), pointDestination, false, true);
			//				}
			//			}
			//		}
			//	}
			//}
			Layers.generation.created[locationGeneration.x, locationGeneration.y, locationGeneration.z] = true;
		}
		public static void Fill(Vector3Int start, Vector3Int end, bool side, bool accesible) {
			// Calculate the distance between start and end coordinates
			Vector3Int distance = end - start;
			Vector3Int direction = new Vector3Int(
				distance.x < 0 ? -1 : 1,
				distance.y < 0 ? -1 : 1,
				distance.z < 0 ? -1 : 1);
			Vector3Int corner = new Vector3Int(start.x >= end.x ? start.x : end.x, start.y >= end.y ? start.y : end.y, start.z >= end.z ? start.z : end.z);
			// Create direction positions
			Direction directionX = new Direction(direction.x, 0, 0);
			Direction directionY = new Direction(0, direction.y, 0);
			Direction directionZ = new Direction(0, 0, direction.z);

			// Iterate through the coordinates
			for (int x = start.x; x * direction.x <= end.x * direction.x; x += direction.x) {
				for (int y = start.y; y * direction.y <= end.y * direction.y; y += direction.y) {
					for (int z = start.z; z * direction.z <= end.z * direction.z; z += direction.z) {
						// Skip iteration if any coordinate is out of bounds
						if (x >= GenerationProp.tileAmount.x || y >= GenerationProp.tileAmount.y || z >= GenerationProp.tileAmount.z)
							continue;
						if (x <= -1 || y <= -1 || z <= -1)
							continue;
						ChunkArray.accesible[chunkGeneration, x, y, z] = accesible;
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


		public static void Line(Vector3Int start, int distance, Direction positition, bool side) {
			for (int i = 0; i < distance; i++) {
				Vector3Int coordinates = start + (i * positition.RelValue) + positition.Tile;
				ChunkArray.sides[chunkGeneration, coordinates.x, coordinates.y, coordinates.z, positition.Side] = side;
			}
		}
	}
}

