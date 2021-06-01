using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorRotator : MonoBehaviour
{
    [SerializeField] RectTransform cursor; 

    void Update()
    {
        cursor.Rotate(Vector3.forward);     
    }
}
