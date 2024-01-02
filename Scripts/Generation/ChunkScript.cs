using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ChunkScript : MonoBehaviour
{
    [HideInInspector]
    public GenerationProp prop;
	[HideInInspector]
	public ChunkArray chunkArray;

	int chunk;
	int chunkRender;
	Vector3Int locationGeneration;
    Vector3Int coordinates;
    
    bool completedChunk = true;

    int d = 0;
	Vector3Int tile = Vector3Int.zero;
	public void RenderChunk(Vector3Int locationRender)
    {
		chunkRender = chunkArray.chunksRender.layer.GetIndex(locationRender);
		locationGeneration = chunkArray.chunksRender.layer.LocationToLocation(locationRender, chunkArray.chunksGeneration.layer);
		chunk = chunkArray.chunksRender.layer.GetIndex(locationRender,chunkArray.chunks.layer);
		//if rendering new chunk then last time forget all values
		if (completedChunk || coordinates != prop.chunkArray.GetCoordinates(prop.chunkArray.chunksGeneration.layer.LocationToCoordinates(locationGeneration))) {
			coordinates = prop.chunkArray.GetCoordinates(prop.chunkArray.chunksGeneration.layer.LocationToCoordinates(locationGeneration));
			chunkArray.chunksRender.gameObject[chunkRender] = new GameObject("tile" + coordinates);
            chunkArray.chunksRender.gameObject[chunkRender].transform.parent = transform;
            chunkArray.chunksRender.gameObject[chunkRender].transform.localPosition = Vector3.Scale(prop.chunkSize, coordinates);
            d = 0;
            tile = Vector3Int.zero;
            completedChunk = false;
        }
        Position pos;
        //for every side
        while (d < 6)
        {
            pos = new Position(d);
            while (tile.z < prop.tileAmmount.z)
            {
                while (tile.y < prop.tileAmmount.y)
                {
                    while (tile.x < prop.tileAmmount.x)
                    {
                        Vector3Int side = tile + pos.Tile;
                        //for every side of wall
                        if (prop.GetSide(locationGeneration, tile, new Position(d)))
                        {
                            Position position = new Position(d);
                            Vector3 positive = new Vector3(0, 0, 0);
                            Vector3 negative = new Vector3(0, 0, 0);
                            //bools
                            bool right = prop.GetSide(locationGeneration, tile, new Position(position.RelValueX));
							bool left = prop.GetSide(locationGeneration, tile, new Position(-position.RelValueX));
							bool up = prop.GetSide(locationGeneration, tile, new Position(position.RelValueY));
                            bool down = prop.GetSide(locationGeneration, tile, new Position(-position.RelValueY));
                            bool right_forward = prop.GetSide(locationGeneration, tile + position.RelValueX, new Position(position.RelValue));
                            bool left_forward = prop.GetSide(locationGeneration, tile - position.RelValueX, new Position(position.RelValue));
                            bool up_forward = prop.GetSide(locationGeneration, tile + position.RelValueY, new Position(position.RelValue));
                            bool down_forward = prop.GetSide(locationGeneration, tile - position.RelValueY, new Position(position.RelValue));
                            //side bool
                            bool back_right = prop.GetSide(locationGeneration, tile + position.RelValue, new Position(position.RelValueX));
                            bool back_up = prop.GetSide(locationGeneration, tile + position.RelValue, new Position(position.RelValueY));

                            if (right)
                                positive -= (Vector3)position.RelValueX * prop.wallThickness / 2;
                            else if (!right_forward)
                            {
                                positive += (Vector3)position.RelValueX * prop.wallThickness / 2;
                                if (!back_right)
                                    createSide(new Position(-pos.RelValueX));
                            }
                            if (left)
                                negative += (Vector3)position.RelValueX * prop.wallThickness / 2;
                            else if (!left_forward)
                            {
                                negative -= (Vector3)position.RelValueX * prop.wallThickness / 2;
                            }
                            if (up)
                                positive -= (Vector3)position.RelValueY * prop.wallThickness / 2;
                            else if (!up_forward)
                            {
                                positive += (Vector3)position.RelValueY * prop.wallThickness / 2;
                                if (!back_up)
                                    createSide(new Position(-pos.RelValueY));
                            }
                            if (down)
                                negative += (Vector3)position.RelValueY * prop.wallThickness / 2;
                            else if (!down_forward)
                            {
                                negative -= (Vector3)position.RelValueY * prop.wallThickness / 2;
                            }
                            createWall();
                            void createWall()
                            {
                                ///
                                GetComponent<MeshScript>().CreateQuad(
                                    Vector3.Scale(prop.chunkSize, coordinates) - (prop.chunkSize / 2)
                                    + (prop.tileSize / 2)
                                    + (Vector3.Scale(prop.tileSize, side))
                                    + (Vector3.Scale(position.Value, prop.tileSize) / 2)
                                    - (Vector3.Scale(position.RelValueX, prop.tileSize) / 2)
                                    - (Vector3.Scale(position.RelValueY, prop.tileSize) / 2)
                                    - (Vector3)position.RelValue * prop.wallThickness / 2
                                    + negative
                                    ,
                                    Vector3.Scale(prop.chunkSize, coordinates) - (prop.chunkSize / 2)
                                    + (prop.tileSize / 2)
                                    + (Vector3.Scale(prop.tileSize, side))
                                    + (Vector3.Scale(position.Value, prop.tileSize) / 2)
                                    + (Vector3.Scale(position.RelValueX, prop.tileSize) / 2)
                                    + (Vector3.Scale(position.RelValueY, prop.tileSize) / 2)
                                    - (Vector3)position.RelValue * prop.wallThickness / 2
                                    + positive
                                    ,
                                    position).transform.parent = prop.chunkArray.chunksRender.gameObject[chunkRender].transform;
                            }
                            void createSide(Position pos)
                            {
                                Vector3Int vector = Vector3Int.Scale(pos.ValueReverse, position.ValueReverse);
                                Vector3Int relVector = vector * pos.Multiplier;
                                Vector3 negative = Vector3.zero;
                                Vector3 positive = Vector3.zero;
                                positive += (Vector3)vector * prop.wallThickness * pos.Multiplier / 2;
                                negative += (Vector3)vector * prop.wallThickness * pos.Multiplier / 2;


								GetComponent<MeshScript>().CreateQuad(
                                    Vector3.Scale(prop.chunkSize, coordinates) - (prop.chunkSize / 2)
                                    + (prop.tileSize / 2)
                                    + (Vector3.Scale(prop.tileSize, side))
                                    - (Vector3.Scale(pos.RelValueX, prop.tileSize) / 2)
                                    - (Vector3.Scale(pos.RelValueY, prop.tileSize) / 2)
                                    - (Vector3)pos.RelValue * prop.wallThickness / 2

                                    + (Vector3.Scale(position.Value * pos.Multiplier, prop.tileSize) / 2)
                                    - (Vector3)position.Value * pos.Multiplier * prop.wallThickness / 2
                                    - (Vector3.Scale(pos.RelValue, prop.tileSize) / 2)
                                    + (Vector3.Scale(position.Value, prop.tileSize) / 2)
                                    - negative
                                    ,
                                    Vector3.Scale(prop.chunkSize, coordinates) - (prop.chunkSize / 2)
                                    + (prop.tileSize / 2)
                                    + (Vector3.Scale(prop.tileSize, side))
                                    + (Vector3.Scale(pos.RelValueX, prop.tileSize) / 2)
                                    + (Vector3.Scale(pos.RelValueY, prop.tileSize) / 2)
                                    - (Vector3)pos.RelValue * prop.wallThickness / 2

                                    - (Vector3.Scale(position.Value * pos.Multiplier, prop.tileSize) / 2)
                                    + (Vector3)position.Value * pos.Multiplier * prop.wallThickness / 2
                                    - (Vector3.Scale(pos.RelValue, prop.tileSize) / 2)
                                    + (Vector3.Scale(position.Value, prop.tileSize) / 2)
                                    + positive
                                    ,
                                    pos).transform.parent = prop.chunkArray.chunksRender.gameObject[chunkRender].transform;
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
        prop.chunkArray.chunksRender.rendered[chunkRender] = true;
    }
}