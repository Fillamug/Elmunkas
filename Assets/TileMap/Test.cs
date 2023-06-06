using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatHelper;

public class Test : MonoBehaviour
{
    [SerializeField]
    private TileMapVisual tileMapVisual;

    private TileMap tileMap;
    private Pathfinding pathfinding;

    public static Team playerTeam;

    void Start()
    {
        tileMap = new TileMap(40, 20, 10f, Vector3.zero);
        pathfinding = new Pathfinding(40, 20, 10f, Vector3.zero);

        tileMap.SetTileMapVisual(tileMapVisual);
        tileMap.SetTileTexture(new Vector3(80, 0, 80), TileMap.Tile.TileTexture.Grass);
        playerTeam = Team.RakosiMatyas;
    }
}
