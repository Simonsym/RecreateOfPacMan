
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Record {

    public InferableTile inferableTile;
    public int direction;

    public Record(InferableTile inferableTile, int direction)
    {
        this.inferableTile = inferableTile;
        this.direction = direction;
    }

    public static Record n(InferableTile inferableTile, int direction) {
        return new Record(inferableTile, direction);
    }

}