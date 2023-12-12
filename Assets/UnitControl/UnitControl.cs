using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitControl : MonoBehaviour
{
    [SerializeField]
    private LayerMask selectLayer;

    private List<GameObject> selected;

    [HideInInspector]
    public List<GameObject> selectable;

    private Vector3 startPos, endPos;

    private List<Vector3> squarePositions, circlePositions, arrowPositions, currentPositions;

    private bool moveButtonPressed, attackButtonPressed, buildButtonPressed;

    private void Awake()
    {
        selected = new List<GameObject>();
        selectable = new List<GameObject>();
        squarePositions = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 0),
                new Vector3(20, 0, 0),
                new Vector3(-20, 0, 0),
                new Vector3(0, 0, 10),
                new Vector3(10, 0, 10),
                new Vector3(-10, 0, 10),
                new Vector3(20, 0, 10),
                new Vector3(-20, 0, 10),
                new Vector3(0, 0, 20),
                new Vector3(10, 0, 20),
                new Vector3(-10, 0, 20),
                new Vector3(20, 0, 20),
                new Vector3(-20, 0, 20),
                new Vector3(0, 0, 30),
                new Vector3(10, 0, 30),
                new Vector3(-10, 0, 30),
                new Vector3(20, 0, 30),
                new Vector3(-20, 0, 30),
            };
        circlePositions = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 10),
                new Vector3(-10, 0, 10),
                new Vector3(0, 0, 20),
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 0),
                new Vector3(10, 0, 20),
                new Vector3(-10, 0, 20),
                new Vector3(0, 0, -10),
                new Vector3(20, 0, 10),
                new Vector3(-20, 0, 10),
                new Vector3(0, 0, 30),
                new Vector3(10, 0, -10),
                new Vector3(-10, 0, -10),
                new Vector3(20, 0, 0),
                new Vector3(-20, 0, 0),
                new Vector3(20, 0, 20),
                new Vector3(-20, 0, 20),
                new Vector3(10, 0, 30),
                new Vector3(-10, 0, 30),
            };
        arrowPositions = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 10),
                new Vector3(10, 0, 10),
                new Vector3(-10, 0, 10),
                new Vector3(0, 0, 20),
                new Vector3(10, 0, 20),
                new Vector3(-10, 0, 20),
                new Vector3(20, 0, 20),
                new Vector3(-20, 0, 20),
                new Vector3(0, 0, 30),
                new Vector3(10, 0, 30),
                new Vector3(-10, 0, 30),
                new Vector3(20, 0, 30),
                new Vector3(-20, 0, 30),
                new Vector3(30, 0, 30),
                new Vector3(-30, 0, 30),
                new Vector3(20, 0, 40),
                new Vector3(-20, 0, 40),
                new Vector3(30, 0, 40),
                new Vector3(-30, 0, 40),
            };
        currentPositions = squarePositions;
        moveButtonPressed = false;
        attackButtonPressed = false;
        buildButtonPressed = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameObject.Find("MapUI").GetComponent<MapUI>().IsOverUI(Input.mousePosition))
        {
            if (!moveButtonPressed && !attackButtonPressed && !BuildButtonPressed)
            {
                startPos = ControlHelper.GetMouseViewportPos();

                RaycastHit raycastHit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity, selectLayer))
                {
                    Select select = raycastHit.collider.GetComponent<Select>();
                    Unit unit = raycastHit.collider.GetComponentInParent<Unit>();

                    if (Input.GetKey("left ctrl"))
                    {
                        if (select.selected == false && unit.Team == Test.playerTeam && selected.Count < 20)
                        {
                            AddToSelection(raycastHit.collider.gameObject);
                        }
                        else
                        {
                            RemoveFromSelection(raycastHit.collider.gameObject);
                        }
                    }
                    else
                    {
                        ClearSelection();
                        if (unit.Team == Test.playerTeam && selected.Count < 20)
                        {
                            AddToSelection(raycastHit.collider.gameObject);
                        }
                    }
                }
                else CheckCtrl();
            }
            else if (!buildButtonPressed)
            {
                Move();
            }
            else 
            GameObject.Find("Construction").GetComponent<Construction>().Contruct();
        }
        if (Input.GetMouseButtonUp(0) && !GameObject.Find("MapUI").GetComponent<MapUI>().IsOverUI(Input.mousePosition))
        {
            if (!moveButtonPressed && !attackButtonPressed && !buildButtonPressed)
            {
                endPos = ControlHelper.GetMouseViewportPos();
                if (startPos != endPos)
                    SelectUnits();
            }
            else
            {
                moveButtonPressed = false;
                attackButtonPressed = false;
                if (GameObject.Find("Construction").GetComponent<Construction>().BuildingPlaced) {
                    buildButtonPressed = false;
                    GameObject.Find("Construction").GetComponent<Construction>().BuildingPlaced = false;
                }
            }
        }
        if (Input.GetMouseButtonDown(1) && !GameObject.Find("MapUI").GetComponent<MapUI>().IsOverUI(Input.mousePosition))
        {
            Move();
        }
        if (Input.GetKeyDown("v")) {
            SwitchFormation();
        }
        SetSelectionUI();
        ControlButtonsUI();
    }

    public void SwitchFormation() {
        if (currentPositions == squarePositions)
        {
            currentPositions = circlePositions;
        }
        else if (currentPositions == circlePositions)
        {
            currentPositions = arrowPositions;
        }
        else if (currentPositions == arrowPositions)
        {
            currentPositions = squarePositions;
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
                    if (gameObject.GetComponentInParent<Unit>().Team == Test.playerTeam && selected.Count < 20)
                    {
                        AddToSelection(gameObject);
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

    public void ClearSelection()
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

    private void SetSelectionUI() {
        GameObject.Find("MapUI").GetComponent<MapUI>().ClearSelectionUI();
        if (selected.Count == 1)
        {
            GameObject.Find("MapUI").GetComponent<MapUI>().SetSingleSelection(selected[0]);
        }
        else
        {
            GameObject.Find("MapUI").GetComponent<MapUI>().SetMultipleSelection(selected);
        }
    }

    public void RemoveFromSelection(GameObject gameObject) {
        selected.Remove(gameObject);
        gameObject.GetComponent<Select>().selected = false;
        gameObject.GetComponent<Select>().ClickMe();
    }

    public void AddToSelection(GameObject gameObject) {
        selected.Add(gameObject);
        gameObject.GetComponent<Select>().selected = true;
        gameObject.GetComponent<Select>().ClickMe();
    }

    public void StopMovement() {
        foreach(GameObject selectedObject in selected)
        {
            selectedObject.GetComponentInParent<Moveable>().SetEndPos(selectedObject.transform.position, selectedObject.transform.position);
        }
    }

    private void ControlButtonsUI() {
        if (selected.Count == 0)
        {
            GameObject.Find("MapUI").GetComponent<MapUI>().HideControlButtons();
        }
        else
        {
            GameObject.Find("MapUI").GetComponent<MapUI>().ShowControlButtons(selected[0]);
        }
    }

    public bool MoveButtonPressed { get => moveButtonPressed; set => moveButtonPressed = value; }
    public bool AttackButtonPressed { get => attackButtonPressed; set => attackButtonPressed = value; }
    public bool BuildButtonPressed { get => buildButtonPressed; set => buildButtonPressed = value; }

    private void Move()
    {
        Vector3 mousePosition = ControlHelper.GetMouseWorldPos();

        int i = 0;
        Vector3 unitPosition = new Vector3(0, 0, 0);
        foreach (GameObject selectedObject in selected)
        {
            if (selectedObject.GetComponentInParent<Moveable>() != null)
            {
                unitPosition += selectedObject.transform.position;
                i++;
            }
        }
        unitPosition = unitPosition / i;
        i = 0;
        foreach (GameObject selectedObject in selected)
        {
            if (selectedObject.GetComponentInParent<Moveable>() != null)
            {
                if (mousePosition.x - unitPosition.x >= 0 && mousePosition.z - unitPosition.z >= 0)
                {
                    if (moveButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 180, 0) * currentPositions[i], selectedObject.transform.position);
                    }
                    else if (attackButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(selectedObject.transform.position, mousePosition);
                    }
                    else
                    selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 180, 0) * currentPositions[i], mousePosition);
                }
                else if (mousePosition.x - unitPosition.x >= 0 && mousePosition.z - unitPosition.z < 0)
                {
                    if (moveButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, -90, 0) * currentPositions[i], selectedObject.transform.position);
                    }
                    else if (attackButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(selectedObject.transform.position, mousePosition);
                    }
                    else
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, -90, 0) * currentPositions[i], mousePosition);
                }
                else if (mousePosition.x - unitPosition.x < 0 && mousePosition.z - unitPosition.z >= 0)
                {
                    if (moveButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 90, 0) * currentPositions[i], selectedObject.transform.position);
                    }
                    else if (attackButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(selectedObject.transform.position, mousePosition);
                    }
                    else
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 90, 0) * currentPositions[i], mousePosition);
                }
                else if (mousePosition.x - unitPosition.x < 0 && mousePosition.z - unitPosition.z < 0)
                {
                    if (moveButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 0, 0) * currentPositions[i], selectedObject.transform.position);
                    }
                    else if (attackButtonPressed)
                    {
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(selectedObject.transform.position, mousePosition);
                    }
                    else
                        selectedObject.GetComponentInParent<Moveable>().SetEndPos(mousePosition + Quaternion.Euler(0, 0, 0) * currentPositions[i], mousePosition);
                }
                i++;
            }
        }
    }
}
