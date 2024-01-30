using System;
using UnityEngine;
namespace Generation {
	public delegate void LayerFunction(int chunk);
	[System.Serializable]
	public class Layer {
		[HideInInspector]
		public Vector3Int size;
		[HideInInspector]
		public Vector3Int coordinatesOffset;

		public bool[,,] created;
		public bool[,,] pendingsDestroy;
		public int[,,] indexes;

		public LayerFunction function;
		public Vector3Int lowerLayerSize; // modifier how many lower layers should be created around edges this
		public Vector3Int Length => size * 2 + Vector3Int.one;
		public int LengthInt => Length.x * Length.y * Length.z;
		public Vector3Int FirstIndex => coordinatesOffset;
		public Vector3Int LastIndex => coordinatesOffset + Length - Vector3Int.one;
		public Layer(Vector3Int lowerLayerSize) {
			this.lowerLayerSize = lowerLayerSize;
		}
		public void Init() {
			created = new bool[Length.x, Length.y, Length.z];
			pendingsDestroy = new bool[Length.x, Length.y, Length.z];
			indexes = new int[Length.x, Length.y, Length.z];
			for (int z = 0; z < Length.z; z++)
				for (int y = 0; y < Length.y; y++)
					for (int x = 0; x < Length.x; x++) {
						indexes[x, y, z] = x + y * Length.x + z * Length.x * Length.y;
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

		public int GetIndex(Vector3Int location) {
			return indexes[location.x, location.y, location.z];
		}
        public bool GetPendingsDestroy(Vector3Int location) {
            return pendingsDestroy[location.x, location.y, location.z];
        }
        public bool GetCreated(Vector3Int location) {
            return created[location.x, location.y, location.z];
        }
        public int GetIndex(Vector3Int location, Layer layer) {
			return layer.GetIndex(layer.CoordinatesToLocation(LocationToCoordinates(location)));
		}
		public Vector3Int LocationToGlobalLocation(Vector3Int location) {
			return location - size;
		}
		public Vector3Int LocationToCoordinates(Vector3Int location) {
			return location + coordinatesOffset;
		}
		public Vector3Int CoordinatesToLocation(Vector3Int coordinates) {
			return coordinates - coordinatesOffset;
		}
		public Vector3Int LocationToLocation(Vector3Int location, Layer layer) {
			return layer.LocationToCoordinates(LocationToCoordinates(location));
		}
		public bool IsLocationOutOfBounds(Vector3Int location) {
			return location.x < 0 || location.y < 0 || location.z < 0 || location.x >= Length.x || location.y >= Length.y || location.z >= Length.z;
		}
	}
	[System.Serializable]
	public static class Layers {
		public static Vector3Int finallLayerSize = new Vector3Int(2,0,2); //fake lowerLayerSize above finalle layer
		public static Layer render = new Layer(new Vector3Int(1, 1, 1));
		public static Layer path = new Layer(new Vector3Int(0, 0, 0));
		public static Layer generation = new Layer(new Vector3Int(0, 0, 0));
		public static Layer[] hierarchy { get; private set; }
		static Layers() {
			hierarchy = new Layer[] { new Layer(new Vector3Int(0,0,0)), generation, path, render }; //atleast one item


			hierarchy[hierarchy.Length - 1].size = finallLayerSize;
			for (int layer = hierarchy.Length - 1; layer > 0; layer--) {
				hierarchy[layer - 1].size += hierarchy[layer].lowerLayerSize + hierarchy[layer].size;
			}
			Vector3Int size = hierarchy[0].size;
			hierarchy[0].Init();
			for (int layer = 1; layer < hierarchy.Length; layer++) {
				hierarchy[layer].coordinatesOffset = size - hierarchy[layer].size;
				hierarchy[layer].Init();
			}
		}
	}
}
