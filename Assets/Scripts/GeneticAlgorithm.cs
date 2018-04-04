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

    public float TopFit = 0;
    public float TopGenFit = 0;
    public List<float> GenFit = new List<float>();
    public List<float> Fit = new List<float>();
    UI ui;

    public GameObject TopCarBrain;
    

    // Use this for initialization
    private void Awake()
    {
        TopCarBrain = new GameObject();
        TopCarBrain.AddComponent<NeuralNetwork>();
        TopCarBrain.GetComponent<NeuralNetwork>().Initialize(random);


        Cars = new GameObject[GenerationSize];
        for (int i = 0; i < GenerationSize; i++)
        {
            Cars[i] = GameObject.Instantiate(Resources.Load("Car") as GameObject);
            Cars[i].GetComponent<CarMovement>().InitBrain(random);
        }

        ui = GetComponent<UI>();
    }

    public void Begin () {
        BeginGen = true;
    }
	
    void Evolve()
    {
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

            float fit = Cars[i].GetComponent<CarMovement>().fitness;
            AvgFit += fit;

            if (Cars[i].GetComponent<CarMovement>().fitness > TopFit)
            {
                TopFit = fit;
                TopCarBrain.GetComponent<NeuralNetwork>().Set(Cars[i].GetComponent<NeuralNetwork>());
                ui.printNeuralNetwork(TopCarBrain.GetComponent<NeuralNetwork>());
            }
        }

        Fit.Add(Elite[0].GetComponent<CarMovement>().fitness);

        float currentGenFit = AvgFit / GenerationSize;
        if(currentGenFit > TopGenFit)
        {
            TopGenFit = currentGenFit;
        }
        GenFit.Add(currentGenFit);


        int Count = 0;
        GameObject[] NewGeneration = new GameObject[GenerationSize];

        for (int i = 0; i < Elite.Length; i++)
        {
            NewGeneration[Count] = Instantiate(Resources.Load("Car") as GameObject);
            NewGeneration[Count].GetComponent<NeuralNetwork>().Initialize(random);
            NewGeneration[Count].GetComponent<NeuralNetwork>().Set(Elite[i].GetComponent<NeuralNetwork>());
            Count++;
        }

        for (int i = Count; i < GenerationSize; i++)
        {
            NeuralNetwork Elite1 = Elite[random.Next(0, NumElite - 1)].GetComponent<NeuralNetwork>();
            NeuralNetwork Elite2 = Elite[random.Next(0, NumElite - 1)].GetComponent<NeuralNetwork>();

            NewGeneration[i] = Instantiate(Resources.Load("Car") as GameObject);
            NewGeneration[i].GetComponent<NeuralNetwork>().Initialize(random);
            NewGeneration[i].GetComponent<NeuralNetwork>().Combine(Elite1, Elite2,PercentMutate);
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
                ui.GenGraph(GenFit, TopGenFit,ui.graphGenPanel,ui.graphGenChildren);
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
