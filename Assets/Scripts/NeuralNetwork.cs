using UnityEngine;
using System;

public class NeuralNetwork : MonoBehaviour {

    float[][] layers;
    float[][][] weights;
    float[][] biases;

    int hiddenLayers = 1;
    int inputNum = 5;
    int hiddenLayerSize = 4;
    int outputNum = 1;
    System.Random rand = new System.Random();

    // Use this for initialization
    void Start () {
        int[][] layers = new int[hiddenLayers+2][];
        layers[0] = new int[inputNum];
        

        for(int i = 1; i <= hiddenLayers; i++)
        {
            layers[i] = new int[hiddenLayerSize];
        }

        layers[layers.Length - 1] = new int[outputNum];

        InitWeights();
        InitBiases();

	}
	
    void InitWeights()
    {
        for (int i = 0; i < hiddenLayers + 1; i++)
        {
            weights[i] = new float[layers[i + 1].Length][];
            for(int j = 0; j < layers[i + 1].Length; j++)
            {
                weights[i][j] = new float[layers[i].Length];
                for(int k = 0; k < layers[i].Length; k++)
                {
                    weights[i][j][k] = (float)(rand.NextDouble() * 2) - 1;
                }
            }
        }
    }

    void InitBiases()
    {
        for (int i = 0; i < hiddenLayers + 1; i++)
        {
            biases[i] = new float[layers[i+1].Length - 1];
            for (int j = 0; j < layers[i+1].Length; j++)
            {
                biases[i][j] = (float)(rand.NextDouble() - .5);
            }
        }
    }

    public float Compute(float[] inputs)
    {
        for (int i = 0; i < layers[0].Length; i++)
        {
            layers[0][i] = inputs[i];
        }


        return layers[layers.Length - 1][0];
    }

}
