using System;
using UnityEngine;
using PathFinfing;
namespace Generation {
	public static class ChunkArray {
		public static bool[,,,,] sides = new bool[Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z, 3];
		public static bool[,,,] grid = new bool[Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z];
        public static Node[] nodes = new Node[Layers.generation.LengthInt * GenerationProp.tileAmmount.x * GenerationProp.tileAmmount.y * GenerationProp.tileAmmount.z];
        public static bool[] genereted = new bool[Layers.generation.LengthInt];
        public static GameObject[] gameObject = new GameObject[Layers.render.LengthInt];

		//[chunk,x,y,z,chunk2,x2,y2,z2]
		//public static Path[,,,,,,,] paths;

		public static Vector3Int coordinates;
		public static Vector3Int GetCoordinates(Vector3Int location) {
			return location + ChunkArray.coordinates - Layers.hierarchy[0].size;
		}
		public static Vector3Int GetLocation(Vector3Int coordinates) {
			return coordinates - ChunkArray.coordinates;
		}
		public static void MoveChunks(Vector3Int distanceToMove) {
			Debug.Log("moving: " + distanceToMove);
			coordinates += distanceToMove;
			distanceToMove = -distanceToMove;
			int sign;
			int length;
			Vector3Int shiftingLocation = Vector3Int.zero;
			Vector3Int prependingLocation = Vector3Int.zero;
			Vector3Int direction = Vector3Int.zero;

			Vector3Int location = Vector3Int.zero;
			for (int layer = 0; layer < Layers.hierarchy.Length; layer++) {
				for (int dimension = 0; dimension <= 2; dimension++) {
					length = Layers.hierarchy[layer].Length[dimension];
					sign = (distanceToMove[dimension] >> 31) | 1;
					shiftingLocation[dimension] = (sign + 1) / 2 * (length - 1);                            // 0 for negative, length - 1 for non-negative
					prependingLocation[dimension] = (1 - sign) / 2 * length + (sign + 1) / 2 * -1;          // length for negative, -1 for non-negative
					direction[dimension] = sign;                                                            // -1 for negative, 1 for non-negative
				}
				for (location.x = shiftingLocation.x; location.x != prependingLocation.x; location.x -= direction.x) {
					for (location.y = shiftingLocation.y; location.y != prependingLocation.y; location.y -= direction.y) {
						for (location.z = shiftingLocation.z; location.z != prependingLocation.z; location.z -= direction.z) {
							if (Layers.hierarchy[layer].IsLocationIncluded(location + distanceToMove)) {
								Layers.hierarchy[layer].MoveChunk(location, location + distanceToMove);
							}
							else {
								Layers.hierarchy[layer].pendingsDestroy[location.x,location.y,location.z] = true;
                                Layers.hierarchy[layer].created[location.x, location.y, location.z] = false;
                            }
							//if (!Layers.hierarchy[layer].IsLocationIncluded(location - distanceToMove)) {
							//	if (!Layers.hierarchy[layer].pendingsDestroy[location.x, location.y, location.z])
							//		Debug.Log("IM NOT USELESS!!!");
								
							//}
						}
					}
				}
			}
		}
	}
}


