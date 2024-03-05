using UnityEngine;
using MyArrays;
namespace Generation {
	[System.Serializable]
	public unsafe class Layer {
		[HideInInspector]
		public Vector3Int size;
		[HideInInspector]
		public Vector3Int coordinatesOffset;

		public bool[,,] created;
		public bool[,,] pendingsDestroy;
		public int[,,] indexes;
		public readonly delegate*<Vector3Int, void> generateFunction;
		public readonly delegate*<Vector3Int, void> destroyFunction;

		public readonly Set3Int radius;
		public Set3Int length => size * 2 + Vector3Int.one;
		public int lengthInt => length.x * length.y * length.z;
		public Set3Int firstIndex => coordinatesOffset;
		public Set3Int lastIndex => coordinatesOffset + length - Vector3Int.one;
		public Layer(Vector3Int radius, delegate*<Vector3Int, void> generateFunction , delegate*<Vector3Int, void> destroyFunction) {
			this.radius = radius;
			this.size = radius;
			this.generateFunction = generateFunction;
			this.destroyFunction = destroyFunction;
		}
		public void Generate() {
			Vector3Int layerLocation = Vector3Int.zero;
			for (layerLocation.y = 0; layerLocation.y < length.y; layerLocation.y++)
				for (layerLocation.z = 0; layerLocation.z < length.z; layerLocation.z++)
					for (layerLocation.x = 0; layerLocation.x < length.x; layerLocation.x++) {
						if (pendingsDestroy[layerLocation.x, layerLocation.y, layerLocation.z]) {
							if (destroyFunction != null) {
								destroyFunction(layerLocation);
							}
							pendingsDestroy[layerLocation.x, layerLocation.y, layerLocation.z] = false;
						}
						else if (created[layerLocation.x, layerLocation.y, layerLocation.z]) {
							continue;
						}
						generateFunction(layerLocation);
						if (GameEventsScript.mainTask.OutOfTime())
							return;
					}
		}
		public void Init() {
			created = new bool[length.x, length.y, length.z];
			pendingsDestroy = new bool[length.x, length.y, length.z];
			indexes = new int[length.x, length.y, length.z];
			for (int z = 0; z < length.z; z++)
				for (int y = 0; y < length.y; y++)
					for (int x = 0; x < length.x; x++) {
						indexes[x, y, z] = x + y * length.x + z * length.x * length.y;
					}
		}
		public void MoveChunk(Vector3Int location, Vector3Int targetLocation) {
			int index = indexes[location.x, location.y, location.z];
			int targetIndex = indexes[targetLocation.x, targetLocation.y, targetLocation.z];
			bool pendingCreate = created[location.x, location.y, location.z];
            bool pendingCreateTarget = created[targetLocation.x, targetLocation.y, targetLocation.z];
            bool pendingDestroy = pendingsDestroy[location.x, location.y, location.z];
            bool pendingDestroyTarget = pendingsDestroy[targetLocation.x, targetLocation.y, targetLocation.z];

            indexes[targetLocation.x, targetLocation.y, targetLocation.z] = index;
			indexes[location.x, location.y, location.z] = targetIndex;
            created[targetLocation.x, targetLocation.y, targetLocation.z] = pendingCreate;
            created[location.x, location.y, location.z] = pendingCreateTarget;
            pendingsDestroy[targetLocation.x, targetLocation.y, targetLocation.z] = pendingDestroy;
            pendingsDestroy[location.x, location.y, location.z] = pendingDestroyTarget;
        }

        public bool GetPendingsDestroy(Vector3Int location) {
            return pendingsDestroy[location.x, location.y, location.z];
        }
        public bool GetCreated(Vector3Int location) {
            return created[location.x, location.y, location.z];
        }
        public int GetIndex(Vector3Int location, Layer layer) {
			return layer.LayerLocationToIndex(layer.LayerCoordinatesToLayerLocation(LayerLocationToLayerCoordinates(location)));
		}
		//TRANSLATIONS
		//to index
		public int LayerLocationToIndex(Vector3Int layerLocation) {
			return indexes[layerLocation.x, layerLocation.y, layerLocation.z];
		}
		public int CoordinatesToIndex(Vector3Int coordinates) {
			return LayerLocationToIndex(CoordinatesToLayerLocation(coordinates));
		}
		//to coordinates
		public Vector3Int LayerLocationToCoodinates(in Vector3Int layerLocation) {
			return ChunkArray.GetCoordinates(LayerLocationToLocation(layerLocation));
		}
		//to layer coordinates
		public Vector3Int LayerLocationToLayerCoordinates(in Vector3Int location) {
			return location + coordinatesOffset;
		}
		//to location
		public Vector3Int LayerLocationToLocation(in Vector3Int location) {
			return location - size;
		}
		public Vector3Int LocationToLayerLocation(in Vector3Int location) {
			return location + size;
		}
		//to layer location
		public Vector3Int LayerCoordinatesToLayerLocation(in Vector3Int layerCoordinates) {
			return layerCoordinates - coordinatesOffset;
		}
		public Vector3Int LayerLocationToOtherLayerLocation(in Vector3Int location, in Layer layer) {
			return layer.LayerCoordinatesToLayerLocation(LayerLocationToLayerCoordinates(location));
		}
		public bool IsLocationOutOfBounds(in Vector3Int location) {
			return location.x < 0 || location.y < 0 || location.z < 0 || location.x >= length.x || location.y >= length.y || location.z >= length.z;
		}
		public Vector3Int CoordinatesToLayerLocation(in Vector3Int coordinates) {
			return LocationToLayerLocation(ChunkArray.GetLocation(coordinates));
		}
	}
	[System.Serializable]
	public unsafe static class Layers {
		public static Layer render = new Layer(new Vector3Int(1,0,1), &ChunkScript.RenderChunk, &ChunkScript.DestroyChunk);
		public static Layer generation = new Layer(new Vector3Int(1, 1, 1), &GenerationScript.GenerateChunk, null);
		public static Layer generationDetail = new Layer(new Vector3Int(1, 0, 1), &GenerationDetail.GenerateDetail, null);
		public static Layer[] hierarchy { get; private set; }
		static Layers() {
			hierarchy = new Layer[] { new Layer(new Vector3Int(0,0,0), null, null), generationDetail, generation, render }; //atleast one item

			for (int layer = hierarchy.Length - 1; layer > 0; layer--) {
				hierarchy[layer - 1].size += hierarchy[layer].size;
			}
			Vector3Int size = hierarchy[0].size;
			hierarchy[0].Init();
			for (int layer = 1; layer < hierarchy.Length; layer++) {
				hierarchy[layer].coordinatesOffset = size - hierarchy[layer].size;
				hierarchy[layer].Init();
			}
		}
		public static void Regenerate() {
			for(int i = 0; i < hierarchy.Length; i++) {
				for(int chunkX = 0; chunkX < hierarchy[i].length.x; chunkX++) {
					for (int chunkY = 0; chunkY < hierarchy[i].length.y; chunkY++) {
						for (int chunkZ = 0; chunkZ < hierarchy[i].length.z; chunkZ++) {
							hierarchy[i].pendingsDestroy[chunkX, chunkY, chunkZ] = true;
							hierarchy[i].created[chunkX, chunkY, chunkZ] = false;
						}
					}
				}
			}
		}
	}
}
