using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

    bool Crash = false;
    [SerializeField] int fitness = 0;
    public float MaxSpeed;
    public float steering;
    NeuralNetwork brain;
    
    private Rigidbody2D rb;
    Vector2 start;

    float maxDistance = 1000;

    void Awake()
    {
        this.enabled = false;
        brain = GetComponent<NeuralNetwork>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        start = transform.position;
    }

    public void InitBrain(System.Random random)
    {
        brain.Initialize(random);
    }

    void FixedUpdate()
    {
        if (!Crash)
        {
 

            RaycastHit2D LHit = Physics2D.Raycast(transform.position, -transform.right,LayerMask.GetMask("Obstacle"));
            RaycastHit2D LFHit = Physics2D.Raycast(transform.position, transform.forward + transform.right, LayerMask.GetMask("Obstacle"));
            RaycastHit2D FHit = Physics2D.Raycast(transform.position, transform.forward, LayerMask.GetMask("Obstacle"));
            RaycastHit2D RFHit = Physics2D.Raycast(transform.position, transform.forward - transform.right, LayerMask.GetMask("Obstacle"));
            RaycastHit2D RHit = Physics2D.Raycast(transform.position, transform.right, LayerMask.GetMask("Obstacle"));

            print(LHit.collider.tag);

            float L = (LHit.collider.tag == "Obstacle" ) ? LHit.distance : maxDistance;
            float LF = (LFHit.collider.tag == "Obstacle") ? LFHit.distance : maxDistance;
            float F = (FHit.collider.tag == "Obstacle") ? FHit.distance : maxDistance;
            float RF = (RFHit.collider.tag == "Obstacle") ? RFHit.distance : maxDistance;
            float R = (RHit.collider.tag == "Obstacle") ? RHit.distance : maxDistance;

            print(L);
            print(LF);
            print(F);
            print(RF);
            print(R);

            float[] Distances = { L,LF,F,RF,R };

            float h = (brain.Compute(brain.Normalize(Distances)) );
            
            float v = 1;

            Vector2 speed = transform.up * v * MaxSpeed;
            rb.velocity = speed;

            float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
            if (direction >= 0.0f)
            {
                rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);

            }
            else
            {
                rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);

            }
            fitness++;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            Crash = true;
        }
    }

    public void ResetCar()
    {
        this.enabled = false;
        fitness = 0;
        transform.position = start;
        Crash = false;
    }

    public void StartCar()
    {
        this.enabled = true;
    }
}
