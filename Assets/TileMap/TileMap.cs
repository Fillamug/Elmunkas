using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap
{
    public class Tile
    {
        public enum TileTexture
        {
            Grass,
            None
        }

        private MyGrid<Tile> grid;
        private int x, z;
        private TileTexture tileTexture;

        public Tile(MyGrid<Tile> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTileTexture(TileTexture tileTexture)
        {
            this.tileTexture = tileTexture;
            grid.GridChanged(x, z);
        }

        public TileTexture GetTileTexture()
        {
            return tileTexture;
        }
    }

    private MyGrid<Tile> grid;

    public TileMap(int width, int length, float scale, Vector3 originPos)
    {
        grid = new MyGrid<Tile>(width, length, scale, originPos, (MyGrid<Tile> g, int x, int z) => new Tile(g, x, z));
    }

    public void SetTileTexture(Vector3 worldPos, Tile.TileTexture tileTexture)
    {
        Tile tile = grid.GetValue(worldPos);
        if (tile != null)
            tile.SetTileTexture(tileTexture);
    }

    public void SetTileMapVisual(TileMapVisual tileMapVisual)
    {
        tileMapVisual.SetGrid(grid);
    }
}
