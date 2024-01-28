using Generation;
using MyArrays;
using System;
using UnityEngine;

namespace PathFinding {
    public static class PathFindingScript {
        private static Matrix<Node> nodes = new Matrix<Node>(Layers.generation.LengthInt, GenerationProp.tileAmmount.x, GenerationProp.tileAmmount.y, GenerationProp.tileAmmount.z);
        private static int maxDistance = 10;
        private static int bestDistance;
        private static Pool<int> nodeQueueIndexes = new Pool<int>(Layers.generation.LengthInt * GenerationProp.tileAmmount.x * GenerationProp.tileAmmount.y * GenerationProp.tileAmmount.z);
        private static bool[] testGoResult = new bool[Direction.Directions.Length];
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
#if UNITY_EDITOR
        static public GameEventsScript gameEvent;
#endif
        public static Vector3Int[] FindPath(Vector3Int startTileCoordinates, Vector3Int endTileCoordinates) {
            nodeQueueIndexes.Clear();
            bestDistance = int.MaxValue;
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = new Node(int.MaxValue);
            }
            PathFindingScript.startTileCoordinates = startTileCoordinates;
            PathFindingScript.endTileCoordinates = endTileCoordinates;
            int startNodeIndex = GetIndex(startTileCoordinates);
            nodes[startNodeIndex] = new Node(0, startTileCoordinates);
            for (int directionIndex = 0; directionIndex < Direction.Directions.Length; directionIndex++) {
                testGoResult[directionIndex] = TryMove(startNodeIndex, Direction.Directions[directionIndex]);
                if (!testGoResult[directionIndex]) {
                    continue;
                }
                for (int secondDirectionIndex = 0; secondDirectionIndex < Direction.Directions.Length; secondDirectionIndex++) {
                    if (Direction.Directions[directionIndex].Value == Direction.Directions[secondDirectionIndex].Value) {
                        continue;
                    }
                    if (testGoResult[Direction.Directions[secondDirectionIndex].Index]) {
                        Vector3Int targetTileCoordinates = nodes[startNodeIndex].tileCoordinates + Direction.Directions[directionIndex].RelValue + Direction.Directions[secondDirectionIndex].RelValue;
                        int targetNodeIndex = GetIndex(targetTileCoordinates);
                        QuickTryMove(startNodeIndex, Direction.Directions[directionIndex].RelValue + Direction.Directions[secondDirectionIndex].RelValue);
                    }
                }
            }
            ProcessQueue();
#if UNITY_EDITOR
            gameEvent.nodes = nodes;
            gameEvent.startTileCoordinate = startTileCoordinates;
            gameEvent.findGizmos = true;
#endif
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
                    if (Direction.Directions[i].RelValue == nodes[index].parentDirection) {
                        continue;
                    }
                    TryMove(index, Direction.Directions[i]);
                }
                
            }
        }
        private static Vector3Int[] GetPath() {
            //int maxPoolLength = 0;
            //Vector3Int currentTileCoordinates = endTileCoordinates;
            //Vector3Int lastDirection = nodes[GetIndex(endTileCoordinates)].parentDirection;
            //while (currentTileCoordinates != startTileCoordinates) {
            //    if (lastDirection != nodes[GetIndex(currentTileCoordinates)].parentDirection) {
            //        maxPoolLength++;
            //    }
            //    lastDirection = nodes[GetIndex(currentTileCoordinates)].parentDirection;
            //    currentTileCoordinates += nodes[GetIndex(currentTileCoordinates)].parentDirection;
            //}
            //currentTileCoordinates = endTileCoordinates;
            //lastDirection = nodes[GetIndex(endTileCoordinates)].parentDirection;
            //Pool<Vector3Int> Path = new Pool<Vector3Int>(maxPoolLength);
            //while (currentTileCoordinates != startTileCoordinates) {
            //    if (lastDirection != nodes[GetIndex(currentTileCoordinates)].parentDirection) {
            //        maxPoolLength++;
            //    }
            //    currentTileCoordinates += nodes[GetIndex(currentTileCoordinates)].parentDirection;
            //}
            //Path.Dispose();
            return new Vector3Int[0];
        }
        private static void AddNodeToQueue(int index) {
            for (int queueIndex = nodeQueueIndexes.Count - 1; queueIndex >= 0; queueIndex--) {
                if (nodes[nodeQueueIndexes[queueIndex]].GetTotalCost() >= nodes[index].GetTotalCost()) {
                    nodeQueueIndexes.Insert(queueIndex + 1, index);
                    return;
                }
            }
            nodeQueueIndexes.Insert(0,index);
        }
        private static bool TryMove(int sourceNodeIndex, Direction direction) {
            Vector3Int targetTileCoordinates = nodes[sourceNodeIndex].tileCoordinates + direction.RelValue;
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
            Vector3Int targetTileCoordinates = nodes[sourceNodeIndex].tileCoordinates + direction;
            int targetNodeIndex = GetIndex(targetTileCoordinates);
            if (NodeCheckMove(sourceNodeIndex, targetNodeIndex)) {
                return;
            }
            Move(sourceNodeIndex, direction, targetNodeIndex);
        }
        private static bool BoundsCheckMove(int sourceNodeIndex, Direction direction) {
            Vector3Int targetTileCoordinates = nodes[sourceNodeIndex].tileCoordinates + direction.RelValue;
            if (targetTileCoordinates.x < 0 || targetTileCoordinates.y < 0 || targetTileCoordinates.z < 0) {
                return true;
            }
            if (Layers.generation.IsLocationOutOfBounds(GenerationProp.TileCoordinatesToCoordinates(targetTileCoordinates)) || GenerationProp.GetSide(nodes[sourceNodeIndex].tileCoordinates, direction)) {
                return true;
            }
            return false;
        }
        private static bool NodeCheckMove(int sourceNodeIndex, int targetNodeIndex) {
            if (nodes[sourceNodeIndex].GetTotalCost() > bestDistance) {
                return true;
            }
            if (nodes[targetNodeIndex].distance <= nodes[sourceNodeIndex].distance) {
                return true;
            }
            return false;
        }
        private static void Move(int nodeIndex, Vector3Int direction, int targetNodeIndex) {
            Vector3Int targetTileCoordinates = nodes[nodeIndex].tileCoordinates + direction;

            nodes[targetNodeIndex] = new Node(-direction, nodes[nodeIndex].distance + 1, targetTileCoordinates);

            if (endTileCoordinates == targetTileCoordinates) {
                Debug.Log("New path found: " + nodes[targetNodeIndex].distance);
                bestDistance = nodes[targetNodeIndex].distance;
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
            public Node(Vector3Int parentDirection, int distance, Vector3Int tileCoordinates) {
                this.parentDirection = parentDirection;
                this.distance = distance;
                this.tileCoordinates = tileCoordinates;
            }
            public Node(int distance, Vector3Int tileCoordinates) {
                this.parentDirection = new Vector3Int();
                this.distance = distance;
                this.tileCoordinates = tileCoordinates;
            }
            public Node(int distance) {
                this.parentDirection = new Vector3Int();
                this.distance = distance;
                this.tileCoordinates = Vector3Int.zero;
            }
            public Node(Vector3Int tileCoordinates) {
                this.parentDirection = new Vector3Int();
                this.distance = 0;
                this.tileCoordinates = tileCoordinates;
            }
            public int GetTotalCost() {
                return distance + Math.Abs(tileCoordinates.x - endTileCoordinates.x) + Math.Abs(tileCoordinates.y - endTileCoordinates.y) + Math.Abs(tileCoordinates.z - endTileCoordinates.z);
            }
        }
    }
}

