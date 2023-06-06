using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile
{
    private MyGrid<PathTile> grid;
    public int x, z;
    public int f, g, h;
    public PathTile previousTile;
    private bool empty;
    private Transform presentUnit;

    public PathTile(MyGrid<PathTile> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
        Empty = true;
        PresentUnit = null;
    }

    public void SetF()
    {
        f = g + h;
    }

    public Transform PresentUnit {
        get { return presentUnit; }
        set { presentUnit = value; grid.GridChanged(x, z); }
    }

    public bool Empty {
        get { return empty; }
        set { empty = value; grid.GridChanged(x, z); }
    }
}
