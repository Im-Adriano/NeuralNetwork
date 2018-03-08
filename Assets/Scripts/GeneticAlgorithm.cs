using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    GameObject[] Cars;
    System.Random random = new System.Random();
    public int GenerationSize = 50;
    public int MaxNumGenerations = 10;
    [Range(0, 1)] public float PercentElite;
    [Range(0, 1)] public float PercentMutate;
    [Range(0,100)]public int GenerationTimeDelay;

    int GenCounter = 0;
    int DelayCounter = 0;
    bool BeginGen = false;
    bool AllCrashed = false;
    bool GenRunning = false;

    // Use this for initialization
    private void Awake()
    {
        Cars = new GameObject[GenerationSize];
        for (int i = 0; i < GenerationSize; i++)
        {
            Cars[i] = GameObject.Instantiate(Resources.Load("Car") as GameObject);
            Cars[i].GetComponent<CarMovement>().InitBrain(random);
        }
    }
    public void Begin () {
        BeginGen = true;
    }
	
    void Evolve()
    {

    }

	// Update is called once per frame
	void FixedUpdate () {
        if (BeginGen)
        {
            for (int i = 0; i < GenerationSize; i++)
            {
                Cars[i].GetComponent<CarMovement>().StartCar();
            }
            BeginGen = false;
            GenRunning = true;
            DelayCounter = 0;
            GenCounter++;

            if(GenCounter >= MaxNumGenerations)
            {
                this.enabled = false;
            }
        }

        if (GenRunning)
        {
            AllCrashed = true;
            for (int i = 0; i < GenerationSize; i++)
            {
                if (!Cars[i].GetComponent<CarMovement>().isCrash())
                {
                    AllCrashed = false;
                }
            }
        }

        if (AllCrashed)
        {
            GenRunning = false;
            if (DelayCounter > GenerationTimeDelay)
            {
                print("Evolving");
                //Evolve generation
                for (int i = 0; i < GenerationSize; i++)
                {
                    Cars[i].GetComponent<CarMovement>().ResetCar();
                }
                BeginGen = true;
            }
            DelayCounter++;
        }

        
        
    }
}
