using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    [HideInInspector]
    public bool selected = false;

    private void Start()
    {
        Camera.main.gameObject.GetComponent<UnitControl>().selectable.Add(this.gameObject);
        ClickMe();
    }

    public void ClickMe()
    {
        if (selected == false)
        {
            transform.parent.Find("HealthBar").gameObject.SetActive(false);
            transform.parent.Find("SelectionCircle").gameObject.SetActive(false);
            if (transform.parent.GetComponent<Unit>().AttackTarget != null) {
                transform.parent.GetComponent<Unit>().AttackTarget.Find("HealthBar").gameObject.SetActive(false);
                transform.parent.GetComponent<Unit>().AttackTarget.Find("SelectionCircle").gameObject.SetActive(false);
            }
        }
        else
        {
            transform.parent.Find("HealthBar").gameObject.SetActive(true);
            transform.parent.Find("SelectionCircle").gameObject.SetActive(true);
            if (transform.parent.GetComponent<Unit>().AttackTarget != null)
            {
                transform.parent.GetComponent<Unit>().AttackTarget.Find("HealthBar").gameObject.SetActive(true);
                transform.parent.GetComponent<Unit>().AttackTarget.Find("SelectionCircle").gameObject.SetActive(true);
            }
        }
    }
}
