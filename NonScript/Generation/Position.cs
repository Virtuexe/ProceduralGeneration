using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct Position
{

    public Position(int value)
    {
        switch (value)
        {
            //MAIN
            case 0: //TOP
                this = Position.Top;
                break;
            case 1: //Front
                this = Position.Front;
                break;
            case 2: //Right
                this = Position.Right;
                break;
            //SECONDARY
            case 3: //BUTTOM
                this = Position.Bottom;
                break;
            case 4: //BACK
                this = Position.Back;
                break;
            case 5: //LEFT
                this = Position.Left;
                break;
            default: throw new ArgumentException("No position with value " + value + " does not exist");
        }
    }
    public Position(Vector3Int value)
    {
        switch (value.x, value.y, value.z)
        {
            case (0, 1, 0):
                this = Position.Top;
                break;
            case (0, 0, 1):
                this = Position.Front;
                break;
            case (1, 0, 0):
                this = Position.Right;
                break;
            case (0, -1, 0):
                this = Position.Bottom;
                break;
            case (0, 0, -1):
                this = Position.Back;
                break;
            case (-1, 0, 0):
                this = Position.Left;
                break;
            default: throw new ArgumentException("No position with position " + value + " does not exist");
        }
    }
    public Position(int x, int y, int z)
    {
        switch (x, y, z)
        {
            case (0, 1, 0):
                this = Position.Top;
                break;
            case (0, 0, 1):
                this = Position.Front;
                break;
            case (1, 0, 0):
                this = Position.Right;
                break;
            case (0, -1, 0):
                this = Position.Bottom;
                break;
            case (0, 0, -1):
                this = Position.Back;
                break;
            case (-1, 0, 0):
                this = Position.Left;
                break;
            default: throw new ArgumentException("No position with position " + new Vector3Int(x,y,z) + " does not exist");
        }
    }
    public readonly static Position[] Directions = { Top, Front, Right, Bottom, Back, Left};
    public Vector3Int Value { get; private set; }
    public Vector3Int RelValue { get { return Value * Multiplier; } }
    public Vector3Int ValueReverse { get; private set; }
    public Vector3Int ValueX { get; private set; }
    public Vector3Int RelValueX { get { return ValueX * Multiplier; } }
    public Vector3Int ValueY { get; private set; }
    public bool Rotation { get; private set; }
    public Vector3Int RelValueY { get { return ValueY * Multiplier; } }
    public int Side;
    public Vector3Int Tile;
    public int Index { get; private set; }
    public int Multiplier;
	public int Binary;
	public static bool Priority(Position first, Position second)
    {
        if (first.Side < second.Side) return false;
        return true;
    }
    public static Position Top = new Position
    {
        Value = new Vector3Int(0, 1, 0),
        ValueReverse = new Vector3Int(1, 0, 1),
        ValueX = new Vector3Int(1, 0, 0),
        ValueY = new Vector3Int(0, 0, 1),
        Index = 0,
        Multiplier = 1,
        Side = 0,
        Tile = new Vector3Int(0, 0, 0),
        Rotation = true,
        Binary = 1
    };

    public static Position Bottom = new Position
    {
        Value = new Vector3Int(0, 1, 0),
        ValueReverse = new Vector3Int(1, 0, 1),
        ValueX = new Vector3Int(1, 0, 0),
        ValueY = new Vector3Int(0, 0, 1),
        Index = 3,
        Multiplier = -1,
        Side = 0,
        Tile = new Vector3Int(0, -1, 0),
        Rotation = false,
		Binary = 2
	};

    public static Position Front = new Position
    {
        Value = new Vector3Int(0, 0, 1),
        ValueReverse = new Vector3Int(1, 1, 0),
        ValueX = new Vector3Int(1, 0, 0),
        ValueY = new Vector3Int(0, 1, 0),
        Index = 1,
        Multiplier = 1,
        Side = 1,
        Tile = new Vector3Int(0, 0, 0),
        Rotation = false,
		Binary = 4
	};

    public static Position Back = new Position
    {
        Value = new Vector3Int(0, 0, 1),
        ValueReverse = new Vector3Int(1, 1, 0),
        ValueX = new Vector3Int(1, 0, 0),
        ValueY = new Vector3Int(0, 1, 0),
        Index = 4,
        Multiplier = -1,
        Side = 1,
        Tile = new Vector3Int(0, 0, -1),
        Rotation = true,
		Binary = 8
	};

    public static Position Right = new Position
    {
        Value = new Vector3Int(1, 0, 0),
        ValueReverse = new Vector3Int(0, 1, 1),
        ValueX = new Vector3Int(0, 0, 1),
        ValueY = new Vector3Int(0, 1, 0),
        Index = 2,
        Multiplier = 1,
        Side = 2,
        Tile = new Vector3Int(0, 0, 0),
        Rotation = true,
		Binary = 16
	};

    public static Position Left = new Position
    {
        Value = new Vector3Int(1, 0, 0),
        ValueReverse = new Vector3Int(0, 1, 1),
        ValueX = new Vector3Int(0, 0, 1),
        ValueY = new Vector3Int(0, 1, 0),
        Index = 5,
        Multiplier = -1,
        Side = 2,
        Tile = new Vector3Int(-1, 0, 0),
        Rotation = false,
		Binary = 32
	};
}
