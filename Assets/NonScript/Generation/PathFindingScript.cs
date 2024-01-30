using Generation;
using MyArrays;
using System;
using UnityEngine;

namespace PathFinding {
	public static unsafe class PathFindingScript {
		private static Matrix<Node> nodes = new Matrix<Node>(Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z);
		private static int maxDistance = -1;
		private static int bestDistance;
		private static Pool<int> nodeQueueIndexes = new Pool<int>(Layers.generation.LengthInt * GenerationProp.tileAmmount.x * GenerationProp.tileAmmount.y * GenerationProp.tileAmmount.z);
		/*
        private static int[] nodeQueueIndexes = new int[
            (Layers.generation.Length.y * GenerationProp.tileAmmount.y * Layers.generation.Length.z * GenerationProp.tileAmmount.z) +
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x * Layers.generation.Length.z * GenerationProp.tileAmmount.z) +
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x * Layers.generation.Length.y * GenerationProp.tileAmmount.y) -
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x + Layers.generation.Length.y * GenerationProp.tileAmmount.y + 1) * 2
            ];
        */
		//static PathFindingScript() {
		//	for (int i = 0; i < nodes.Length; i++) {
		//		nodes[i].testGoResult = new Set<bool>(Direction.Directions.Length);
		//	}
		//}
		static Vector3Int startTileCoordinates;
		static Vector3Int endTileCoordinates;
#if UNITY_EDITOR
		static public GameEventsScript gameEvent;
#endif
		public static Set<Vector3Int> FindPath(Vector3Int startTileCoordinates, Vector3Int endTileCoordinates) {
			nodeQueueIndexes.Clear();
			bestDistance = int.MaxValue;
			for (int i = 0; i < nodes.Length; i++) {
				(*nodes[i]).distance = int.MaxValue;
				(*nodes[i]).parentDirection = Vector3Int.zero;
				//nodes[i].testGoResult.Clear();
			}
			PathFindingScript.startTileCoordinates = startTileCoordinates;
			PathFindingScript.endTileCoordinates = endTileCoordinates;
			int startNodeIndex = GetIndex(startTileCoordinates);

			(*nodes[startNodeIndex]).distance = 0;
			(*nodes[startNodeIndex]).tileCoordinates = startTileCoordinates;

			for (int directionIndex = 0; directionIndex < Direction.Directions.Length; directionIndex++) {
				TryMove(startNodeIndex, Direction.Directions[directionIndex]);
			}
			ProcessQueue();
			Set<Vector3Int> bestPath = GetPath();
#if UNITY_EDITOR
			gameEvent.nodes = nodes;
			gameEvent.startTileCoordinate = startTileCoordinates;
			gameEvent.findGizmos = true;
			gameEvent.gizmosBestPath.Dispose();
			gameEvent.gizmosBestPath = new Set<Vector3Int>(bestPath.array, bestPath.Length);
#endif
			return bestPath;
		}
		private static void ProcessQueue() {
			while (!nodeQueueIndexes.IsEmpty()) {
				int index = nodeQueueIndexes.Last();
				nodeQueueIndexes.Remove();
				if ((*nodes[index]).distance == maxDistance) {
					continue;
				}
				for (int i = 0; i < Direction.Directions.Length; i++) {
					if (Direction.Directions[i].RelValue == (*nodes[index]).parentDirection) {
						continue;
					}
					TryMove(index, Direction.Directions[i]);
				}

			}
		}
		private static Set<Vector3Int> GetPath() {
			int Length = 2;
			Vector3Int currentTileCoordinates = endTileCoordinates;
			Vector3Int lastDirection = (*nodes[GetIndex(endTileCoordinates)]).parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
				currentTileCoordinates += (*nodes[GetIndex(currentTileCoordinates)]).parentDirection;
				if (lastDirection != (*nodes[GetIndex(currentTileCoordinates)]).parentDirection) {
					Length++;
				}
				lastDirection = (*nodes[GetIndex(currentTileCoordinates)]).parentDirection;
			}

