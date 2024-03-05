using Unity.VisualScripting;
using UnityEngine;
namespace Generation
{
    public static class ChunkScript
    {
        static int chunk;
        static int chunkRender;
        static Vector3Int locationGeneration;
        static int indexGeneration;
        static Vector3Int coordinates;

        static bool completedChunk = true;

        static int d = 0;
        static Vector3Int tile = Vector3Int.zero;
        public static void RenderChunk(Vector3Int locationRender)
        {
			chunkRender = Layers.render.LayerLocationToIndex(locationRender);
            locationGeneration = Layers.render.LayerLocationToOtherLayerLocation(locationRender, Layers.generation);
            indexGeneration = Layers.generation.LayerLocationToIndex(locationGeneration);
			chunk = Layers.render.GetIndex(locationRender, Layers.hierarchy[0]);
            //if rendering new chunk then last time forget all values
            if (completedChunk || coordinates != ChunkArray.GetCoordinates(Layers.generation.LayerLocationToLayerCoordinates(locationGeneration)))
            {
                coordinates = ChunkArray.GetCoordinates(Layers.render.LayerLocationToLayerCoordinates(locationRender));
                ChunkArray.gameObject[chunkRender] = new GameObject("tile" + coordinates);
                ChunkArray.gameObject[chunkRender].transform.parent = GenerationProp.transform;
                ChunkArray.gameObject[chunkRender].transform.localPosition = Vector3.Scale(GenerationProp.chunkSize, coordinates);
                d = 0;
                tile = Vector3Int.zero;
                completedChunk = false;
            }
			Direction pos;
            //for every side
            while (d < 6)
            {
                pos = new Direction(d);
				while (tile.z < GenerationProp.tileAmount.z)
                {
                    while (tile.y < GenerationProp.tileAmount.y)
                    {
                        while (tile.x < GenerationProp.tileAmount.x)
                        {
                            if (!ChunkArray.accesible[indexGeneration, tile.x, tile.y, tile.z]) {
								goto Continue;
                            }
                            Vector3Int side = tile + pos.Tile;
                            //for every side of wall
                            if (GenerationProp.GetSide(locationGeneration, tile, new Direction(d)))
                            {
                                Direction position = new Direction(d);
                                Vector3 positive = new Vector3(0, 0, 0);
                                Vector3 negative = new Vector3(0, 0, 0);
                                //bools
                                bool right = GenerationProp.GetSide(locationGeneration, tile, new Direction(position.RelValueX));
                                bool left = GenerationProp.GetSide(locationGeneration, tile, new Direction(-position.RelValueX));
                                bool up = GenerationProp.GetSide(locationGeneration, tile, new Direction(position.RelValueY));
                                bool down = GenerationProp.GetSide(locationGeneration, tile, new Direction(-position.RelValueY));
                                bool right_forward = GenerationProp.GetSide(locationGeneration, tile + position.RelValueX, new Direction(position.RelValue));
                                bool left_forward = GenerationProp.GetSide(locationGeneration, tile - position.RelValueX, new Direction(position.RelValue));
                                bool up_forward = GenerationProp.GetSide(locationGeneration, tile + position.RelValueY, new Direction(position.RelValue));
                                bool down_forward = GenerationProp.GetSide(locationGeneration, tile - position.RelValueY, new Direction(position.RelValue));
                                //side bool
                                bool back_right = GenerationProp.GetSide(locationGeneration, tile + position.RelValue, new Direction(position.RelValueX));
                                bool back_up = GenerationProp.GetSide(locationGeneration, tile + position.RelValue, new Direction(position.RelValueY));

                                if (right)
                                    positive -= (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                else if (!right_forward)
                                {
                                    positive += (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                    if (!back_right)
                                        createSide(new Direction(-pos.RelValueX));
                                }
                                if (left)
                                    negative += (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                else if (!left_forward)
                                {
                                    negative -= (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                }
                                if (up)
                                    positive -= (Vector3)position.RelValueY * GenerationProp.wallThickness / 2;
                                else if (!up_forward)
                                {
                                    positive += (Vector3)position.RelValueY * GenerationProp.wallThickness / 2;
                                    if (!back_up)
                                        createSide(new Direction(-pos.RelValueY));
                                }
                                if (down)
                                    negative += (Vector3)position.RelValueY * GenerationProp.wallThickness / 2;
                                else if (!down_forward)
                                {
                                    negative -= (Vector3)position.RelValueY * GenerationProp.wallThickness / 2;
                                }
                                createWall();
                                void createWall()
                                {
                                    MeshScript.CreateQuad(
                                        Vector3.Scale(GenerationProp.chunkSize, coordinates) - (GenerationProp.chunkSize / 2)
                                        + (GenerationProp.tileSize / 2)
                                        + (Vector3.Scale(GenerationProp.tileSize, side))
                                        + (Vector3.Scale(position.Value, GenerationProp.tileSize) / 2)
                                        - (Vector3.Scale(position.RelValueX, GenerationProp.tileSize) / 2)
                                        - (Vector3.Scale(position.RelValueY, GenerationProp.tileSize) / 2)
                                        - (Vector3)position.RelValue * GenerationProp.wallThickness / 2
                                        + negative
                                        ,
                                        Vector3.Scale(GenerationProp.chunkSize, coordinates) - (GenerationProp.chunkSize / 2)
                                        + (GenerationProp.tileSize / 2)
                                        + (Vector3.Scale(GenerationProp.tileSize, side))
                                        + (Vector3.Scale(position.Value, GenerationProp.tileSize) / 2)
                                        + (Vector3.Scale(position.RelValueX, GenerationProp.tileSize) / 2)
                                        + (Vector3.Scale(position.RelValueY, GenerationProp.tileSize) / 2)
                                        - (Vector3)position.RelValue * GenerationProp.wallThickness / 2
                                        + positive
                                        ,
                                        position).transform.parent = ChunkArray.gameObject[chunkRender].transform;
                                }
                                void createSide(Direction pos)
                                {
                                    Vector3Int vector = Vector3Int.Scale(pos.ValueReverse, position.ValueReverse);
                                    Vector3Int relVector = vector * pos.Multiplier;
                                    Vector3 negative = Vector3.zero;
                                    Vector3 positive = Vector3.zero;
                                    positive += (Vector3)vector * GenerationProp.wallThickness * pos.Multiplier / 2;
                                    negative += (Vector3)vector * GenerationProp.wallThickness * pos.Multiplier / 2;


                                    MeshScript.CreateQuad(
                                        Vector3.Scale(GenerationProp.chunkSize, coordinates) - (GenerationProp.chunkSize / 2)
                                        + (GenerationProp.tileSize / 2)
                                        + (Vector3.Scale(GenerationProp.tileSize, side))
                                        - (Vector3.Scale(pos.RelValueX, GenerationProp.tileSize) / 2)
                                        - (Vector3.Scale(pos.RelValueY, GenerationProp.tileSize) / 2)
                                        - (Vector3)pos.RelValue * GenerationProp.wallThickness / 2

                                        + (Vector3.Scale(position.Value * pos.Multiplier, GenerationProp.tileSize) / 2)
                                        - (Vector3)position.Value * pos.Multiplier * GenerationProp.wallThickness / 2
                                        - (Vector3.Scale(pos.RelValue, GenerationProp.tileSize) / 2)
                                        + (Vector3.Scale(position.Value, GenerationProp.tileSize) / 2)
                                        - negative
                                        ,
                                        Vector3.Scale(GenerationProp.chunkSize, coordinates) - (GenerationProp.chunkSize / 2)
                                        + (GenerationProp.tileSize / 2)
                                        + (Vector3.Scale(GenerationProp.tileSize, side))
                                        + (Vector3.Scale(pos.RelValueX, GenerationProp.tileSize) / 2)
                                        + (Vector3.Scale(pos.RelValueY, GenerationProp.tileSize) / 2)
                                        - (Vector3)pos.RelValue * GenerationProp.wallThickness / 2

                                        - (Vector3.Scale(position.Value * pos.Multiplier, GenerationProp.tileSize) / 2)
                                        + (Vector3)position.Value * pos.Multiplier * GenerationProp.wallThickness / 2
                                        - (Vector3.Scale(pos.RelValue, GenerationProp.tileSize) / 2)
                                        + (Vector3.Scale(position.Value, GenerationProp.tileSize) / 2)
                                        + positive
                                        ,
                                        pos).transform.parent = ChunkArray.gameObject[chunkRender].transform;
                                }
								
							}
						Continue:
                            tile.x++;
                            if (GameEventsScript.mainTask.OutOfTime())
                                return;
                        }
                        tile.x = 0;
                        tile.y++;
                    }
                    tile.y = 0;
                    tile.z++;
                }
                tile.z = 0;
                d++;
            }
			if (Random.value < 1f) {
				TrySpawnNpc();
                if (!GameEventsScript.playerFoundKey) {
					TrySpawnKey();
				}
                TrySpawnTrapDoor();
			}

			completedChunk = true;
			Layers.render.created[locationRender.x, locationRender.y, locationRender.z] = true;
        }
		public static void DestroyChunk(Vector3Int locationRender) {
            Object.Destroy(ChunkArray.gameObject[Layers.render.LayerLocationToIndex(locationRender)]);
        }
        public static void TrySpawnNpc() {
            TileCoordinates tileCoordinates = new TileCoordinates();
            if (TrySpawn(ref tileCoordinates)) {
                NPCScript.Spawn(GenerationProp.TileCoordinatesToRealCoordinates(tileCoordinates));
			}
		}
		public static void TrySpawnKey() {
			TileCoordinates tileCoordinates = new TileCoordinates();
			
			if (TrySpawn(ref tileCoordinates)) {
                KeyPickup.Spawn(GenerationProp.TileCoordinatesToRealCoordinates(tileCoordinates));
			}
		}
		public static void TrySpawnTrapDoor() {
			TileCoordinates tileCoordinates = new TileCoordinates();
			if (TrySpawn(ref tileCoordinates)) {
                TrapDoor.Spawn(GenerationProp.TileCoordinatesToRealCoordinates(tileCoordinates));
			}
		}
		public static bool TrySpawn(ref TileCoordinates tileCoordinate) {
            int chunkGeneration = Layers.generation.LayerLocationToIndex(locationGeneration);
			for (int tries = 0; tries < 10; tries++) {
				Vector3Int tile = new Vector3Int(Random.Range(0, GenerationProp.tileAmount.x - 1), Random.Range(0, GenerationProp.tileAmount.y - 1), Random.Range(0, GenerationProp.tileAmount.z - 1));
				if (ChunkArray.accesible[chunkGeneration, tile.x, tile.y, tile.z]) {
                    tileCoordinate = new TileCoordinates(coordinates, tile);
					return true;
				}
			}
            return false;
		}
	}
}
