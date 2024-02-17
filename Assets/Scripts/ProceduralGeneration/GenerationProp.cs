﻿using MyMath;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Generation {
	public static class GenerationProp {
		public static Vector3 tileSize = new Vector3(3, 5, 3);
		public static float wallThickness = 0.2f;
		public static Vector3Int tileAmmount = new Vector3Int(10, 1, 10);
		public static Vector3Int chunkPathDistance = new Vector3Int(1, 0, 1);
		public static int roomsMinCount = 1;
		public static int roomsMaxCount = 1;
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
			return ChunkArray.sides[Layers.generation.LayerLocationToIndex(locationGeneration), side.x, side.y, side.z, pos.Side];
		}
		public static bool GetSide(TileCoordinates tileCoordinates, Direction direction) {
			tileCoordinates += new TileCoordinates(Vector3Int.zero, direction.Tile);
			tileCoordinates.coordinates = Layers.generation.CoordinatesToLayerLocation(tileCoordinates.coordinates);

			return ChunkArray.sides[Layers.generation.LayerLocationToIndex(tileCoordinates.coordinates), tileCoordinates.tiles.x, tileCoordinates.tiles.y, tileCoordinates.tiles.z, direction.Side];
		}
		public static Vector3Int CoordinatesToTileCoordinates(Vector3Int coordinates) {
			return coordinates * tileAmmount;
		}
		public static Vector3Int TileCoordinatesToTile(Vector3Int tileCoordinates) {
			return new Vector3Int(tileCoordinates.x % tileAmmount.x, tileCoordinates.y % tileAmmount.y, tileCoordinates.z % tileAmmount.z);
		}
		public static TileCoordinates RealCoordinatesToTileCoordinates(Vector3 realCoordinates) {
			Vector3 mainChunkOrigin = transform.position - (chunkSize / 2);

			Vector3 distanceFromMainChunkOrigin = realCoordinates - mainChunkOrigin;

			Vector3Int coordinates = new Vector3Int(
				(int)Functions.ScaleDown(distanceFromMainChunkOrigin.x, chunkSize.x),
				(int)Functions.ScaleDown(distanceFromMainChunkOrigin.y, chunkSize.y),
				(int)Functions.ScaleDown(distanceFromMainChunkOrigin.z, chunkSize.z));

			Vector3 chunkOrigin = mainChunkOrigin + Vector3.Scale(chunkSize, coordinates);

			Vector3 distanceFromChunkOrigin = realCoordinates - chunkOrigin;

			Vector3Int tiles = new Vector3Int(
				(int)(distanceFromChunkOrigin.x / tileSize.x),
				(int)(distanceFromChunkOrigin.y / tileSize.y),
				(int)(distanceFromChunkOrigin.z / tileSize.z));

			TileCoordinates tileCoordinates = new TileCoordinates(coordinates, tiles);
			return tileCoordinates;
		}
		public static Vector3 TileCoordinatesToRealCoordinates(TileCoordinates tileCoordinates) {
			Vector3 chunkOrigin = transform.position - (chunkSize / 2) + 
				Vector3.Scale(chunkSize, new Vector3(tileCoordinates.coordinates.x, tileCoordinates.coordinates.y, tileCoordinates.coordinates.z));

			Vector3 realPositionWithinChunk = Vector3.Scale(tileSize, new Vector3(tileCoordinates.tiles.x, tileCoordinates.tiles.y, tileCoordinates.tiles.z));

			Vector3 realCoordinates = chunkOrigin + realPositionWithinChunk;
			return realCoordinates;
		}
	}
	public unsafe struct TileCoordinates {
		public Vector3Int coordinates;
		public Vector3Int tiles;
		public TileCoordinates(Vector3Int coordinates, Vector3Int tile) {
			this.coordinates = coordinates;
			this.tiles = tile;
			FixCoordinate();
		}
		public int ToInt() {
			Vector3Int location = ChunkArray.GetLocation(coordinates);
			return location.x * GenerationProp.tileAmmount.x +
				location.y * GenerationProp.tileAmmount.y +
				location.z * GenerationProp.tileAmmount.z + 
				tiles.x + 
				tiles.y + 
				tiles.z;
		}
		public void FixCoordinate() {
			coordinates += new Vector3Int(
				tiles.x / GenerationProp.tileAmmount.x + (tiles.x < 0 ? -1 : 0),
				tiles.y / GenerationProp.tileAmmount.y + (tiles.y < 0 ? -1 : 0),
				tiles.z / GenerationProp.tileAmmount.z + (tiles.z < 0 ? -1 : 0));


            tiles = new Vector3Int(
                (tiles.x % GenerationProp.tileAmmount.x + GenerationProp.tileAmmount.x) % GenerationProp.tileAmmount.x,
                (tiles.y % GenerationProp.tileAmmount.y + GenerationProp.tileAmmount.y) % GenerationProp.tileAmmount.y,
                (tiles.z % GenerationProp.tileAmmount.z + GenerationProp.tileAmmount.z) % GenerationProp.tileAmmount.z);
        }
		private static TileCoordinates selectedTileCoordinate;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TileCoordinates operator +(TileCoordinates left, TileCoordinates right) {
			selectedTileCoordinate = new TileCoordinates(left.coordinates + right.coordinates, left.tiles + right.tiles);
			FixSelectedCoordinate();
			return selectedTileCoordinate;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TileCoordinates operator -(TileCoordinates left, TileCoordinates right) {
			selectedTileCoordinate = new TileCoordinates(left.coordinates - right.coordinates, left.tiles - right.tiles);
			FixSelectedCoordinate();
			return selectedTileCoordinate;
		}
		public static int Distance(TileCoordinates left, TileCoordinates right) {
			return
				Math.Abs((left.coordinates.x * GenerationProp.tileAmmount.x + left.tiles.x) - (right.coordinates.x * GenerationProp.tileAmmount.x + right.tiles.x)) +
				Math.Abs((left.coordinates.y * GenerationProp.tileAmmount.y + left.tiles.y) - (right.coordinates.y * GenerationProp.tileAmmount.y + right.tiles.y)) +
				Math.Abs((left.coordinates.z * GenerationProp.tileAmmount.z + left.tiles.z) - (right.coordinates.z * GenerationProp.tileAmmount.z + right.tiles.z));
        }
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TileCoordinates left, TileCoordinates right) {
			return left.coordinates == right.coordinates && left.tiles == right.tiles;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TileCoordinates left, TileCoordinates right) {
			return left.coordinates != right.coordinates || left.tiles != right.tiles;
		}
		public static void FixSelectedCoordinate() {
            selectedTileCoordinate.coordinates += new Vector3Int(
                selectedTileCoordinate.tiles.x / GenerationProp.tileAmmount.x + (selectedTileCoordinate.tiles.x < 0 ? -1 : 0),
                selectedTileCoordinate.tiles.y / GenerationProp.tileAmmount.y + (selectedTileCoordinate.tiles.y < 0 ? -1 : 0),
                selectedTileCoordinate.tiles.z / GenerationProp.tileAmmount.z + (selectedTileCoordinate.tiles.z < 0 ? -1 : 0));


            selectedTileCoordinate.tiles = new Vector3Int(
                (selectedTileCoordinate.tiles.x % GenerationProp.tileAmmount.x + GenerationProp.tileAmmount.x) % GenerationProp.tileAmmount.x,
                (selectedTileCoordinate.tiles.y % GenerationProp.tileAmmount.y + GenerationProp.tileAmmount.y) % GenerationProp.tileAmmount.y,
                (selectedTileCoordinate.tiles.z % GenerationProp.tileAmmount.z + GenerationProp.tileAmmount.z) % GenerationProp.tileAmmount.z);
        }
		public override string ToString() {
			return "[coordinates:" + coordinates + ",tile:" + tiles + "]";
		}
	}
}
