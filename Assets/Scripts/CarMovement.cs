﻿using System.Collections;
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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        start = transform.position;
        brain = GetComponent<NeuralNetwork>();
    }



    void FixedUpdate()
    {
        if (!Crash)
        {
            float[] temp = { 1f, 2f, 20f, 10f, 8f };
            float h = (brain.Compute(brain.Normalize(temp)) );
            print(h);
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

    private void ResetCar()
    {
        this.enabled = false;
        fitness = 0;
        transform.position = start;
        Crash = false;
    }

    private void StartCar()
    {
        this.enabled = true;
    }
}
