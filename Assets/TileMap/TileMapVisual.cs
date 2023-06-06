using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapVisual : MonoBehaviour
{
    [System.Serializable]
    public struct TileTextureUV
    {
        public TileMap.Tile.TileTexture tileTexture;
        public Vector2Int uv00, uv11;
    }

    struct UVCoordinates
    {
        public Vector2 uv00, uv11;
    }

    [SerializeField] private TileTextureUV[] tileTextureUVs;
    private Dictionary<TileMap.Tile.TileTexture, UVCoordinates> UVDictionary;
    private MyGrid<TileMap.Tile> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float width = texture.width;
        float height = texture.height;
        UVDictionary = new Dictionary<TileMap.Tile.TileTexture, UVCoordinates>();
        foreach (TileTextureUV tileTextureUV in tileTextureUVs)
        {
            UVDictionary[tileTextureUV.tileTexture] = new UVCoordinates
            {
                uv00 = new Vector2(tileTextureUV.uv00.x / width, tileTextureUV.uv00.y / height),
                uv11 = new Vector2(tileTextureUV.uv11.x / width, tileTextureUV.uv11.y / height)
            };
        }
    }

    private void UpdateTileMapVisual()
    {
        VisualHelper.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetLength(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for(int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetLength(); j++)
            {
                int k = i * grid.GetLength() + j;
                Vector3 quadScale = new Vector3(1, 0, 1) * grid.getScale();
                TileMap.Tile tile = grid.GetValue(i, j);
                TileMap.Tile.TileTexture tileTexture = tile.GetTileTexture();
                Vector2 uv00, uv11;
                if (tileTexture == TileMap.Tile.TileTexture.None)
                {
                    uv00 = Vector2.zero;
                    uv11 = Vector2.zero;
                    quadScale = Vector3.zero;
                }
                else
                {
                    UVCoordinates uVCoordinates = UVDictionary[tileTexture];
                    uv00 = uVCoordinates.uv00;
                    uv11 = uVCoordinates.uv11;
                }
                VisualHelper.AddToMeshArrays(vertices, uv, triangles, k, grid.GridToWorld(i, j) + quadScale * 0.5f, 0, quadScale, uv00, uv11);
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }

    public void SetGrid(MyGrid<TileMap.Tile> grid)
    {
        this.grid = grid;
        UpdateTileMapVisual();
        grid.GridEvent += GridChanged;
    }

    private void GridChanged(object sender, MyGrid<TileMap.Tile>.GridEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateTileMapVisual();
        }
    }
}
