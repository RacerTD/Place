using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDataset
{
    public long Ticks;
    public List<Coordinate> ChangeList;

    public PlaceDataset(long ticks, List<Coordinate> changeList)
    {
        Ticks = ticks;
        ChangeList = changeList;
    }
}
