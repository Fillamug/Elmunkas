using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    [SerializeField] private Transform constructionTransform;

    private void Update()
    {
        if (Input.GetKeyDown("b")) {
            Pathfinding.Instance.Grid.WorldToGrid(ControlHelper.GetMouseWorldPos(), out int x, out int z);
            PathTile constructionTile = Pathfinding.Instance.Grid.GetValue(x, z);
            if (constructionTile.Empty)
            {
                constructionTile.PresentUnit = Instantiate(constructionTransform, Pathfinding.Instance.Grid.GridToWorld(x, z)+new Vector3(5, 0, 5), Quaternion.identity);
                Pathfinding.Instance.GetTile(x, z).Empty = false;
            }
            else
            {

            }
        }
    }
}
