using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D body;

    float xInput;
    float yInput;

    

    void Start()
    {
        

    }


    void Update()
    {
        // Endast input här
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Physics här
        body.velocity = new Vector2(xInput * speed, yInput * speed);
    }
}
