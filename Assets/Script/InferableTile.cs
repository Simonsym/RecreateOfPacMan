using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InferableTile
{
    public static readonly int EDGE_LEFT = -14;
    public static readonly int EDGE_RIGHT = 14;
    public static readonly int EDGE_TOP = 15;
    public static readonly int EDGE_BOTTOM = -15;

    public const int UP = 0;
    public const int RIGHT = 1;
    public const int DOWN = 2;
    public const int LEFT = 3;

    public static readonly int TYPE_TURN = 0;
    public static readonly int TYPE_WALL = 1;
    public static readonly int TYPE_OTHERS = 2;


    public int x, y;
    int[] directions;
    int tileType;
    int rotate;

    static readonly Dictionary<int, List<int>> turnMapping = new Dictionary<int, List<int>>() {
        {0,  new List<int> {InferableTile.RIGHT, InferableTile.DOWN}},
        {90, new List<int> {InferableTile.DOWN,  InferableTile.LEFT}},
        {180,new List<int> {InferableTile.LEFT,  InferableTile.UP}},
        {270,new List<int> {InferableTile.UP,    InferableTile.RIGHT}}
    };

    static readonly Dictionary<int, List<int>> wallMapping = new Dictionary<int, List<int>>() {
        {0,    new List<int> {InferableTile.UP,   InferableTile.DOWN}},
        {90,   new List<int> {InferableTile.LEFT, InferableTile.RIGHT}},
        {180,  new List<int> {InferableTile.UP,   InferableTile.DOWN}},
        {270,  new List<int> {InferableTile.LEFT, InferableTile.RIGHT}},
    };

    public InferableTile() {
        x = 0;
        y = 0;

        directions = new int[2];
        directions[0] = RIGHT;
        directions[1] = DOWN;

        tileType = TYPE_TURN;

        rotate = 0;
    }

    public InferableTile(int x, int y, int tileType) {
        this.x = x;
        this.y = y;

        directions = new int[2];
        if(tileType == TYPE_TURN) {
            directions[0] = turnMapping[0][0];
            directions[1] = turnMapping[0][1];
        }
        else if (tileType == TYPE_WALL) {
            directions[0] = wallMapping[0][0];
            directions[1] = wallMapping[0][1];
        }
        else {
            directions = new int[0];
        }

        this.tileType = tileType;

        rotate = 0;
    }

    public int[] getDirections() {
        if(tileType == TYPE_TURN) {
            int[] result = new int[2];

            result[0] = turnMapping[rotate][0];
            result[1] = turnMapping[rotate][1];

            return result;
        }
        else if (tileType == TYPE_WALL) {
            int[] result = new int[2];

            result[0] = wallMapping[rotate][0];
            result[1] = wallMapping[rotate][1];

            return result;
        }
        else {
            return new int[0];
        }

    }

}
