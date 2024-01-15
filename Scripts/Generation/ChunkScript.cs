using UnityEngine;
namespace Generation
{
    public class ChunkScript : MonoBehaviour
    {
        int chunk;
        int chunkRender;
        Vector3Int locationGeneration;
        Vector3Int coordinates;

        bool completedChunk = true;

        int d = 0;
        Vector3Int tile = Vector3Int.zero;
        public void RenderChunk(Vector3Int locationRender)
        {
            chunkRender = Layers.render.GetIndex(locationRender);
            locationGeneration = Layers.render.LocationToLocation(locationRender, Layers.generation);
            chunk = Layers.render.GetIndex(locationRender, Layers.hierarchy[0]);
            //if rendering new chunk then last time forget all values
            if (completedChunk || coordinates != ChunkArray.GetCoordinates(Layers.generation.LocationToCoordinates(locationGeneration)))
            {
                coordinates = ChunkArray.GetCoordinates(Layers.render.LocationToCoordinates(locationRender));
                ChunkArray.gameObject[chunkRender] = new GameObject("tile" + coordinates);
                ChunkArray.gameObject[chunkRender].transform.parent = transform;
                ChunkArray.gameObject[chunkRender].transform.localPosition = Vector3.Scale(GenerationProp.chunkSize, coordinates);
                d = 0;
                tile = Vector3Int.zero;
                completedChunk = false;
            }
            Position pos;
            //for every side
            while (d < 6)
            {
                pos = new Position(d);
                while (tile.z < GenerationProp.tileAmmount.z)
                {
                    while (tile.y < GenerationProp.tileAmmount.y)
                    {
                        while (tile.x < GenerationProp.tileAmmount.x)
                        {
                            Vector3Int side = tile + pos.Tile;
                            //for every side of wall
                            if (GenerationProp.GetSide(locationGeneration, tile, new Position(d)))
                            {
                                Position position = new Position(d);
                                Vector3 positive = new Vector3(0, 0, 0);
                                Vector3 negative = new Vector3(0, 0, 0);
                                //bools
                                bool right = GenerationProp.GetSide(locationGeneration, tile, new Position(position.RelValueX));
                                bool left = GenerationProp.GetSide(locationGeneration, tile, new Position(-position.RelValueX));
                                bool up = GenerationProp.GetSide(locationGeneration, tile, new Position(position.RelValueY));
                                bool down = GenerationProp.GetSide(locationGeneration, tile, new Position(-position.RelValueY));
                                bool right_forward = GenerationProp.GetSide(locationGeneration, tile + position.RelValueX, new Position(position.RelValue));
                                bool left_forward = GenerationProp.GetSide(locationGeneration, tile - position.RelValueX, new Position(position.RelValue));
                                bool up_forward = GenerationProp.GetSide(locationGeneration, tile + position.RelValueY, new Position(position.RelValue));
                                bool down_forward = GenerationProp.GetSide(locationGeneration, tile - position.RelValueY, new Position(position.RelValue));
                                //side bool
                                bool back_right = GenerationProp.GetSide(locationGeneration, tile + position.RelValue, new Position(position.RelValueX));
                                bool back_up = GenerationProp.GetSide(locationGeneration, tile + position.RelValue, new Position(position.RelValueY));

                                if (right)
                                    positive -= (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                else if (!right_forward)
                                {
                                    positive += (Vector3)position.RelValueX * GenerationProp.wallThickness / 2;
                                    if (!back_right)
                                        createSide(new Position(-pos.RelValueX));
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
                                        createSide(new Position(-pos.RelValueY));
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
                                    ///
                                    GetComponent<MeshScript>().CreateQuad(
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
                                void createSide(Position pos)
                                {
                                    Vector3Int vector = Vector3Int.Scale(pos.ValueReverse, position.ValueReverse);
                                    Vector3Int relVector = vector * pos.Multiplier;
                                    Vector3 negative = Vector3.zero;
                                    Vector3 positive = Vector3.zero;
                                    positive += (Vector3)vector * GenerationProp.wallThickness * pos.Multiplier / 2;
                                    negative += (Vector3)vector * GenerationProp.wallThickness * pos.Multiplier / 2;


                                    GetComponent<MeshScript>().CreateQuad(
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
            completedChunk = true;
            Layers.render.created[locationRender.x, locationRender.y, locationRender.z] = true;
        }
    }
}
