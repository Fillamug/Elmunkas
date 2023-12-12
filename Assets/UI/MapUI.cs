using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument mapUI;
    [SerializeField]
    private RenderTexture miniMap;

    // Start is called before the first frame update
    void Start()
    {
        mapUI.rootVisualElement.Q<Button>("FormationButton").clicked += () => Camera.main.gameObject.GetComponent<UnitControl>().SwitchFormation();
        mapUI.rootVisualElement.Q<VisualElement>("MiniMap").style.backgroundImage = new StyleBackground(Background.FromRenderTexture(miniMap));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSingleSelection(GameObject selectedUnit) {
        mapUI.rootVisualElement.Q<VisualElement>("NoUnit").style.display = DisplayStyle.None;
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").style.display = DisplayStyle.Flex;
        mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").style.display = DisplayStyle.None;
        for (int i = 0; i < 20; i++) {
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").style.display = DisplayStyle.None;
        }
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<VisualElement>("UnitIcon").style.backgroundImage = selectedUnit.GetComponentInParent<Unit>().Stats.unitIcon;
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<VisualElement>("HealthBar").style.width = new Length((float)selectedUnit.GetComponentInParent<Unit>().Health / selectedUnit.GetComponentInParent<Unit>().Stats.maxHealth * 100, LengthUnit.Percent);
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<Label>("UnitName").text = selectedUnit.GetComponentInParent<Unit>().Stats.unitName;
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<Label>("Health").text = "Health: " + selectedUnit.GetComponentInParent<Unit>().Health.ToString() + "/" + selectedUnit.GetComponentInParent<Unit>().Stats.maxHealth.ToString();
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<Label>("DPS").text = "DPS: " + (selectedUnit.GetComponentInParent<Unit>().Stats.attackDamage / selectedUnit.GetComponentInParent<Unit>().Stats.attackSpeed).ToString();
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<Label>("DamageType").text = "Damage Type: " + selectedUnit.GetComponentInParent<Unit>().Stats.attackType.ToString();
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").Q<Label>("ArmorType").text = "Armor Type: " + selectedUnit.GetComponentInParent<Unit>().Stats.armorType.ToString();
    }

    public void SetMultipleSelection(List<GameObject> selectedUnits) {
        mapUI.rootVisualElement.Q<VisualElement>("NoUnit").style.display = DisplayStyle.None;
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").style.display = DisplayStyle.None;
        mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").style.display = DisplayStyle.Flex;
        int i = 0;
        foreach (GameObject selectedUnit in selectedUnits)
        {
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").Q<VisualElement>("UnitIcon").style.backgroundImage = selectedUnit.GetComponentInParent<Unit>().Stats.unitIcon;
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").Q<VisualElement>("HealthBar").style.width = new Length((float)selectedUnit.GetComponentInParent<Unit>().Health / selectedUnit.GetComponentInParent<Unit>().Stats.maxHealth * 100, LengthUnit.Percent);
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").Q<Button>("Button").clicked += () => { Camera.main.gameObject.GetComponent<UnitControl>().ClearSelection(); Camera.main.gameObject.GetComponent<UnitControl>().AddToSelection(selectedUnit); };
            i++;
        }
    }

    public void ClearSelectionUI() {
        mapUI.rootVisualElement.Q<VisualElement>("NoUnit").style.display = DisplayStyle.Flex;
        mapUI.rootVisualElement.Q<VisualElement>("SingleUnit").style.display = DisplayStyle.None;
        mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").style.display = DisplayStyle.None;
        for (int i = 0; i < 20; i++)
        {
            mapUI.rootVisualElement.Q<VisualElement>("MultipleUnits").Q<VisualElement>($"Cell{i}").style.display = DisplayStyle.None;
        }
    }

    public bool IsOverUI(Vector3 pos) {
        return mapUI.rootVisualElement.Q<VisualElement>("TopBar").ContainsPoint(mapUI.rootVisualElement.Q<VisualElement>("TopBar").WorldToLocal(pos)) || mapUI.rootVisualElement.Q<VisualElement>("SideBar").ContainsPoint(mapUI.rootVisualElement.Q<VisualElement>("SideBar").WorldToLocal(pos));
    }

    public void ShowControlButtons(GameObject selectedUnit) {
        HideControlButtons();
        mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell1").style.display = DisplayStyle.Flex;
        mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell1").Q<Button>("Button").clicked += () => Camera.main.gameObject.GetComponent<UnitControl>().StopMovement();
        if (selectedUnit.GetComponentInParent<Moveable>() != null) {
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell0").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell0").Q<Button>("Button").clicked += () => Camera.main.gameObject.GetComponent<UnitControl>().MoveButtonPressed = true;
        }
        if (selectedUnit.GetComponentInParent<Unit>().Stats.attackDamage != 0) {
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell2").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell2").Q<Button>("Button").clicked += () => Camera.main.gameObject.GetComponent<UnitControl>().AttackButtonPressed = true;
        }
        if (selectedUnit.GetComponentInParent<Unit>().Stats.name == "Worker")
        {
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell3").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell3").Q<Button>("Button").clicked += () => Camera.main.gameObject.GetComponent<UnitControl>().BuildButtonPressed = true;
        }
        if (Camera.main.gameObject.GetComponent<UnitControl>().BuildButtonPressed) {
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell4").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell4").Q<Button>("Button").clicked += () => GameObject.Find("Construction").GetComponent<Construction>().ConstructionType = 0;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell5").style.display = DisplayStyle.Flex;
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>("Cell5").Q<Button>("Button").clicked += () => GameObject.Find("Construction").GetComponent<Construction>().ConstructionType = 1;
        }
    }

    public void HideControlButtons() {
        for (int i = 0; i < 8; i++)
        {
            mapUI.rootVisualElement.Q<VisualElement>("UnitControls").Q<VisualElement>($"Cell{i}").style.display = DisplayStyle.None;
        }
    }
}
