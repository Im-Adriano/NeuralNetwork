using UnityEngine;
using System;

public class NeuralNetwork : MonoBehaviour
{

    public float[][] layers;
    public float[][][] NeuronWeights;
    public float[][] BiasWeights;
    float BiasNeuron = 1f;

    int hiddenLayers = 1;
    int inputNum = 5;
    int hiddenLayerSize = 4;
    int outputNum = 1;
    System.Random rand;

    // Use this for initialization
    public void Initialize(System.Random random)
    {
        rand = random;

        layers = new float[hiddenLayers + 2][];
        layers[0] = new float[inputNum];


        for (int i = 1; i <= hiddenLayers; i++)
        {
            layers[i] = new float[hiddenLayerSize];
        }

        layers[layers.Length - 1] = new float[outputNum];
        
        InitWeights();
        InitBiases();

    }

    void InitWeights()
    {
        NeuronWeights = new float[hiddenLayers + 1][][];
        for (int i = 0; i <= hiddenLayers; i++)
        {
            NeuronWeights[i] = new float[layers[i + 1].Length][];
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                NeuronWeights[i][j] = new float[layers[i].Length];
                for (int k = 0; k < layers[i].Length; k++)
                {
                    NeuronWeights[i][j][k] = (float)(rand.NextDouble() * 2) - 1;
                }
            }
        }
    }

    void InitBiases()
    {
        BiasWeights = new float[hiddenLayers + 1][];
        for (int i = 0; i <= hiddenLayers; i++)
        {
            BiasWeights[i] = new float[layers[i + 1].Length];
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                BiasWeights[i][j] = (float)(rand.NextDouble() - .5f);
            }
        }
    }

    public float Compute(float[] inputs)
    {
        for (int i = 0; i < layers[0].Length; i++)
        {
            layers[0][i] = inputs[i];
        }

        for (int i = 0; i < NeuronWeights.Length; i++)
        {
            for (int j = 0; j < NeuronWeights[i].Length; j++)
            {
                float sum = 0;
                for (int k = 0; k < NeuronWeights[i][j].Length; k++)
                {
                    sum += layers[i][k] * NeuronWeights[i][j][k];
                }
                sum += BiasWeights[i][j] * BiasNeuron;
                //layers[i + 1][j] = 1 / (float)(1 + Math.Pow(Math.E, -sum));  0 to 1
                layers[i + 1][j] = sum / (float) Math.Sqrt(1 + Math.Pow(sum, 2)); // -1 to 1
            }
        }

        return layers[layers.Length - 1][0];
    }

    public float[] Normalize(float[] data)
    {

        float mean = 0;
        float variance = 0;

        for(int i = 0; i < data.Length; i++)
        {
            mean += data[i];
            variance += (float) Math.Pow(data[i],2);
        }
        mean = mean / data.Length;
        variance = (variance / data.Length) - (float) Math.Pow(mean, 2);
        float[] ret = new float[data.Length];
        for(int i = 0; i < data.Length; i++)
        {
            ret[i] = (data[i] - mean) / (float)Math.Sqrt(variance);
        }

        return ret;
    }



    public void Combine(NeuralNetwork other, NeuralNetwork other2, float mutate)
    {
        
        for (int i = 0; i <= hiddenLayers; i++)
        {
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                for (int k = 0; k < layers[i].Length; k++)
                {
                    if (rand.NextDouble() < mutate)
                    {
                        NeuronWeights[i][j][k] = (float)(rand.NextDouble() * 2) - 1;
                    }
                    else {
                        if (rand.NextDouble() > .5)
                        {
                            NeuronWeights[i][j][k] = NeuronWeights[i][j][k];
                        }
                        else
                        {
                            NeuronWeights[i][j][k] = other2.NeuronWeights[i][j][k];
                        }
                    }
                }
            }
        }
        for (int i = 0; i <= hiddenLayers; i++)
        {
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                if (rand.NextDouble() < mutate)
                {
                    BiasWeights[i][j] = (float)(rand.NextDouble() - .5f);
                }
                else
                {
                    if (rand.NextDouble() > .5)
                    {
                        BiasWeights[i][j] = BiasWeights[i][j];
                    }
                    else
                    {
                        BiasWeights[i][j] = other2.BiasWeights[i][j];
                    }
                }

            }
        }
    }

    public void Set(NeuralNetwork other)
    {
        for (int i = 0; i <= hiddenLayers; i++)
        {
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                for (int k = 0; k < layers[i].Length; k++)
                {
                    NeuronWeights[i][j][k] = other.NeuronWeights[i][j][k];
                }
            }
        }
        for (int i = 0; i <= hiddenLayers; i++)
        {
            for (int j = 0; j < layers[i + 1].Length; j++)
            {
                BiasWeights[i][j] = other.BiasWeights[i][j];        

            }
        }
    }
}

