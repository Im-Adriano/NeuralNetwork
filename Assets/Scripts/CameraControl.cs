using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public  float Speed = 5;
    public int ZoomSpeed = 5;
    public int rotationSpeed = 10;

    public Camera c;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(h,v);
        movement = transform.TransformDirection(movement);
        movement *= Speed * Time.deltaTime;
        
        c.transform.Translate(movement);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            c.orthographicSize += ZoomSpeed * Time.deltaTime ;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            c.orthographicSize -= ZoomSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            c.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            c.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }


    }
}
