using MyArrays;
using System;
using System.Numerics;
using UnityEngine;
namespace Generation {
	public static class GenerationScript {
		private static Vector3Int locationGeneration;
		private static int chunkGeneration;
		private static int chunk;
		private static Vector3Int generationDetailLocation;
		private static int generationDetailIndex;
		private static Vector3Int coordinates;
		private static CustomRandom rand = new CustomRandom();
		public static unsafe void GenerateChunk(Vector3Int locationGeneration) {
			GenerationScript.locationGeneration = locationGeneration;
			chunkGeneration = Layers.generation.LayerLocationToIndex(locationGeneration);
			chunk = Layers.generation.GetIndex(locationGeneration, Layers.hierarchy[0]);
			generationDetailLocation = Layers.generation.LayerLocationToOtherLayerLocation(locationGeneration, Layers.generationDetail);
			generationDetailIndex = Layers.generationDetail.LayerLocationToIndex(generationDetailLocation);
			coordinates = ChunkArray.GetCoordinates(Layers.generation.LayerLocationToLayerCoordinates(locationGeneration));
			rand.SetSeed(coordinates.x, coordinates.y, coordinates.z);

			for (int z = 0; z < GenerationProp.tileAmount.z; z++) {
				for (int y = 0; y < GenerationProp.tileAmount.y; y++) {
					for (int x = 0; x < GenerationProp.tileAmount.x; x++) {
						for (int d = 0; d < 3; d++) {
							ChunkArray.sides[chunkGeneration, x, y, z, d] = true;
						}
						ChunkArray.accesible[chunkGeneration, x, y, z] = false;
					}
				}
			}
			CreateRooms();
			CreateHallways();
			Layers.generation.created[locationGeneration.x, locationGeneration.y, locationGeneration.z] = true;
		}
		private static void CreateRooms() {
			Layers.generationDetail.radius.LoopRadius((generationDetailOffset) => {
				Set3<int> offsetedLocationGeneration = new Set3<int>();
				for (int i = 0; i < 3; i++) {
					offsetedLocationGeneration[i] = locationGeneration[i] + generationDetailOffset[i];
				}
				int generationDetailIndex = Layers.generationDetail.LayerLocationToIndex((Layers.generation.LayerLocationToOtherLayerLocation(new Vector3Int(offsetedLocationGeneration.x, offsetedLocationGeneration.y, offsetedLocationGeneration.z), Layers.generationDetail)));
				for (int room = 0; room < ChunkArray.roomsAmount[generationDetailIndex]; room++) {
					Vector3Int roomOrigin = ChunkArray.roomCenters[generationDetailIndex, room] + (new Vector3Int(generationDetailOffset.x, generationDetailOffset.y, generationDetailOffset.z) * GenerationProp.tileAmount);
					Fill(roomOrigin - (ChunkArray.roomSizes[generationDetailIndex, room] / 2), roomOrigin + ((ChunkArray.roomSizes[generationDetailIndex, room] + Vector3Int.one) / 2), false, true);
				}
			});
		}
		private static void CreateHallways() {
			if(ChunkArray.roomsAmount[generationDetailIndex] <= 1) {
				goto Skip;
			}
			Ranges selecetedRoomRange = new Ranges(ChunkArray.roomsAmount[generationDetailIndex]);
			for (int thisRoomIndex = 0; thisRoomIndex < ChunkArray.roomsAmount[generationDetailIndex]; thisRoomIndex++) {
				int roomConnectWithIndex;
				if (selecetedRoomRange.Count != 0) {
					selecetedRoomRange.Clear();
				}
				Ranges selecetedRoomRangeForThisRoom = selecetedRoomRange.Copy();
				selecetedRoomRangeForThisRoom -= thisRoomIndex;
				roomConnectWithIndex = rand.ChooseFromRange(selecetedRoomRangeForThisRoom);
				selecetedRoomRange -= roomConnectWithIndex;


				Ranges vectorRange = new Ranges(3);
				Set3Int roomOrigin = (Set3Int)ChunkArray.roomCenters[generationDetailIndex, thisRoomIndex];
				Set3Int roomOriginConnectWith = (Set3Int)ChunkArray.roomCenters[generationDetailIndex, roomConnectWithIndex];
				Set3Int toVector = roomOrigin;
				Set3Int fromVector = roomOrigin;
				for (int componentCounter = 0; componentCounter < 3; componentCounter++) {
					int component = rand.ChooseFromRange(vectorRange);
					vectorRange -= component;
					toVector[component] = roomOriginConnectWith[component];

					Fill((Vector3Int)fromVector, (Vector3Int)toVector, false, true);

					fromVector = toVector;
				}
				vectorRange.Free();
				selecetedRoomRangeForThisRoom.Free();
			}
			selecetedRoomRange.Free();
			Skip:
			CreateHallwaysIntoChunk(Set3Int.forward, false);
			CreateHallwaysIntoChunk(Set3Int.right, false);
			CreateHallwaysIntoChunk(Set3Int.left, true);
			CreateHallwaysIntoChunk(Set3Int.back, true);
		}
		private static void CreateHallwaysIntoChunk(Set3Int generationDetailOffset, bool thisPriority) {
			if (generationDetailOffset == Set3Int.zero) {
				return;
			}
			int generationDetailOffsetedIndex = Layers.generationDetail.LayerLocationToIndex((Vector3Int)(generationDetailLocation + generationDetailOffset));

			Set3Int roomOrigin = (Set3Int)ChunkArray.roomCenters[generationDetailIndex, 0];
			Set3Int roomOriginConnectWith = (Set3Int)ChunkArray.roomCenters[generationDetailOffsetedIndex, 0] + (generationDetailOffset * GenerationProp.tileAmount);
			Set3Int toVector;
			Set3Int fromVector;
			if (thisPriority) {
				toVector = roomOrigin;
				fromVector = roomOrigin;
			}
			else {
				toVector = roomOriginConnectWith;
				fromVector = roomOriginConnectWith;
				roomOriginConnectWith = roomOrigin;
			}
			for (int component = 0; component < 3; component++) {
				toVector[component] = roomOriginConnectWith[component];

				Fill((Vector3Int)fromVector, (Vector3Int)toVector, false, true);

				fromVector = toVector;
			}
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

