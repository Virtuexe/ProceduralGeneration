using Generation;
using System.Drawing;
using UnityEngine;
namespace PathFinfing{
    public static class PathFindingScript {
        static int nodeAmount = 1;
        static Node[] nodes = new Node[Layers.generation.LengthInt * GenerationProp.tileAmmount.x * GenerationProp.tileAmmount.y * GenerationProp.tileAmmount.z];

        public static void FindPath(Vector3Int startChunk, Vector3Int startTile, Vector3Int endChunk, Vector3Int endTile) {
            Vector3Int startTileCoordinates = GenerationProp.CoordinatesToTileCoordinates(ChunkArray.GetLocation(startChunk)) + startTile;
            Vector3Int endTileCoordinates = GenerationProp.CoordinatesToTileCoordinates(ChunkArray.GetLocation(endChunk)) + endTile;
            nodes[0] = new Node(-1,startTileCoordinates);
            for (int i = 0; i < Direction.Directions.Length; i++) {
                TryGo(0, Direction.Directions[i]);
            }
        }
        private static void TryGo(int nodeIndex, Direction direction) {
            if (!GenerationProp.GetSide(nodes[nodeIndex].tileCoordinates, direction)) {
                return;
            }
            nodes[nodeAmount] = new Node(nodeIndex, nodes[nodeIndex].tileCoordinates + direction.RelValue);
            nodeAmount++;
        }
    }
    struct Node {
        public int parentIndex { get; private set; }
        public Vector3Int tileCoordinates;
        public Node(int parentIndex, Vector3Int tileCoordinates) {
            this.parentIndex = parentIndex;
            this.tileCoordinates = tileCoordinates;
        }
    }
}

