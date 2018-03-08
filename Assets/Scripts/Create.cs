using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour {
    private void OnMouseDown()
    {
        print("Create");
        GameObject hex = Instantiate(Resources.Load("Obstacle") as GameObject);
        hex.transform.position = transform.position;
        Destroy(gameObject);
    }
}
