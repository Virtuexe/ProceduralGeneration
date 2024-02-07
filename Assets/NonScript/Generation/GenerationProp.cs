using CustomVariables;
using MyArrays;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Generation {
	public static class GenerationProp {
		public static Vector3 tileSize = new Vector3(3, 5, 3);
		public static float wallThickness = 0.2f;
		public static Vector3Int tileAmmount = new Vector3Int(10, 1, 10);
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
		public static bool GetSide(TileCoordinates tileCoordinates, Direction direction) {
			tileCoordinates.tile += new TileCoordinates(Vector3Int.zero, direction.Tile);
			return ChunkArray.sides[Layers.generation.GetIndex(tileCoordinates.coordinates), tileCoordinates.tile.x, tileCoordinates.tile.y, tileCoordinates.tile.z, direction.Side];
		}
		static void FixCoordinates(ref Vector3Int chunk, ref Vector3Int tile) {
			Vector3Int globalCoordinates = new Vector3Int(chunk.x * tileAmmount.x + tile.x, chunk.y * tileAmmount.y + tile.y, chunk.z * tileAmmount.z + tile.z);
			chunk = new Vector3Int(globalCoordinates.x / tileAmmount.x, globalCoordinates.y / tileAmmount.y, globalCoordinates.z / tileAmmount.z);
			tile = new Vector3Int(globalCoordinates.x % tileAmmount.x, globalCoordinates.y % tileAmmount.y, globalCoordinates.z % tileAmmount.z);
		}
		public static Vector3Int CoordinatesToTileCoordinates(Vector3Int coordinates) {
			return coordinates * tileAmmount;
		}
		public static Vector3Int TileCoordinatesToTile(Vector3Int tileCoordinates) {
			return new Vector3Int(tileCoordinates.x % tileAmmount.x, tileCoordinates.y % tileAmmount.y, tileCoordinates.z % tileAmmount.z);
		}
		public static TileCoordinates RealCoordinatesToTileCoordinates(Vector3 realCoordinates) {
			Vector3 distance = realCoordinates - Vector3.Scale(chunkSize, ChunkArray.coordinates) - (transform.position - (chunkSize / 2) - Vector3.Scale(chunkSize, Layers.generation.size));
			TileCoordinates tileCoordinates = new TileCoordinates();
            tileCoordinates.coordinates.x = (int)(distance.x / chunkSize.x);
            tileCoordinates.coordinates.y = (int)(distance.y / chunkSize.y);
            tileCoordinates.coordinates.z = (int)(distance.z / chunkSize.z);
            tileCoordinates.tile.x = (int)(distance.x / tileSize.x) % tileAmmount.x;
			tileCoordinates.tile.y = (int)(distance.y / tileSize.y) % tileAmmount.y;
			tileCoordinates.tile.z = (int)(distance.z / tileSize.z) % tileAmmount.z;
			return tileCoordinates;
		}
		public static Vector3 TileCoordinatesToRealCoordinates(TileCoordinates tileCoordinates) {
			Vector3 chunkOrigin = Vector3.Scale(chunkSize, ChunkArray.coordinates) + (transform.position - (chunkSize / 2) - Vector3.Scale(chunkSize, Layers.generation.size));

			Vector3 tileInChunkPosition = new Vector3(
				tileCoordinates.coordinates.x * chunkSize.x + tileCoordinates.tile.x.number * tileSize.x + tileSize.x / 2,
				tileCoordinates.coordinates.y * chunkSize.y + tileCoordinates.tile.y.number * tileSize.y + tileSize.y / 2,
				tileCoordinates.coordinates.z * chunkSize.z + tileCoordinates.tile.z.number * tileSize.z + tileSize.z / 2
			);

			Vector3 realCoordinates = chunkOrigin + tileInChunkPosition;

			return realCoordinates;
        }
	}
	public unsafe struct TileCoordinates {
		public Vector3<int> coordinates;
		public Vector3<EntangledPInt> tile;
        public TileCoordinates(Vector3Int coordinates, Vector3Int tile) {
			this.coordinates = new Vector3<int>(coordinates.x, coordinates.y, coordinates.z);
			fixed(Vector3<int>* p = &this.coordinates)
			this.tile = new Vector3<EntangledPInt>(
				new EntangledPInt(GenerationProp.tileAmmount.x, &p->x),
				new EntangledPInt(GenerationProp.tileAmmount.y, &p->x),
				new EntangledPInt(GenerationProp.tileAmmount.z, &p->x));
		}
        public override string ToString() {
			return "[coordinates:" + coordinates + ",tile:" + tile + "]";
		}
	}
}
