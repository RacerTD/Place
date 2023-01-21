using System;
using UnityEngine;

/// <summary>
/// A simplified coordinate for r/place coordinates
/// </summary>
[System.Serializable]
public class Coordinate
{
    public short X;
    public short Y;
    public byte Color;

    public Coordinate(short x, short y, byte color)
    {
        X = x;
        Y = y;
        Color = color;
    }
}