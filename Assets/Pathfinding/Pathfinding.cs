using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int straightStepCost = 10;
    private const int diagonalStepCost = 14;

    public static Pathfinding Instance { get; private set; }

    private MyGrid<PathTile> grid;
    private List<PathTile> findingList;
    private List<PathTile> foundList;

    public Pathfinding(int width, int length, float scale, Vector3 originPos)
    {
        Instance = this;
        Grid = new MyGrid<PathTile>(width, length, scale, originPos, (MyGrid<PathTile> g, int x, int z) => new PathTile(g, x, z));
    }

    public MyGrid<PathTile> Grid { get => grid; set => grid = value; }

    private List<PathTile> FindPath(int startX, int startZ, int endX, int endZ, int range)
    {
        PathTile startTile = Grid.GetValue(startX, startZ);
        PathTile endTile = Grid.GetValue(endX, endZ);

        findingList = new List<PathTile> { startTile };
        foundList = new List<PathTile>();

        for(int i = 0; i < Grid.GetWidth(); i++)
        {
            for(int j = 0; j < Grid.GetLength(); j++)
            {
                PathTile pathTile = Grid.GetValue(i, j);
                pathTile.g = int.MaxValue;
                pathTile.SetF();
                pathTile.previousTile = null;
            }
        }

        startTile.g = 0;
        startTile.h = GetH(startTile, endTile);
        startTile.SetF();

        while (findingList.Count > 0)
        {
            PathTile currentTile = GetMinFTile(findingList);
            /*if (currentTile == endTile)
                return GetPath(endTile);*/
            if (Math.Floor(Math.Sqrt((endTile.x - currentTile.x) * (endTile.x - currentTile.x) + (endTile.z - currentTile.z) * (endTile.z - currentTile.z))) <= range)
                return GetPath(currentTile);

            findingList.Remove(currentTile);
            foundList.Add(currentTile);

            foreach(PathTile neighbourTile in GetNeighbourTiles(currentTile))
            {
                if (foundList.Contains(neighbourTile)) continue;
                if (!neighbourTile.Empty)
                {
                    foundList.Add(neighbourTile);
                    continue;
                }

                int g = currentTile.g + GetH(currentTile, neighbourTile);
                if (g < neighbourTile.g)
                {
                    neighbourTile.previousTile = currentTile;
                    neighbourTile.g = g;
                    neighbourTile.h = GetH(neighbourTile, endTile);
                    neighbourTile.SetF();

                    if (!findingList.Contains(neighbourTile))
                        findingList.Add(neighbourTile);
                }
            }
        }

        return null;
    }

    private int GetH(PathTile a, PathTile b)
    {
        int dX = Mathf.Abs(a.x - b.x);
        int dZ = Mathf.Abs(a.z - b.z);
        int dXZ = Mathf.Abs(dX - dZ);
        return diagonalStepCost * Mathf.Min(dX, dZ) + straightStepCost * dXZ;
    }

    private PathTile GetMinFTile(List<PathTile> pathTileList)
    {
        PathTile cheapestFTile = pathTileList[0];
        for(int i = 1; i < pathTileList.Count; i++)
        {
            if(pathTileList[i].f < cheapestFTile.f)
            {
                cheapestFTile = pathTileList[i];
            }
        }
        return cheapestFTile;
    }

    private List<PathTile> GetPath(PathTile endTile)
    {
        List<PathTile> path = new List<PathTile>();
        path.Add(endTile);
        PathTile currentTile = endTile;
        while (currentTile.previousTile != null)
        {
            path.Add(currentTile.previousTile);
            currentTile = currentTile.previousTile;
        }
        path.Reverse();
        return path;
    }

    public PathTile GetTile(int x, int z)
    {
        return Grid.GetValue(x, z);
    }

    public PathTile GetTile(Vector3 worldPos) {
        Grid.WorldToGrid(worldPos, out int x, out int z);
        return Grid.GetValue(x, z);
    }

    private List<PathTile> GetNeighbourTiles(PathTile currentTile)
    {
        List<PathTile> neighbourTiles = new List<PathTile>();

        if(currentTile.x-1 >= 0)
        {
            neighbourTiles.Add(GetTile(currentTile.x - 1, currentTile.z));
            if (currentTile.z - 1 >= 0) neighbourTiles.Add(GetTile(currentTile.x - 1, currentTile.z - 1));
            if (currentTile.z + 1 < Grid.GetLength()) neighbourTiles.Add(GetTile(currentTile.x - 1, currentTile.z + 1));
        }
        if (currentTile.x + 1 < Grid.GetWidth())
        {
            neighbourTiles.Add(GetTile(currentTile.x + 1, currentTile.z));
            if (currentTile.z - 1 >= 0) neighbourTiles.Add(GetTile(currentTile.x + 1, currentTile.z - 1));
            if (currentTile.z + 1 < Grid.GetLength()) neighbourTiles.Add(GetTile(currentTile.x + 1, currentTile.z + 1));
        }
        if (currentTile.z - 1 >= 0) neighbourTiles.Add(GetTile(currentTile.x, currentTile.z - 1));
        if (currentTile.z + 1 < Grid.GetLength()) neighbourTiles.Add(GetTile(currentTile.x, currentTile.z + 1));

        return neighbourTiles;
    }

    public List<PathTile> FindPath(Vector3 startWorldPos, Vector3 endWorldPos, int range)
    {
        Grid.WorldToGrid(startWorldPos, out int startX, out int startZ);
        Grid.WorldToGrid(endWorldPos, out int endX, out int endZ);

        return FindPath(startX, startZ, endX, endZ, range);
        /*if (path == null)
            return null;
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathTile pathTile in path)
            {
                vectorPath.Add(new Vector3(pathTile.x, 0, pathTile.z) * grid.getScale() + new Vector3(1, 0, 1) * grid.getScale() * 0.5f + new Vector3(0, startWorldPos.y, 0));
            }
            return vectorPath;
        }*/
    }

    public Vector3 GetVector(PathTile pathTile, Vector3 worldPos) {
        return new Vector3(pathTile.x, 0, pathTile.z) * Grid.getScale() + new Vector3(1, 0, 1) * Grid.getScale() * 0.5f + new Vector3(0, worldPos.y, 0);
    }
}
