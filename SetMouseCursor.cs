using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMouseCursor : MonoBehaviour
{
    [SerializeField] RectTransform cursorImage;

    public bool setCursor = false; 


    private void Update()
    {
        if (!setCursor) return; 

        Plane plane = new Plane(Vector3.up, -0.2f);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            cursorImage.position = worldPosition; 
        }
    }
}
