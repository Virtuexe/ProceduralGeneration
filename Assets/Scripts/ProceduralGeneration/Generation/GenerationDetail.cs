using MyArrays;
using MyMath;
using UnityEngine;

namespace Generation {
	public static class GenerationDetail {
		static CustomRandom rand = new CustomRandom();
		public static void GenerateDetail(Vector3Int locationGenerationDetail) {
			Vector3Int coordinates = Layers.generationDetail.LayerLocationToCoodinates(locationGenerationDetail);
			rand.SetSeed(coordinates.x, coordinates.y, coordinates.z);
			
			int indexGenerationDetail = Layers.generationDetail.LayerLocationToIndex(locationGenerationDetail);
			ChunkArray.roomsAmount[indexGenerationDetail] = rand.Integer(GenerationProp.roomCount.min, GenerationProp.roomCount.max);
			for (int room = 0; room < ChunkArray.roomsAmount[indexGenerationDetail]; room++) {
				Vector3Int roomSize = new Vector3Int(
					rand.Integer(GenerationProp.roomSize.min.x, GenerationProp.roomSize.max.x),
					rand.Integer(GenerationProp.roomSize.min.y, GenerationProp.roomSize.max.y),
					rand.Integer(GenerationProp.roomSize.min.z, GenerationProp.roomSize.max.z));
				Vector3Int roomCenter = new Vector3Int(
					rand.Index(GenerationProp.tileAmount.x), 
					rand.Index(GenerationProp.tileAmount.y),
					rand.Index(GenerationProp.tileAmount.z));
				ChunkArray.roomCenters[indexGenerationDetail, room] = roomCenter;
				ChunkArray.roomSizes[indexGenerationDetail, room] = roomSize;
			}
			Layers.generationDetail.created[locationGenerationDetail.x, locationGenerationDetail.y, locationGenerationDetail.z] = true;
		}
	}
}