using System;
using UnityEngine;
using PathFinding;
using MyArrays;
using System.Collections.Generic;

namespace Generation {
	public static class ChunkArray {
		//generation detail
		public static int[] roomsAmount = new int[Layers.generationDetail.lengthInt];
		public static Vector3Int[,] roomCenters = new Vector3Int[Layers.generationDetail.lengthInt, GenerationProp.roomCount.max];
		public static Vector3Int[,] roomSizes = new Vector3Int[Layers.generationDetail.lengthInt, GenerationProp.roomCount.max];
		//generation
		public static bool[,,,,] sides = new bool[Layers.generation.lengthInt, GenerationProp.tileAmount.x, GenerationProp.tileAmount.y, GenerationProp.tileAmount.z, 3];
		public static bool[,,,] accesible = new bool[Layers.generation.lengthInt, GenerationProp.tileAmount.x, GenerationProp.tileAmount.y, GenerationProp.tileAmount.z];
		public static bool[] genereted = new bool[Layers.generation.lengthInt];
		//render
		public static GameObject[] gameObject = new GameObject[Layers.render.lengthInt];
		public static Dictionary<Vector3Int, int[]> keysTaken = new Dictionary<Vector3Int, int[]>();

		public static Vector3Int coordinates;
		static ChunkArray() {
			GameEventsScript.GameEnd += End;
		}
		private static void End() {
			keysTaken.Clear();
		}
		public static Vector3Int GetCoordinates(Vector3Int location) {
			return location + ChunkArray.coordinates - Layers.hierarchy[0].size;
		}
		public static Vector3Int GetLocation(Vector3Int coordinates) {
			return coordinates - ChunkArray.coordinates;
		}
		public static void MoveChunks(Vector3Int distanceToMove) {
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
					length = Layers.hierarchy[layer].length[dimension];
					sign = (distanceToMove[dimension] >> 31) | 1;
					shiftingLocation[dimension] = (sign + 1) / 2 * (length - 1);                            // 0 for negative, length - 1 for non-negative
					prependingLocation[dimension] = (1 - sign) / 2 * length + (sign + 1) / 2 * -1;          // length for negative, -1 for non-negative
					direction[dimension] = sign;                                                            // -1 for negative, 1 for non-negative
				}
				for (location.x = shiftingLocation.x; location.x != prependingLocation.x; location.x -= direction.x) {
					for (location.y = shiftingLocation.y; location.y != prependingLocation.y; location.y -= direction.y) {
						for (location.z = shiftingLocation.z; location.z != prependingLocation.z; location.z -= direction.z) {
							if (!Layers.hierarchy[layer].IsLocationOutOfBounds(location + distanceToMove)) {
								Layers.hierarchy[layer].MoveChunk(location, location + distanceToMove);
							} else {
								Layers.hierarchy[layer].pendingsDestroy[location.x, location.y, location.z] = true;
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


