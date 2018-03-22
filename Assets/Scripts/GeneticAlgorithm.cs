using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    GameObject[] Cars;
    System.Random random = new System.Random();
    [Range(2, 100)] public int GenerationSize = 50;
    [Range(0, 1000)] public int MaxNumGenerations = 10;
    [Range(2, 10)] public int NumElite;
    [Range(0, 1)] public float PercentMutate;
    [Range(0, 100)] public int GenerationTimeDelay;

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
        
        int NumMutate = (int)(PercentMutate * GenerationSize);
        print(NumElite);
        GameObject[] Elite = new GameObject[NumElite];
        float AvgFit = 0;
        for (int i = 0; i < GenerationSize; i++)
        {
            for(int q = 0; q < Elite.Length; q++)
            {
                if (Elite[q] != null)
                {
                    if (Cars[i].GetComponent<CarMovement>().fitness > Elite[q].GetComponent<CarMovement>().fitness)
                    {
                        Elite[q] = Cars[i];
                        break;
                    }
                }
                else
                {
                    Elite[q] = Cars[i];
                    break;
                }
            }
            AvgFit += Cars[i].GetComponent<CarMovement>().fitness;
        }

        print(AvgFit/GenerationSize);

        int Count = 0;
        GameObject[] NewGeneration = new GameObject[GenerationSize];

        for (int i = 0; i < Elite.Length; i++)
        {
            NewGeneration[Count] = Instantiate(Resources.Load("Car") as GameObject);
            NewGeneration[Count].GetComponent<NeuralNetwork>().Initialize(random);
            NewGeneration[Count].GetComponent<NeuralNetwork>().Set(Elite[i].GetComponent<NeuralNetwork>());
            Count++;
        }
/*
        for (int i = 0; i < NumMutate; i++)
        {
            NewGeneration[Count] = Instantiate(Resources.Load("Car") as GameObject);
            NewGeneration[Count].GetComponent<CarMovement>().InitBrain(random);
            Count++;
        }
*/
        for (int i = Count; i < GenerationSize; i++)
        {
            NeuralNetwork Elite1 = Elite[random.Next(0, NumElite - 1)].GetComponent<NeuralNetwork>();
            NeuralNetwork Elite2 = Elite[random.Next(0, NumElite - 1)].GetComponent<NeuralNetwork>();

            NewGeneration[i] = Instantiate(Resources.Load("Car") as GameObject);
            NewGeneration[i].GetComponent<NeuralNetwork>().Initialize(random);
            NewGeneration[i].GetComponent<NeuralNetwork>().Combine(Elite1, Elite2, PercentMutate);
        }
        
        for(int i = 0; i < GenerationSize; i++)
        {
            Destroy(Cars[i]);
            Cars[i] = NewGeneration[i];
        }
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
                Evolve();
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
