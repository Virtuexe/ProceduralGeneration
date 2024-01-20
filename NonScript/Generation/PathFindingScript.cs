using Generation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace PathFinding{
    public static class PathFindingScript {
        private static Node[] nodes = new Node[Layers.generation.LengthInt * GenerationProp.tileAmmount.x * GenerationProp.tileAmmount.y * GenerationProp.tileAmmount.z];
        private static int nodeCount = 0;
        private static int[] nodeQueueIndexes = new int[
            (Layers.generation.Length.y * GenerationProp.tileAmmount.y * Layers.generation.Length.z * GenerationProp.tileAmmount.z) +
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x * Layers.generation.Length.z * GenerationProp.tileAmmount.z) +
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x * Layers.generation.Length.y * GenerationProp.tileAmmount.y) -
            (Layers.generation.Length.x * GenerationProp.tileAmmount.x + Layers.generation.Length.y * GenerationProp.tileAmmount.y + 1) * 2
            ];

        static Vector3Int startTileCoordinates;
        static Vector3Int endTileCoordinates;
        public static void FindPath(Vector3Int startTileCoordinates, Vector3Int endTileCoordinates) {
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = new Node(int.MaxValue);
            }
            PathFindingScript.startTileCoordinates = startTileCoordinates;
            PathFindingScript.endTileCoordinates = endTileCoordinates;
            int startNodeIndex = GetIndex(startTileCoordinates);
            nodes[startNodeIndex] = new Node();
            for (int i = 0; i < Direction.Directions.Length; i++) {
                TryGo(startNodeIndex, Direction.Directions[i]);
            }
            ProcessQueue();
        }
        private static void ProcessQueue() {
            Debug.Log("finding...");
            while (nodeCount != 0) {
                Debug.Log("queue...");
                nodeCount--;
                for (int i = 0; i < Direction.Directions.Length; i++) {
                    if(Direction.Directions[i] == nodes[nodeCount].parentDirection) {
                        return;
                    }
                    TryGo(nodeCount, Direction.Directions[i]);
                }
            }
        } 
        private static void AddNodeToQueue(int index) {
            nodeQueueIndexes[index] = nodeCount;
            nodeCount++;
        }
        private static void TryGo(int nodeIndex, Direction direction) {
            if (Layers.generation.IsLocationOutOfBounds(GenerationProp.TileCoordinatesToCoordinates(nodes[nodeIndex].tileCoordinates)) || !GenerationProp.GetSide(nodes[nodeIndex].tileCoordinates, direction)) {
                return;
            }
            int targetNodeIndex = GetIndex(nodes[nodeIndex].tileCoordinates + direction.RelValue);
            if (nodes[targetNodeIndex].distance <= nodes[nodeIndex].distance) {
                return;
            }
            nodes[targetNodeIndex] = new Node(new Direction(-direction.RelValue), nodes[nodeIndex].distance + 1, nodes[nodeIndex].tileCoordinates + direction.RelValue);
            if(endTileCoordinates == nodes[nodeIndex].tileCoordinates + direction.RelValue) {
                Debug.Log("new best path: " + nodes[targetNodeIndex].distance);
                return;
            }
            Debug.Log("added");
            AddNodeToQueue(targetNodeIndex);
        }
        public static int GetIndex(Vector3Int TileCoordinates) {
            Vector3Int coordinates = GenerationProp.TileCoordinatesToCoordinates(TileCoordinates);
            int chunkIndex = Layers.generation.GetIndex(coordinates);
            Vector3Int tile = GenerationProp.TileCoordinatesToTile(TileCoordinates);
            return chunkIndex + Layers.generation.LengthInt * (tile.x + GenerationProp.tileAmmount.x * (tile.y + GenerationProp.tileAmmount.y * tile.z));
        }
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
    }
}

