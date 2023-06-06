using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private void Start()
    {
        if (transform.GetComponentInParent<Unit>().Team != Test.playerTeam) {
            transform.Find("Bar").Find("BarSprite").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            transform.parent.Find("SelectionCircle").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }
    }

    void Update()
    {
        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.x = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
