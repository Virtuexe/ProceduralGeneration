using Generation;
using MyArrays;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PathFinding {
	public static unsafe class PathFindingScript {
		private static Matrix<Node> nodes = new Matrix<Node>(Layers.generation.lengthInt, GenerationProp.tileAmount.x, GenerationProp.tileAmount.y, GenerationProp.tileAmount.z);
		private static int maxDistance = -1;
		private static int bestDistance;
		private static Pool<int> nodeQueueIndexes = new Pool<int>(Layers.generation.lengthInt * GenerationProp.tileAmount.x * GenerationProp.tileAmount.y * GenerationProp.tileAmount.z * Direction.Directions.Length);
		static TileCoordinates startTileCoordinates;
		static TileCoordinates endTileCoordinates;
#if UNITY_EDITOR
		static public GameEventsScript gameEvent;
#endif
		public static Pool<TileCoordinates> FindPath(TileCoordinates startTileCoordinates, TileCoordinates endTileCoordinates) {
            if (startTileCoordinates == endTileCoordinates) {
				return new Pool<TileCoordinates>();
			}
			nodeQueueIndexes.Clear();
			bestDistance = int.MaxValue;
			for (int i = 0; i < nodes.Length; i++) {
				nodes.buffer[i].distance = int.MaxValue;
				nodes.buffer[i].queueIndex = -1;
				nodes.buffer[i].parentDirection = Vector3Int.zero;
			}
			PathFindingScript.startTileCoordinates = startTileCoordinates;
			PathFindingScript.endTileCoordinates = endTileCoordinates;
			int startNodeIndex = GetIndex(startTileCoordinates);

			nodes.buffer[startNodeIndex].distance = 0;
			nodes.buffer[startNodeIndex].tileCoordinates = startTileCoordinates;

			for (int directionIndex = 0; directionIndex < Direction.Directions.Length; directionIndex++) {
				TryMove(startNodeIndex, Direction.Directions[directionIndex]);
			}
            ProcessQueue();
			Set<TileCoordinates> bestPath = GetPath();
#if UNITY_EDITOR
			gameEvent.nodes = nodes;
			gameEvent.startTileCoordinate = startTileCoordinates;
			gameEvent.findGizmos = true;
			gameEvent.gizmosBestPath.Free();
			gameEvent.gizmosBestPath = bestPath;
#endif
			return new Pool<TileCoordinates>(bestPath.Length, bestPath.buffer, bestPath.Length);
        }
		private static void ProcessQueue() {
			while (!nodeQueueIndexes.IsEmpty()) {
				int index = *nodeQueueIndexes.Last();
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
				for (int i = 0; i < nodeQueueIndexes.Count; i++) {
					for (int y = 0; y < nodeQueueIndexes.Count; y++) {
						if (nodeQueueIndexes.buffer[i] == nodeQueueIndexes.buffer[y]) {
							if (i == y)
								continue;
						}
					}
				}
			}
		}
		private static Set<TileCoordinates> GetPath() {
			int Length = 2;
			TileCoordinates currentTileCoordinates = endTileCoordinates;
			Vector3Int lastDirection = nodes.buffer[GetIndex(endTileCoordinates)].parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
				currentTileCoordinates += new TileCoordinates(Vector3Int.zero, nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection);
				if (lastDirection != nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection) {
					Length++;
				}
				lastDirection = nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection;
			}

			Pool<TileCoordinates> path = new Pool<TileCoordinates>(Length);
			path.Add(endTileCoordinates);
			currentTileCoordinates = endTileCoordinates;
			lastDirection = nodes.buffer[GetIndex(endTileCoordinates)].parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
				currentTileCoordinates += new TileCoordinates(Vector3Int.zero, nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection);
				if (lastDirection != nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection) {
					path.Add(currentTileCoordinates);
				}
				lastDirection = nodes.buffer[GetIndex(currentTileCoordinates)].parentDirection;
			}
			path.Add(startTileCoordinates);
			return new Set<TileCoordinates>(path.buffer, Length);
        }
		private static void AddNodeToQueue(int index) {
			for (int queueIndex = nodeQueueIndexes.Count - 1; queueIndex >= 0; queueIndex--) {
				int nodeQueueIndex = nodeQueueIndexes.buffer[queueIndex];
                if (nodes.buffer[nodeQueueIndex].GetTotalCost() >= nodes.buffer[index].GetTotalCost()) {
                    nodeQueueIndexes.Insert(queueIndex + 1, index);
                    return;
				}
			}
			nodeQueueIndexes.Insert(0,index);
		}
		private static bool TryMove(int sourceNodeIndex, Direction direction) {
			TileCoordinates targetTileCoordinates = nodes.buffer[sourceNodeIndex].tileCoordinates;
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
			TileCoordinates targetTileCoordinates = nodes.buffer[sourceNodeIndex].tileCoordinates;
			targetTileCoordinates += new TileCoordinates(Vector3Int.zero, direction);

			int targetNodeIndex = GetIndex(targetTileCoordinates);
			if (NodeCheckMove(sourceNodeIndex, targetNodeIndex)) {
				return;
			}
			Move(sourceNodeIndex, direction, targetNodeIndex);
		}
		private static bool BoundsCheckMove(int sourceNodeIndex, Direction direction) {
			TileCoordinates targetTileCoordinates = nodes.buffer[sourceNodeIndex].tileCoordinates;
			targetTileCoordinates += new TileCoordinates(Vector3Int.zero, direction.RelValue);

			if (Layers.generation.IsLocationOutOfBounds(Layers.generation.CoordinatesToLayerLocation(targetTileCoordinates.coordinates)) || 
				GenerationProp.GetSide(nodes.buffer[sourceNodeIndex].tileCoordinates, direction)) {
				return true;
			}
			return false;
		}
		private static bool NodeCheckMove(int sourceNodeIndex, int targetNodeIndex) {
			if (nodes[sourceNodeIndex]->GetTotalCost() >= bestDistance) {
				return true;
			}
			if (nodes[targetNodeIndex]->distance <= nodes[sourceNodeIndex]->distance) {
				return true;
			}
			return false;
		}
		private static void Move(int nodeIndex, Vector3Int direction, int targetNodeIndex) {
			TileCoordinates targetTileCoordinates = nodes.buffer[nodeIndex].tileCoordinates;
			targetTileCoordinates += new TileCoordinates(Vector3Int.zero, direction);

			(*nodes[targetNodeIndex]).parentDirection = -direction;
			(*nodes[targetNodeIndex]).distance = (*nodes[nodeIndex]).distance + 1;
			(*nodes[targetNodeIndex]).tileCoordinates = targetTileCoordinates;

			if (endTileCoordinates == targetTileCoordinates) {
				//Debug.Log("New path found: " + (*nodes[targetNodeIndex]).distance);
				bestDistance = (*nodes[targetNodeIndex]).distance;
				return;
			}

			AddNodeToQueue(targetNodeIndex);
		}
		public static int GetIndex(TileCoordinates tileCoordinates) {
			int chunkIndex = Layers.generation.CoordinatesToIndex(tileCoordinates.coordinates);
			return chunkIndex + Layers.generation.lengthInt * (tileCoordinates.tiles.x + GenerationProp.tileAmount.x * (tileCoordinates.tiles.y + GenerationProp.tileAmount.y * tileCoordinates.tiles.z));
		}
		public struct Node {
			public Vector3Int parentDirection;
			public int distance;
			public TileCoordinates tileCoordinates;
			public int queueIndex;
			public int GetTotalCost() {
                int distance = this.distance + TileCoordinates.Distance(tileCoordinates, endTileCoordinates);
				return distance;
			}
		}
	}
}

