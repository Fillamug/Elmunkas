using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Unit
{
    [SerializeField] private int length;
    [SerializeField] private int width;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        base.Attack(StopAttacking);
    }

    private int StopAttacking()
    {
        AttackTarget = null;
        return 0;
    }

    public List<Vector2Int> getAllPositions(int x, int z) {
        List<Vector2Int> allPositions = new List<Vector2Int>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                allPositions.Add(new Vector2Int(x + i, z + j));
            }
        }
        return allPositions;
    }

    protected override void RemoveFromTile() {
        Pathfinding.Instance.Grid.WorldToGrid(transform.position, out int x, out int z);
        List<Vector2Int> allPositions = getAllPositions(x, z);
        foreach (Vector2Int position in allPositions)
        {
            Pathfinding.Instance.Grid.GetValue(position.x, position.y).PresentUnit = null;
            Pathfinding.Instance.GetTile(position.x, position.y).Empty = true;
        }
    }

    protected override void AddToTile()
    {
        Pathfinding.Instance.Grid.WorldToGrid(transform.position, out int x, out int z);
        List<Vector2Int> allPositions = getAllPositions(x, z);
        foreach (Vector2Int position in allPositions)
        {
            Pathfinding.Instance.Grid.GetValue(position.x, position.y).PresentUnit = transform;
            Pathfinding.Instance.GetTile(position.x, position.y).Empty = false;
        }
    }
}
