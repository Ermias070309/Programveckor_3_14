using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    Vector3 startPoint;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.parent.position;
    }

   private void OnMouseDrag()
    {
        Vector3 newPositsion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPositsion.z = 0;

        transform.position = newPositsion;


        Vector3 direction = newPositsion - startPoint;
        transform.right = direction * transform.lossyScale.x;


        float dist = Vector2.Distance(startPoint, newPositsion);

    }
}
