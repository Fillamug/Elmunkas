using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    [SerializeField]
    private LayerMask selectLayer;

    private List<GameObject> selected;

    [HideInInspector]
    public List<GameObject> selectable;

    Vector3 startPos, endPos;

    private void Awake()
    {
        selected = new List<GameObject>();
        selectable = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = ControlHelper.GetMouseViewportPos();

            RaycastHit raycastHit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity, selectLayer))
            {
                Select select = raycastHit.collider.GetComponent<Select>();
                Unit unit = raycastHit.collider.GetComponentInParent<Unit>();

                if (Input.GetKey("left ctrl"))
                {
                    if (select.selected == false && unit.Team == Test.playerTeam)
                    {
                        selected.Add(raycastHit.collider.gameObject);
                        select.selected = true;
                        select.ClickMe();
                    }
                    else
                    {
                        selected.Remove(raycastHit.collider.gameObject);
                        select.selected = false;
                        select.ClickMe();
                    }
                }
                else
                {
                    ClearSelection();
                    if (unit.Team == Test.playerTeam)
                    {
                        selected.Add(raycastHit.collider.gameObject);
                        select.selected = true;
                        select.ClickMe();
                    }
                }
            }
            else CheckCtrl();
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPos = ControlHelper.GetMouseViewportPos();
            if (startPos != endPos)
                SelectUnits();
        }
    }

    private void SelectUnits()
    {
        List<GameObject> deSelect = new List<GameObject>();

        CheckCtrl();

        Rect selectRect = new Rect(startPos.x, startPos.y, endPos.x - startPos.x, endPos.y - startPos.y);

        foreach(GameObject gameObject in selectable)
        {
            if (gameObject != null)
            {
                if (selectRect.Contains(Camera.main.WorldToViewportPoint(gameObject.transform.position), true))
                {
                    if (gameObject.GetComponentInParent<Unit>().Team == Test.playerTeam)
                    {
                        selected.Add(gameObject);
                        gameObject.GetComponent<Select>().selected = true;
                        gameObject.GetComponent<Select>().ClickMe();
                    }
                }
            }
            else
                deSelect.Add(gameObject);
        }

        if (deSelect.Count > 0)
        {
            foreach(GameObject gameObject in deSelect)
            {
                selectable.Remove(gameObject);
            }
            deSelect.Clear();
        }
    }

    private void ClearSelection()
    {
        if (selected.Count > 0)
        {
            foreach (GameObject gameObject in selected)
            {
                if (gameObject != null)
                {
                    gameObject.GetComponent<Select>().selected = false;
                    gameObject.GetComponent<Select>().ClickMe();
                }
            }
            selected.Clear();
        }
    }

    private void CheckCtrl()
    {
        if (Input.GetKey("left ctrl") == false)
        {
            ClearSelection();
        }
    }
}
