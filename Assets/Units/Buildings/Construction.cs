using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    [SerializeField] private Transform[] constructionTransform;
    private int constructionType;
    private bool buildingPlaced;

    private void Start()
    {
        constructionType = 0;
        buildingPlaced = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            Contruct();
        }
        else if (Input.GetKeyDown("t")) {
            if (constructionType == 0) constructionType = 1;
            else constructionType = 0;
        }
    }

    public int ConstructionType { get => constructionType; set => constructionType = value; }
    public bool BuildingPlaced { get => buildingPlaced; set => buildingPlaced = value; }

    public void Contruct() {
        Pathfinding.Instance.Grid.WorldToGrid(ControlHelper.GetMouseWorldPos(), out int x, out int z);
        List<Vector2Int> allPositions = constructionTransform[constructionType].GetComponent<Building>().getAllPositions(x, z);
        bool empty = true;
        foreach (Vector2Int position in allPositions)
        {
            if (!Pathfinding.Instance.Grid.GetValue(position.x, position.y).Empty) empty = false;
        }
        if (empty)
        {
            Transform transform = Instantiate(constructionTransform[constructionType], Pathfinding.Instance.Grid.GridToWorld(x, z) + new Vector3(5, 0, 5), Quaternion.identity);
            foreach (Vector2Int position in allPositions)
            {
                Pathfinding.Instance.Grid.GetValue(position.x, position.y).PresentUnit = transform;
                Pathfinding.Instance.GetTile(position.x, position.y).Empty = false;
            }
            buildingPlaced = true;
        }
    }
}
