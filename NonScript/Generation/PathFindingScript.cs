using Generation;
using UnityEngine;
namespace PathFinfing{
    public static class PathFindingScript {
        static Vector3Int startTileCoordinates;
        static Vector3Int endTileCoordinates;
        public static void FindPath(Vector3Int startChunk, Vector3Int startTile, Vector3Int endChunk, Vector3Int endTile) {
            startTileCoordinates = GenerationProp.CoordinatesToTileCoordinates(ChunkArray.GetLocation(startChunk)) + startTile;
            endTileCoordinates = GenerationProp.CoordinatesToTileCoordinates(ChunkArray.GetLocation(endChunk)) + endTile;
            for (int i = 0; i < Direction.Directions.Length; i++) {
                TryGo(startTile, Direction.Directions[i]);
            }
        }
        private static void TryGo(Vector3Int tileCoordinates, Direction direction) {
            if (!GenerationProp.GetSide(tileCoordinates, direction)) {
                return;
            }
            int parentNodeIndex = GetIndex(tileCoordinates);
            int targetNodeIndex = GetIndex(tileCoordinates + direction.RelValue);
            if (ChunkArray.nodes[targetNodeIndex].distance <= ChunkArray.nodes[parentNodeIndex].distance) {
                return;
            }
            ChunkArray.nodes[targetNodeIndex] = new Node(new Direction(-direction.RelValue), ChunkArray.nodes[parentNodeIndex].distance+1);
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
        public Node(Direction parentDirection, int distance) {
            this.parentDirection = parentDirection;
            this.distance = distance;
        }
    }
}