			Pool<Vector3Int> path = new Pool<Vector3Int>(Length);
			path.Add(endTileCoordinates);
			currentTileCoordinates = endTileCoordinates;
			lastDirection = (*nodes[GetIndex(endTileCoordinates)]).parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
				currentTileCoordinates += (*nodes[GetIndex(currentTileCoordinates)]).parentDirection;
				if (lastDirection != (*nodes[GetIndex(currentTileCoordinates)]).parentDirection) {
					path.Add(currentTileCoordinates);
				}
				lastDirection = (*nodes[GetIndex(currentTileCoordinates)]).parentDirection;
			}
            path.Add(startTileCoordinates);
            return new Set<Vector3Int>(path.array, Length);
        }
		private static void AddNodeToQueue(int index) {
			for (int queueIndex = nodeQueueIndexes.Count - 1; queueIndex >= 0; queueIndex--) {
				if ((*nodes[*nodeQueueIndexes[queueIndex]]).GetTotalCost() >= (*nodes[index]).GetTotalCost()) {
					nodeQueueIndexes.Insert(queueIndex + 1, index);
					return;
				}
			}
			nodeQueueIndexes.Insert(0,index);
		}
		private static bool TryMove(int sourceNodeIndex, Direction direction) {
			Vector3Int targetTileCoordinates = (*nodes[sourceNodeIndex]).tileCoordinates + direction.RelValue;
			if (BoundsCheckMove(sourceNodeIndex, direction)) {
				return false;
			}
			int targetNodeIndex = GetIndex(targetTileCoordinates);
			if (NodeCheckMove(sourceNodeIndex, targetNodeIndex)) {
				return false;
			}
			Move(sourceNodeIndex, direction.RelValue, targetNodeIndex);
			return true;
		}
		private static void QuickTryMove(int sourceNodeIndex, Vector3Int direction) {
			Vector3Int targetTileCoordinates = (*nodes[sourceNodeIndex]).tileCoordinates + direction;
			int targetNodeIndex = GetIndex(targetTileCoordinates);
			if (NodeCheckMove(sourceNodeIndex, targetNodeIndex)) {
				return;
			}
			Move(sourceNodeIndex, direction, targetNodeIndex);
		}
		private static bool BoundsCheckMove(int sourceNodeIndex, Direction direction) {
			Vector3Int targetTileCoordinates = (*nodes[sourceNodeIndex]).tileCoordinates + direction.RelValue;
			if (targetTileCoordinates.x < 0 || targetTileCoordinates.y < 0 || targetTileCoordinates.z < 0) {
				return true;
			}
			if (Layers.generation.IsLocationOutOfBounds(GenerationProp.TileCoordinatesToCoordinates(targetTileCoordinates)) || GenerationProp.GetSide((*nodes[sourceNodeIndex]).tileCoordinates, direction)) {
				return true;
			}
			return false;
		}
		private static bool NodeCheckMove(int sourceNodeIndex, int targetNodeIndex) {
			if ((*nodes[sourceNodeIndex]).GetTotalCost() >= bestDistance) {
				return true;
			}
			if ((*nodes[targetNodeIndex]).distance <= (*nodes[sourceNodeIndex]).distance) {
				return true;
			}
			return false;
		}
		private static void Move(int nodeIndex, Vector3Int direction, int targetNodeIndex) {
			Vector3Int targetTileCoordinates = (*nodes[nodeIndex]).tileCoordinates + direction;

			(*nodes[targetNodeIndex]).parentDirection = -direction;
			(*nodes[targetNodeIndex]).distance = (*nodes[nodeIndex]).distance + 1;
			(*nodes[targetNodeIndex]).tileCoordinates = targetTileCoordinates;

			if (endTileCoordinates == targetTileCoordinates) {
				Debug.Log("New path found: " + (*nodes[targetNodeIndex]).distance);
				bestDistance = (*nodes[targetNodeIndex]).distance;
				return;
			}

			AddNodeToQueue(targetNodeIndex);
		}
		public static int GetIndex(Vector3Int TileCoordinates) {
			Vector3Int coordinates = GenerationProp.TileCoordinatesToCoordinates(TileCoordinates);
			int chunkIndex = Layers.generation.GetIndex(coordinates);
			Vector3Int tile = GenerationProp.TileCoordinatesToTile(TileCoordinates);
			return chunkIndex + Layers.generation.LengthInt * (tile.x + GenerationProp.tileAmmount.x * (tile.y + GenerationProp.tileAmmount.y * tile.z));
		}
		public struct Node {
			public Vector3Int parentDirection;
			public int distance;
			public Vector3Int tileCoordinates;
			public int GetTotalCost() {
				return distance + Math.Abs(tileCoordinates.x - endTileCoordinates.x) + Math.Abs(tileCoordinates.y - endTileCoordinates.y) + Math.Abs(tileCoordinates.z - endTileCoordinates.z);
			}
		}
	}
}

