using System.Drawing;
using UnityEngine;

namespace Generation {
	public static class GenerationProp {
		public static Vector3 tileSize = new Vector3(3, 5, 3);
		public static float wallThickness = 0.2f;
		public static Vector3Int tileAmmount = new Vector3Int(3, 1, 3);
		public static Vector3Int chunkPathDistance = new Vector3Int(1, 0, 1);
		public static int mapPathDistanceInt { get { return (chunkPathDistance.x * 2 + 1) * (chunkPathDistance.y * 2 + 1) * (chunkPathDistance.z * 2 + 1); } }

		public static int seed;

		public static Transform transform;

		//calculations
		public static Vector3 chunkSize {
			get {
				return new Vector3(tileAmmount.x * tileSize.x, tileAmmount.y * tileSize.y, tileAmmount.z * tileSize.z);
			}
		}
		public static Vector3Int mapCenter {
			get {
				return (Layers.render.Length - Vector3Int.one) / 2;
			}
		}
		public static Vector3Int mapRenderDistantStart {
			get {
				return mapCenter - Layers.render.size;
			}
		}
		public static Vector3Int mapRenderDistantEnd {
			get {
				return mapCenter + Layers.render.size;
			}
		}
		public static int GetIndex(Vector3Int centerCoordinates, Vector3Int targetItemCoordinates, Vector3Int sizeFromCenter) {
			sizeFromCenter = sizeFromCenter * 2 + Vector3Int.one;

			Vector3Int targetCellLocation = targetItemCoordinates - centerCoordinates;

			return targetCellLocation.x
					+ (targetCellLocation.y * sizeFromCenter.x)
					+ (targetCellLocation.z * sizeFromCenter.x * sizeFromCenter.y);
		}
		public static Vector3Int GetTargetItemCoordinates(Vector3Int centerCoordinates, int locationIndex, Vector3Int sizeFromCenter) {
			sizeFromCenter = sizeFromCenter * 2 + Vector3Int.one;

			Vector3Int targetCellLocation = Vector3Int.zero;
			targetCellLocation.x = locationIndex % sizeFromCenter.x; //26 % 3 = 2
			targetCellLocation.y = (locationIndex / sizeFromCenter.x) % sizeFromCenter.y; //2 % 3 = 2
			targetCellLocation.z = locationIndex / (sizeFromCenter.x * sizeFromCenter.y); //26 / 9 = 2

			return targetCellLocation + centerCoordinates;
		}
		public static (Vector3Int outer, Vector3Int inner) FixOutOfBounds(Vector3Int outerCoordinates, Vector3Int innerCoordinates, Vector3Int innerBounds) {
			outerCoordinates += new Vector3Int(innerCoordinates.x / innerBounds.x, innerCoordinates.y / innerBounds.y, innerCoordinates.z / innerBounds.z);
			innerCoordinates -= new Vector3Int(innerCoordinates.x / innerBounds.x, innerCoordinates.y / innerBounds.y, innerCoordinates.z / innerBounds.z) * innerBounds;
			return (outerCoordinates, innerCoordinates);
		}
		public static bool GetSide(Vector3Int locationGeneration, Vector3Int side, Direction pos) {
			//swap back,down,left to front,up,right
			side += pos.Tile;
			if (side.x < 0) {
				locationGeneration.x--;
			}
			if (side.y < 0) {
				locationGeneration.y--;
			}
			if (side.z < 0) {
				locationGeneration.z--;
			}
			//locationGeneration += new Vector3Int(side.x / tileAmmount.x, side.y / tileAmmount.y, side.z / tileAmmount.z); <- TODO: what is this (╯°□°)╯︵ ┻━┻
			side.x = (side.x % tileAmmount.x + tileAmmount.x) % tileAmmount.x;
			side.y = (side.y % tileAmmount.y + tileAmmount.y) % tileAmmount.y;
			side.z = (side.z % tileAmmount.z + tileAmmount.z) % tileAmmount.z;
			return ChunkArray.sides[Layers.generation.GetIndex(locationGeneration), side.x, side.y, side.z, pos.Side];
		}
		public static bool GetSide(Vector3Int tileCoordinates, Direction direction) {
			tileCoordinates += direction.Tile;
            Vector3Int generationLocation = TileCoordinatesToCoordinates(tileCoordinates);
			Vector3Int tile = TileCoordinatesToTile(tileCoordinates);
            return ChunkArray.sides[Layers.generation.GetIndex(generationLocation), tile.x, tile.y, tile.z, direction.Side];
		}
		static void FixCoordinates(ref Vector3Int chunk, ref Vector3Int tile) {
			Vector3Int globalCoordinates = new Vector3Int(chunk.x * tileAmmount.x + tile.x, chunk.y * tileAmmount.y + tile.y, chunk.z * tileAmmount.z + tile.z);
			chunk = new Vector3Int(globalCoordinates.x / tileAmmount.x, globalCoordinates.y / tileAmmount.y, globalCoordinates.z / tileAmmount.z);
			tile = new Vector3Int(globalCoordinates.x % tileAmmount.x, globalCoordinates.y % tileAmmount.y, globalCoordinates.z % tileAmmount.z);
		}
		public static Vector3Int CoordinatesToTileCoordinates(Vector3Int coordinates) {
			return coordinates * tileAmmount;
		}
		public static Vector3Int TileCoordinatesToCoordinates(Vector3Int tileCoordinates) {
			return new Vector3Int(tileCoordinates.x / tileAmmount.x, tileCoordinates.y / tileAmmount.y, tileCoordinates.z / tileAmmount.z);
		}
		public static Vector3Int TileCoordinatesToTile(Vector3Int tileCoordinates) {
			return new Vector3Int(tileCoordinates.x % tileAmmount.x, tileCoordinates.y % tileAmmount.y, tileCoordinates.z % tileAmmount.z);
		}
        public static Vector3Int RealCoordinatesToTileCoordinates(Vector3 realCoordinates) {
            Vector3 distance = realCoordinates - Vector3.Scale(chunkSize, ChunkArray.coordinates) - (transform.position - (chunkSize / 2) - Vector3.Scale(chunkSize, Layers.generation.size));
            Vector3Int tileCoordinates = Vector3Int.zero;
            tileCoordinates.x = (int)(distance.x / tileSize.x);
            tileCoordinates.y = (int)(distance.y / tileSize.y);
            tileCoordinates.z = (int)(distance.z / tileSize.z);
            return tileCoordinates;
        }
        public static Vector3 TileCoordinatesToRealCoordinates(Vector3Int tileCoordinates) {
            Vector3 distance = new Vector3(tileCoordinates.x * tileSize.x,
                                           tileCoordinates.y * tileSize.y,
                                           tileCoordinates.z * tileSize.z);

            Vector3 realCoordinates = Vector3.Scale(chunkSize, ChunkArray.coordinates)
                                      + (transform.position - (chunkSize / 2) - Vector3.Scale(chunkSize, Layers.generation.size))
                                      + distance;

            return realCoordinates + (tileSize / 2);
        }

    }
}
