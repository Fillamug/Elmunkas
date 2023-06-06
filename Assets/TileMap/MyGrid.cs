using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid<T>
{
    public class GridEventArgs : EventArgs
    {
        public int x, z;
    }
    public event EventHandler<GridEventArgs> GridEvent;

    private int width, length;
    private T[,] values;
    private float scale;
    Vector3 originPos;

    public MyGrid(int width, int length, float scale, Vector3 originPos, Func<MyGrid<T>, int, int, T> createGrid)
    {
        this.width = width;
        this.length = length;
        values = new T[width, length];
        this.scale = scale;
        this.originPos = originPos;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                values[i, j] = createGrid(this, i, j);
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetLength()
    {
        return length;
    }

    public float getScale()
    {
        return scale;
    }

    public Vector3 GridToWorld(int x, int z)
    {
        return new Vector3(x, 0, z) * scale;
    }

    public void WorldToGrid(Vector3 worldPos, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / scale);
        z = Mathf.FloorToInt((worldPos - originPos).z / scale);
    }

    public Vector3 RoundToGrid(Vector3 worldPos) {
        return new Vector3(Mathf.FloorToInt((worldPos - originPos).x / scale) + 0.5f, 0, Mathf.FloorToInt((worldPos - originPos).z / scale)  + 0.5f) * scale + new Vector3(0, worldPos.y, 0);
    }

    public void GridChanged(int x, int z)
    {
        GridEvent?.Invoke(this, new GridEventArgs { x = x, z = z });
    }

    public void SetValue(int x, int z, T t)
    {
        if (x >= 0 && z >= 0 && x <= width && z <= length)
        {
            values[x, z] = t;
            GridChanged(x, z);
        }
    }

    public void SetValue(Vector3 worldPos, T t)
    {
        int x, z;
        WorldToGrid(worldPos, out x, out z);
        SetValue(x, z, t);
    }

    public T GetValue (int x, int z)
    {
        if (x >= 0 && z >= 0 && x <= width && z <= length)
            return values[x, z];
        else
            return default;
    }

    public T GetValue(Vector3 worldPos)
    {
        int x, z;
        WorldToGrid(worldPos, out x, out z);
        return GetValue(x, z);
    }
}
