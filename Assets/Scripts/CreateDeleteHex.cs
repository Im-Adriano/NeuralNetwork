using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDeleteHex : MonoBehaviour {

    public bool active = true;

    private void OnMouseDown()
    {
        if (active)
        {
            SpriteRenderer render = GetComponent<SpriteRenderer>();
            Color change = render.color;
            if (gameObject.layer == 0)
            {
                change.a = 1f;
                render.color = change;
                gameObject.layer = 8;
                tag = "Obstacle";
            }
            else
            {
                change.a = .2f;
                render.color = change;
                gameObject.layer = 0;
                tag = "Untagged";
            }
        }
    }
}
