using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public SpriteRenderer wireEnd;
    public GameObject lightOn;
    Vector3 startPoint;
    Vector3 startPosition;
    bool isConnected = false;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.parent.position;
        startPosition = transform.position;
    }

   private void OnMouseDrag()
    {
        // Mouse point to world point
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;

        // chek for nerby conection points
        Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition, .2f);
        foreach (Collider2D collider in colliders)
        {
            // make sure not my collider
            if (collider.gameObject != gameObject)
            {
                // update wire to the conection poisition
                UpdateWire(collider.transform.position);
                isConnected = true;

                //check if wires are same color
                if(transform.parent.name.Equals(collider.transform.parent.name))
                {
                    // finish step
                    collider.GetComponent<Wire>()?.Done();
                    Done();
                }
                return;

            }

        }

        //update wire
        UpdateWire(newPosition);

    }

    void Done()
    {
        // turn on light
        lightOn.SetActive(true);

        //Destroy the script
        Destroy(this);
    }

    private void OnMouseUp()
    {
        // reset wire position
        if (!isConnected)
        {
            UpdateWire(startPosition);
        }
    }

    void UpdateWire(Vector3 newPosition)
    {
        // update position
        transform.position = newPosition;

       

        //update direction
        Vector3 direction = newPosition - startPoint;
        transform.right = direction * transform.lossyScale.x;


        
        // update scale
        float dist = Vector2.Distance(startPoint, newPosition);
        wireEnd.size = new Vector2(dist, wireEnd.size.y);
    }


}
