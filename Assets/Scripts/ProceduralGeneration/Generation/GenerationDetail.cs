using MyArrays;
using UnityEngine;

namespace Generation {
	public static class GenerationDetail {
		static CustomRandom rand = new CustomRandom();
		public static void GenerateDetail(Vector3Int locationGenerationDetail) {
			Vector3Int coordinates = Layers.generationDetail.LayerLocationToCoodinates(locationGenerationDetail);
			rand.SetSeed(coordinates.x, coordinates.y, coordinates.z);
			
			int indexGenerationDetail = Layers.generationDetail.LayerLocationToIndex(locationGenerationDetail);
			ChunkArray.roomsAmount[indexGenerationDetail] = rand.Number(GenerationProp.roomsMinCount, GenerationProp.roomsMaxCount);
			Set3<int> roomPossiblePlacementMax = new Set3<int>(
				GenerationProp.tileAmmount.x + GenerationProp.tileAmmount.x * Layers.generation.lowerLayerSize.x - 1,
				GenerationProp.tileAmmount.y + GenerationProp.tileAmmount.y * Layers.generation.lowerLayerSize.y - 1,
				GenerationProp.tileAmmount.z + GenerationProp.tileAmmount.z * Layers.generation.lowerLayerSize.z - 1);
			Set3<int> roomPossiblePlacementMin = new Set3<int>(
				-GenerationProp.tileAmmount.x + 1,
				-GenerationProp.tileAmmount.y + 1,
				-GenerationProp.tileAmmount.z + 1);
			for (int i = 0; i < ChunkArray.roomsAmount[indexGenerationDetail]; i++) {
				ChunkArray.rooms[indexGenerationDetail, i, 0] = new Vector3Int(
					rand.Number(-roomPossiblePlacementMax.x, roomPossiblePlacementMax.x), 
					rand.Number(-roomPossiblePlacementMax.y, roomPossiblePlacementMax.y), 
					rand.Number(-roomPossiblePlacementMax.z, roomPossiblePlacementMax.z));
				ChunkArray.rooms[indexGenerationDetail, i, 1] = new Vector3Int(
					rand.Number(-roomPossiblePlacementMax.x, roomPossiblePlacementMax.x),
					rand.Number(-roomPossiblePlacementMax.y, roomPossiblePlacementMax.y),
					rand.Number(-roomPossiblePlacementMax.z, roomPossiblePlacementMax.z));
			}
			Layers.generationDetail.created[locationGenerationDetail.x, locationGenerationDetail.y, locationGenerationDetail.z] = true;
		}
	}
}