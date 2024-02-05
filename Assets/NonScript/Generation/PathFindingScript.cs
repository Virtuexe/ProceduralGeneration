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
		static TileCoordinates startTileCoordinates;
		static TileCoordinates endTileCoordinates;
#if UNITY_EDITOR
		static public GameEventsScript gameEvent;
#endif
		public static Set<TileCoordinates> FindPath(TileCoordinates startTileCoordinates, TileCoordinates endTileCoordinates) {
			if(startTileCoordinates == endTileCoordinates) {
				return new Set<TileCoordinates>();
			}
			nodeQueueIndexes.Clear();
			bestDistance = int.MaxValue;
			for (int i = 0; i < nodes.Length; i++) {
				nodes.array[i].distance = int.MaxValue;
				nodes.array[i].parentDirection = Vector3Int.zero;
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
			Set<TileCoordinates> bestPath = GetPath();
#if UNITY_EDITOR
			gameEvent.nodes = nodes;
			gameEvent.startTileCoordinate = startTileCoordinates;
			gameEvent.findGizmos = true;
			gameEvent.gizmosBestPath.Dispose();
			gameEvent.gizmosBestPath = new Set<TileCoordinates>(bestPath.array, bestPath.Length);
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
		private static Set<TileCoordinates> GetPath() {
			int Length = 2;
			TileCoordinates currentTileCoordinates = endTileCoordinates;
			Vector3Int lastDirection = nodes.array[GetIndex(endTileCoordinates)].parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
				currentTileCoordinates += new TileCoordinates(Vector3Int.zero, nodes.array[GetIndex(currentTileCoordinates)].parentDirection);
				if (lastDirection != nodes.array[GetIndex(currentTileCoordinates)].parentDirection) {
					Length++;
				}
				lastDirection = nodes.array[GetIndex(currentTileCoordinates)].parentDirection;
			}

			Pool<TileCoordinates> path = new Pool<TileCoordinates>(Length);
			//path.Add(endTileCoordinates);
			//currentTileCoordinates = endTileCoordinates;
			//lastDirection = nodes.array[GetIndex(endTileCoordinates)].parentDirection;
			//while (currentTileCoordinates != startTileCoordinates) {
			//	currentTileCoordinates.tile += nodes.array[GetIndex(currentTileCoordinates)].parentDirection;
			//	if (lastDirection != nodes.array[GetIndex(currentTileCoordinates)].parentDirection) {
			//		path.Add(currentTileCoordinates);
			//	}
			//	lastDirection = nodes.array[GetIndex(currentTileCoordinates)].parentDirection;
			//}
   //         path.Add(startTileCoordinates);
            return new Set<TileCoordinates>(path.array, Length);
        }
		private static void AddNodeToQueue(int index) {
			for (int queueIndex = nodeQueueIndexes.Count - 1; queueIndex >= 0; queueIndex--) {
				if (nodes.array[nodeQueueIndexes.array[queueIndex]].GetTotalCost() >= (*nodes[index]).GetTotalCost()) {
					nodeQueueIndexes.Insert(queueIndex + 1, index);
					return;
				}
			}
			nodeQueueIndexes.Insert(0,index);
		}
		private static bool TryMove(int sourceNodeIndex, Direction direction) {
			TileCoordinates targetTileCoordinates = nodes.array[sourceNodeIndex].tileCoordinates;
			targetTileCoordinates += new TileCoordinates(Vector3Int.zero, direction.RelValue);

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
			TileCoordinates targetTileCoordinates = nodes.array[sourceNodeIndex].tileCoordinates;
			targetTileCoordinates += new TileCoordinates(Vector3Int.zero, direction);

			int targetNodeIndex = GetIndex(targetTileCoordinates);
			if (NodeCheckMove(sourceNodeIndex, targetNodeIndex)) {
				return;
			}
			Move(sourceNodeIndex, direction, targetNodeIndex);
		}
		private static bool BoundsCheckMove(int sourceNodeIndex, Direction direction) {
			TileCoordinates targetTileCoordinates = nodes.array[sourceNodeIndex].tileCoordinates;
			targetTileCoordinates = new TileCoordinates(Vector3Int.zero, direction.RelValue);

			if (targetTileCoordinates.coordinates.x < 0 || targetTileCoordinates.coordinates.y < 0 || targetTileCoordinates.coordinates.z < 0) {
				return true;
			}
			if (Layers.generation.IsLocationOutOfBounds(targetTileCoordinates.coordinates) || GenerationProp.GetSide(nodes.array[sourceNodeIndex].tileCoordinates, direction)) {
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
			TileCoordinates targetTileCoordinates = nodes.array[nodeIndex].tileCoordinates;
			targetTileCoordinates.coordinates += direction;

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
		public static int GetIndex(TileCoordinates tileCoordinates) {
			int chunkIndex = Layers.generation.GetIndex(tileCoordinates.coordinates);
			return chunkIndex + Layers.generation.LengthInt * (tileCoordinates.tile.x + GenerationProp.tileAmmount.x * (tileCoordinates.tile.y + GenerationProp.tileAmmount.y * tileCoordinates.tile.z));
		}
		public struct Node {
			public Vector3Int parentDirection;
			public int distance;
			public TileCoordinates tileCoordinates;
			public int GetTotalCost() {
				TileCoordinates newTileCoordinates = tileCoordinates - endTileCoordinates;
				newTileCoordinates.Abs();
				return distance + newTileCoordinates.ToInt();
			}
		}
	}
}

