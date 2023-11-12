using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class RoomScript : MonoBehaviour
{
    public GenerationRoom roomObject;
    public MeshScript mesh;
    public GeneratorManagerScript manager;
    GenerationRoom room;
    bool[,,] checkedTiles;

    public GameObject CreateRoom(GenerationRoom room)
    {
        float width = manager.tileWidth;
        float height = manager.tileHeight;
        float thicknes = manager.wallThickness;
        GameObject g = new GameObject("tile");
        this.room = room;
        for (int position = 0; position <= 5; position++)
        {
            Position pos = new Position(new Vector3Int(0, 0, 0));
            switch (position)
            {

                case 0:
                    pos = Position.Buttom;
                    break;
                case 1:
                    pos = Position.Front;
                    break;
                case 2:
                    pos = Position.Back;
                    break;
                case 3:
                    pos = Position.Right;
                    break;
                case 4:
                    pos = Position.Left;
                    break;

                case 5:
                    pos = Position.Top;
                    break;
                default:
                    break;
            }
            Vector3Int currentTile = new Vector3Int(0, 0, 0);
            List<GenerationMesh> positions = new List<GenerationMesh>();
            bool lengthSet = true;
            Vector3Int lastTile = new Vector3Int(0, 0, 0);
            /*
             * example floor:
             *  forward is X
             *  left is Y
             *  next layer is up
             *  while(layer)
             *  {
             *      while(Y)
             *      {
             *          while(X)
             *      }
             *  }
             */
            while (pos.ValueZ.x * currentTile.x < room.tiles.GetLength(0) && pos.ValueZ.y * currentTile.y < room.tiles.GetLength(1) && pos.ValueZ.z * currentTile.z < room.tiles.GetLength(2))
            {
                while (pos.ValueY.x * currentTile.x < room.tiles.GetLength(0) && pos.ValueY.y * currentTile.y < room.tiles.GetLength(1) && pos.ValueY.z * currentTile.z < room.tiles.GetLength(2))
                {
                    while (pos.ValueX.x * currentTile.x < room.tiles.GetLength(0) && pos.ValueX.y * currentTile.y < room.tiles.GetLength(1) && pos.ValueX.z * currentTile.z < room.tiles.GetLength(2))
                    {
                        if (lengthSet)
                        {
                            //set start position and end if next is not valid
                            if (PlacableWall(currentTile, pos)) {
                                lastTile = currentTile;
                                lengthSet = false;
                                if (!PlacableWall(currentTile + pos.ValueX, pos))
                                {
                                    Vector3 from = new Vector3(0, 0, 0), to = new Vector3(0, 0, 0);
                                    Corners(pos, lastTile, currentTile, ref from, ref to);
                                    positions.Add(new GenerationMesh(from, to));
                                    lengthSet = true;
                                }
                            }
                        }
                        else
                        {
                            //set end here if next position is not valid
                            if (!PlacableWall(currentTile + pos.ValueX, pos)) {
                                Vector3 from = new Vector3(0,0,0),to = new Vector3(0,0,0);
                                Corners(pos,lastTile, currentTile, ref from, ref to);
                                positions.Add(new GenerationMesh(from, to));
                                lengthSet = true;
                            }
                        }
                        //adds +1 to X
                        currentTile = currentTile + pos.ValueX;
                    }
                    //sets X to 0 and adds +1 Y
                    currentTile = currentTile * (pos.ValueY + pos.ValueZ) + pos.ValueY;
                }
                //sets X and Y to zero and adds +1 layer
                currentTile = currentTile * pos.ValueZ + pos.ValueZ;
            }
            for (int i = 0; i < positions.Count; i++)
            {
                //combine same meshes
                Vector3 from = positions[i].from;
                Vector3 to = positions[i].to;
                //check if +Y is same if yes remove it and add 1 Y



                Vector3 fromCurrent = from;
                int index = ConnectabbleWallIndex(positions,fromCurrent, to, pos);
                while (index != -1)
                {
                    to += Vector3.Distance(to,positions[index].to) * (Vector3)pos.ValueY;
                    fromCurrent += Vector3.Distance(fromCurrent, positions[index].from) * (Vector3)pos.ValueY;
                    positions.RemoveAt(index);
                    index = ConnectabbleWallIndex(positions, fromCurrent, to, pos);
                }
                

                Vector3 third = new Vector3(to.x - (pos.ValueX.x * (to.x - from.x)), to.y - (pos.ValueX.y * (to.y - from.y)), to.z - (pos.ValueX.z * (to.z - from.z)));
                /// TEST
                /*
                while (positions.Remove(new GenerationMesh(third, to + new Vector3(width * pos.ValueY.x, height * pos.ValueY.y, width * pos.ValueY.z))))
                {
                    to += new Vector3(width * pos.ValueY.x, height * pos.ValueY.y, width * pos.ValueY.z);
                    third += new Vector3(width * pos.ValueY.x, height * pos.ValueY.y, width * pos.ValueY.z);
                }
                */
                ///
                //create mesh
                GameObject m;
                if (pos.Value == Position.Front.Value || pos.Value == Position.Right.Value || pos.Value == Position.Top.Value)
                {
                    m = mesh.CreateQuad(
                        third,
                        to,
                        from
                        );
                }
                else
                {
                    m = mesh.CreateQuad(
                        third,
                        from,
                        to
                        );
                }

                GameObject testFROM = new GameObject("FROM");
                GameObject testTO = new GameObject("TO");
                GameObject testB = new GameObject("B");
                testFROM.transform.parent = m.transform;
                testTO.transform.parent = m.transform;
                testB.transform.parent = m.transform;
                testFROM.transform.position = from;
                testTO.transform.position = to;
                testB.transform.position = new Vector3(to.x - (pos.ValueX.x * (to.x - from.x)), to.y - (pos.ValueX.y * (to.y - from.y)), to.z - (pos.ValueX.z * (to.z - from.z)));

                m.name = pos.Value.ToString();
                m.transform.parent = g.transform;
                m.transform.position = Vector3.zero;
            }
        }
        return g;
    }

    bool TileEmpty(Vector3Int coordinates)
    {
        bool checkCurrent = (coordinates.x < room.tiles.GetLength(0) && coordinates.y < room.tiles.GetLength(1) && coordinates.z < room.tiles.GetLength(2)) && (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.z >= 0);
        if (checkCurrent && room.tiles[coordinates.x, coordinates.y, coordinates.z] == true)
            return true;
        return false;
    }
    bool PlacableWall(Vector3Int coordinates, Position position)
    {
        bool checkCurrent;
        bool checkBehind;
        checkCurrent = (coordinates.x < room.tiles.GetLength(0) && coordinates.y < room.tiles.GetLength(1) && coordinates.z < room.tiles.GetLength(2)) && (coordinates.x >= 0 && coordinates.y >= 0 && coordinates.z >= 0);
        checkBehind = (coordinates.x + position.Value.x < room.tiles.GetLength(0) && coordinates.y + position.Value.y < room.tiles.GetLength(1) && coordinates.z + position.Value.z < room.tiles.GetLength(2)) && (coordinates.x + position.Value.x >= 0 && coordinates.y + position.Value.y >= 0 && coordinates.z + position.Value.z >= 0);
        //if tile exists, there is no tile behind it and it does not contain door on its side -> wall can be placed 
        if (checkCurrent && room.tiles[coordinates.x, coordinates.y, coordinates.z] && (!checkBehind || !room.tiles[coordinates.x + position.Value.x, coordinates.y + position.Value.y, coordinates.z + position.Value.z]) && !room.doors.Contains(new GenerationDoor(coordinates, position)))
        {
            return true;
        }
        return false;
    }
    void Corners(Position position, Vector3Int fromTile, Vector3Int toTile, ref Vector3 from, ref Vector3 to)
    {
        //from - thickness
        from = new Vector3(fromTile.x * manager.tileWidth, fromTile.y * manager.tileHeight, fromTile.z * manager.tileWidth)
            - new Vector3(position.Value.x * manager.wallThickness, position.Value.y * manager.wallThickness, position.Value.z * manager.wallThickness);
        //to + size of tile
        to = new Vector3(toTile.x * manager.tileWidth, toTile.y * manager.tileHeight, toTile.z * manager.tileWidth)
            + new Vector3(position.ValueReverse.x * manager.tileWidth, position.ValueReverse.y * manager.tileHeight, position.ValueReverse.z * manager.tileWidth)
            - new Vector3(position.Value.x * manager.wallThickness, position.Value.y * manager.wallThickness, position.Value.z * manager.wallThickness);
        if (position.Value == Position.Front.Value || position.Value == Position.Right.Value || position.Value == Position.Top.Value)
        {
            from += new Vector3(position.Value.x * manager.tileWidth, position.Value.y * manager.tileHeight, position.Value.z * manager.tileWidth);
            to += new Vector3(position.Value.x * manager.tileWidth, position.Value.y * manager.tileHeight, position.Value.z * manager.tileWidth);
        }
        /*--------------FROM--------------*/

        //X

        //IF tile is next to from continue ELSE remove thickness offset
        if (TileEmpty(fromTile - position.ValueX))
        {
            //IF tile is empty behind that tile dont add thickess to X ELSE add thicknees to X
            if (TileEmpty(fromTile - position.ValueX + position.Value))
            {
                // + thickness offset X
                from += - new Vector3(position.ValueX.x * manager.wallThickness, position.ValueX.y * manager.wallThickness, position.ValueX.z * manager.wallThickness);

            }
        }
        else
        {
            //- thickness offset X
            from += new Vector3(position.ValueX.x * manager.wallThickness, position.ValueX.y * manager.wallThickness, position.ValueX.z * manager.wallThickness);

        }

        //Y

        //IF tile is next to from continue ELSE remove thickness offset
        if (TileEmpty(fromTile - position.ValueY))
        {
            //IF tile is empty behind that tile dont add thickess to X ELSE add thicknees to X
            if (TileEmpty(fromTile - position.ValueY + position.Value))
            {
                // thickness offset X
                from += - new Vector3(position.ValueY.x * manager.wallThickness, position.ValueY.y * manager.wallThickness, position.ValueY.z * manager.wallThickness);
            }
        }
        else
        {
            // - thickness offset X
            from += new Vector3(position.ValueY.x * manager.wallThickness, position.ValueY.y * manager.wallThickness, position.ValueY.z * manager.wallThickness);

        }

        /*--------------TO--------------*/

        //X

        //IF tile is next to from continue ELSE remove thickness offset
        if (TileEmpty(toTile + position.ValueX))
        {
            //IF tile is empty behind that tile dont remove thickness
            if (TileEmpty(toTile + position.ValueX + position.Value))
            {
                //- thickness offset X
                to += new Vector3(position.ValueX.x * manager.wallThickness, position.ValueX.y * manager.wallThickness, position.ValueX.z * manager.wallThickness);
            }
        }
        else
        {
            //- thickness offset X
            to += -new Vector3(position.ValueX.x * manager.wallThickness, position.ValueX.y * manager.wallThickness, position.ValueX.z * manager.wallThickness);
        }

        //Y

        //IF tile is next to from continue ELSE remove thickness offset
        if (TileEmpty(toTile + position.ValueY))
        {
            //IF tile is empty behind that tile dont add thickess to X ELSE add thicknees to X
            if (TileEmpty(toTile + position.ValueY + position.Value))
            {
                //-thickness offset Y
                to += new Vector3(position.ValueY.x * manager.wallThickness, position.ValueY.y * manager.wallThickness, position.ValueY.z * manager.wallThickness);
            }
        }
        else
        {
            //thickness offset Y
            to += -new Vector3(position.ValueY.x * manager.wallThickness, position.ValueY.y * manager.wallThickness, position.ValueY.z * manager.wallThickness);
        }
    }
    int ConnectabbleWallIndex(List<GenerationMesh> positions, Vector3 from, Vector3 to, Position pos)
    {
        int index = positions.FindIndex(
        mesh => (
        (new Vector3(from.x * pos.ValueZ.x, from.y * pos.ValueZ.y, from.z * pos.ValueZ.z) == new Vector3(mesh.from.x * pos.ValueZ.x, mesh.from.y * pos.ValueZ.y, mesh.from.z * pos.ValueZ.z)) ? //if on same layer
                                                                                                                                                                                                                     //true
            (
            //from X must be same as from2 X AND to Y must be same as from2 Y AND to X must be same as to2 X
                new Vector3(from.x*pos.ValueX.x,from.y * pos.ValueX.y, from.z * pos.ValueX.z) == new Vector3(mesh.from.x * pos.ValueX.x, mesh.from.y * pos.ValueX.y, mesh.from.z * pos.ValueX.z)
                &&
                new Vector3(to.x*pos.ValueY.x,to.y * pos.ValueY.y, to.z * pos.ValueY.z) == new Vector3(mesh.from.x * pos.ValueY.x, mesh.from.y * pos.ValueY.y, mesh.from.z * pos.ValueY.z)
                &&
                new Vector3(to.x*pos.ValueX.x, to.y * pos.ValueX.y, to.z * pos.ValueX.z) == new Vector3(mesh.to.x * pos.ValueX.x, mesh.to.y * pos.ValueX.y, mesh.to.z * pos.ValueY.z)
            ) ?
            true : false
            :
            //false
            false
    )
    );
        return index;
    }
}
[System.Serializable]
public struct GenerationRoom
{
    public bool[,,] tiles;
    public List<GenerationDoor> doors;
    public GenerationRoom(bool[,,] tiles, List<GenerationDoor> doors)
    {
        this.tiles = tiles;
        this.doors = doors;
    }
}
public struct GenerationDoor
{
    public Vector3Int coordinates;
    public Position position;
    public GenerationDoor(Vector3Int coordinates, Position position)
    {
        this.coordinates = coordinates;
        this.position = position;
    }
}
public struct GenerationMesh
{
    public Vector3 from;
    public Vector3 to;
    public GenerationMesh(Vector3 from, Vector3 to)
    {
        this.from = from;
        this.to = to;
    }
}


