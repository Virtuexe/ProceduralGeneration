using MyArrays;
using MyMath;
using UnityEngine;

namespace Generation {
	public static class GenerationDetail {
		static CustomRandom rand = new CustomRandom();

		static Range<Set3<int>> roomPossiblePlacement = new Range<Set3<int>>(
			new Set3<int>(
				-GenerationProp.tileAmount.x + 1,
				-GenerationProp.tileAmount.y + 1,
				-GenerationProp.tileAmount.z + 1),
			new Set3<int>(
				GenerationProp.tileAmount.x + (GenerationProp.tileAmount.x * Layers.generationDetail.layerSize.x) - 1,
				GenerationProp.tileAmount.y + (GenerationProp.tileAmount.y * Layers.generationDetail.layerSize.y) - 1,
				GenerationProp.tileAmount.z + (GenerationProp.tileAmount.z * Layers.generationDetail.layerSize.z) - 1));
		public static void GenerateDetail(Vector3Int locationGenerationDetail) {
			Vector3Int coordinates = Layers.generationDetail.LayerLocationToCoodinates(locationGenerationDetail);
			rand.SetSeed(coordinates.x, coordinates.y, coordinates.z);
			
			int indexGenerationDetail = Layers.generationDetail.LayerLocationToIndex(locationGenerationDetail);
			ChunkArray.roomsAmount[indexGenerationDetail] = rand.Number(GenerationProp.roomCount.min, GenerationProp.roomCount.max);
			for (int room = 0; room < ChunkArray.roomsAmount[indexGenerationDetail]; room++) {
				Vector3Int roomSize = new Vector3Int(
					rand.Number(GenerationProp.roomSize.min.x , GenerationProp.roomSize.max.x),
					rand.Number(GenerationProp.roomSize.min.y , GenerationProp.roomSize.max.y),
					rand.Number(GenerationProp.roomSize.min.z , GenerationProp.roomSize.max.z));
				Vector3Int roomOrigin = new Vector3Int(
					rand.Number(roomPossiblePlacement.min.x + roomSize.x, roomPossiblePlacement.max.x), 
					rand.Number(roomPossiblePlacement.min.y + roomSize.y, roomPossiblePlacement.max.y), 
					rand.Number(roomPossiblePlacement.min.z + roomSize.z, roomPossiblePlacement.max.z));
				ChunkArray.rooms[indexGenerationDetail, room, 0] = roomOrigin;
				ChunkArray.rooms[indexGenerationDetail, room, 1] = roomOrigin - roomSize;
			}
			Layers.generationDetail.created[locationGenerationDetail.x, locationGenerationDetail.y, locationGenerationDetail.z] = true;
		}
	}
}