using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

    bool Crash = false;
    public float fitness = 0;
    public float MaxSpeed;
    public float steering;
    NeuralNetwork brain;
    


    private Rigidbody2D rb;
    Vector2 start;
    Quaternion startRot;

    void Awake()
    {
        this.enabled = false;
        brain = GetComponent<NeuralNetwork>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        start = transform.position;
        startRot = transform.rotation;
    }

    public void InitBrain(System.Random random)
    {
        brain.Initialize(random);
    }

    void FixedUpdate()
    {
        Vector2 pos = new Vector2(transform.position.x,transform.position.y);
        if (!Crash)
        {

            RaycastHit2D LHit = Physics2D.Raycast(pos, -Vector2.right, Mathf.Infinity,  LayerMask.GetMask("Obstacle"));
            RaycastHit2D LFHit = Physics2D.Raycast(pos, Vector2.up + Vector2.right, Mathf.Infinity,  LayerMask.GetMask("Obstacle"));
            RaycastHit2D FHit = Physics2D.Raycast(pos, Vector2.up, Mathf.Infinity,  LayerMask.GetMask("Obstacle"));
            RaycastHit2D RFHit = Physics2D.Raycast(pos, Vector2.up - Vector2.right, Mathf.Infinity,  LayerMask.GetMask("Obstacle"));
            RaycastHit2D RHit = Physics2D.Raycast(pos, Vector2.right, Mathf.Infinity,  LayerMask.GetMask("Obstacle"));

            
            float L = LHit.distance ;
            float LF =  LFHit.distance ;
            float F =  FHit.distance;
            float RF =  RFHit.distance ;
            float R =  RHit.distance ;

            fitness += (L + LF + F + RF + R) / 50;

            float[] Distances = { L,LF,F,RF,R };
            float h = (brain.Compute(brain.Normalize(Distances)) );
            
            if (float.IsNaN(h))
            {
                h = 0;
            }
         
            float v = 1;

            Vector2 speed = transform.up * v * MaxSpeed;
            rb.velocity = speed;

            rb.rotation += h * steering;
            fitness += (L + LF + F + RF + R) / 50;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.enabled == true)
        {
            if (collision.tag == "Obstacle")
            {
                Crash = true;
            }
        }
    }

    public void ResetCar()
    {
        
        this.enabled = false;
        rb.velocity = Vector2.zero;
        fitness = 0;
        transform.position = start;
        transform.rotation = startRot;
        Crash = false;
    }

    public bool isCrash()
    {
        return Crash;
    }

    public void StartCar()
    {
        this.enabled = true;
    }
}
