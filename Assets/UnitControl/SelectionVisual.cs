using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionVisual : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    Vector3 startPos, endPos;

    private void Start()
    {
        rectTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity))
            {
                startPos = raycastHit.point;

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            rectTransform.gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(0))
        {
            if (!rectTransform.gameObject.activeInHierarchy)
            {
                rectTransform.gameObject.SetActive(true);
            }

            endPos = Input.mousePosition;
            Vector3 selectionStart = Camera.main.WorldToScreenPoint(startPos);
            selectionStart.z = 0;
            Vector3 selectionCenter = (selectionStart + endPos) / 2f;
            rectTransform.position = selectionCenter;

            float width = Mathf.Abs(selectionStart.x - endPos.x);
            float height = Mathf.Abs(selectionStart.y - endPos.y);
            rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
