using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete : MonoBehaviour {

    private void OnMouseDown()
    {
        GameObject GhostHex = Instantiate(Resources.Load("GhostObstacle") as GameObject);
        GhostHex.transform.position = transform.position;
        Destroy(gameObject);
    }
    
}
