using Generation;
using MyArrays;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace PathFinding{
    public static class PathFindingScript {
        private static Matrix<Node> nodes = new Matrix<Node>(Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z);
        private static int maxDistance = 5;
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

        static Vector3Int startTileCoordinates;
        static Vector3Int endTileCoordinates;
        public static Pool<Vector3Int> FindPath(Vector3Int startTileCoordinates, Vector3Int endTileCoordinates) {
            nodeQueueIndexes.Clear();
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = new Node(int.MaxValue);
            }
            PathFindingScript.startTileCoordinates = startTileCoordinates;
            PathFindingScript.endTileCoordinates = endTileCoordinates;
            int startNodeIndex = GetIndex(startTileCoordinates);
            nodes[startNodeIndex] = new Node(0, startTileCoordinates);
            for (int i = 0; i < Direction.Directions.Length; i++) {
                TryGo(startNodeIndex, Direction.Directions[i]);
            }
            ProcessQueue();
            return GetPath();
        }
        private static void ProcessQueue() {
            while (!nodeQueueIndexes.IsEmpty()) {
                int index = nodeQueueIndexes.Last();
                nodeQueueIndexes.Remove();
                if (nodes[index].distance == maxDistance) {
                    continue;
                }
                for (int i = 0; i < Direction.Directions.Length; i++) {
                    if (Direction.Directions[i] == nodes[index].parentDirection) {
                        continue;
                    }
                    TryGo(index, Direction.Directions[i]);
                }
                
            }
        }
        private static Pool<Vector3Int> GetPath() {
            int maxPoolLength = 0;
            Vector3Int currentTileCoordinates = endTileCoordinates;
			Direction lastDirection = nodes[GetIndex(endTileCoordinates)].parentDirection;
			while (currentTileCoordinates != startTileCoordinates) {
                if (lastDirection != nodes[GetIndex(currentTileCoordinates)].parentDirection) {
                    maxPoolLength++;
                }
                lastDirection = nodes[GetIndex(currentTileCoordinates)].parentDirection;
                currentTileCoordinates += nodes[GetIndex(currentTileCoordinates)].parentDirection.RelValue;
			}
			currentTileCoordinates = endTileCoordinates;
			lastDirection = nodes[GetIndex(endTileCoordinates)].parentDirection;
            Pool<Vector3Int> Path = new Pool<Vector3Int>(maxPoolLength);
			while (currentTileCoordinates != startTileCoordinates) {
				if (lastDirection != nodes[GetIndex(currentTileCoordinates)].parentDirection) {
					maxPoolLength++;
				}
				currentTileCoordinates += nodes[GetIndex(currentTileCoordinates)].parentDirection.RelValue;
			}
            Path.Dispose();
		    return new Pool<Vector3Int>();
        }
        private static void AddNodeToQueue(int index) {
            for (int queueIndex = nodeQueueIndexes.Count - 1; queueIndex >= 0; queueIndex--) {
                if (nodes[nodeQueueIndexes[queueIndex]].GetTotalCost() >= nodes[index].GetTotalCost()) {
                    nodeQueueIndexes.Insert(queueIndex, index);
                    return;
                }
            }
            nodeQueueIndexes.Add(index);
        }
        private static void TryGo(int nodeIndex, Direction direction) {
            if (nodes[nodeIndex].tileCoordinates.x + direction.RelValue.x < 0 || nodes[nodeIndex].tileCoordinates.y + direction.RelValue.y < 0 || nodes[nodeIndex].tileCoordinates.z + direction.RelValue.z < 0) {
                return;
            }
            if (Layers.generation.IsLocationOutOfBounds(GenerationProp.TileCoordinatesToCoordinates(nodes[nodeIndex].tileCoordinates + direction.RelValue)) || GenerationProp.GetSide(nodes[nodeIndex].tileCoordinates, direction)) {
                return;
            }
            int targetNodeIndex = GetIndex(nodes[nodeIndex].tileCoordinates + direction.RelValue);
            if (nodes[targetNodeIndex].distance <= nodes[nodeIndex].distance) {
                return;
            }
            nodes[targetNodeIndex] = new Node(new Direction(-direction.RelValue), nodes[nodeIndex].distance + 1, nodes[nodeIndex].tileCoordinates + direction.RelValue);
            if (endTileCoordinates == nodes[nodeIndex].tileCoordinates + direction.RelValue) {
                Debug.Log("new path found: " + nodes[targetNodeIndex].distance);
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
            public Direction parentDirection { get; private set; }
            public int distance { get; private set; }
            public Vector3Int tileCoordinates { get; private set; }
            public Node(Direction parentDirection, int distance, Vector3Int tileCoordinates) {
                this.parentDirection = parentDirection;
                this.distance = distance;
                this.tileCoordinates = tileCoordinates;
            }
            public Node(int distance, Vector3Int tileCoordinates) {
                this.parentDirection = new Direction();
                this.distance = distance;
                this.tileCoordinates = tileCoordinates;
            }
            public Node(int distance) {
                this.parentDirection = new Direction();
                this.distance = distance;
                this.tileCoordinates = Vector3Int.zero;
            }
            public Node(Vector3Int tileCoordinates) {
                this.parentDirection = new Direction();
                this.distance = 0;
                this.tileCoordinates = tileCoordinates;
            }
            public int GetTotalCost() {
                return distance + Math.Abs(tileCoordinates.x - endTileCoordinates.x) + Math.Abs(tileCoordinates.y - endTileCoordinates.y) + Math.Abs(tileCoordinates.z - endTileCoordinates.z);
            }
        }
    }
}

