﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public GameObject graphPanel;
    List<GameObject> graphChildren = new List<GameObject>();
    public GameObject dataPanel;
    List<GameObject> dataChildren = new List<GameObject>();


    public void printNeuralNetwork(NeuralNetwork NN)
    {
        for (int i = 0; i < dataChildren.Count; i++)
        {
            Destroy(dataChildren[i]);
        }

        float layerWidth = dataPanel.GetComponent<RectTransform>().rect.width / NN.layers.Length;

        GameObject[][] neurons = new GameObject[NN.layers.Length][];
        for (int i = 0; i < NN.layers.Length; i++)
        {
            float layerHeight = dataPanel.GetComponent<RectTransform>().rect.height / NN.layers[i].Length;
            neurons[i] = new GameObject[NN.layers[i].Length];
            for (int j = 0; j < NN.layers[i].Length; j++)
            {

                neurons[i][j] = Instantiate(Resources.Load("Neuron") as GameObject);
                neurons[i][j].transform.SetParent(dataPanel.transform);
                dataChildren.Add(neurons[i][j]);
                neurons[i][j].transform.localScale = Vector3.one;
                if (neurons[i].Length % 2 == 1)
                {
                    neurons[i][j].transform.localPosition = (-Vector3.right * layerWidth + (Vector3.right * (i * layerWidth))) + (Vector3.zero + (Vector3.up * ((j - (neurons[i].Length / 2)) * layerHeight)));
                }
                else
                {
                    neurons[i][j].transform.localPosition = (-Vector3.right * layerWidth + (Vector3.right * (i * layerWidth))) + (Vector3.zero + (Vector3.up * ((j + .5f - (neurons[i].Length / 2)) * layerHeight)));
                }


            }
        }

        for (int i = 0; i < NN.NeuronWeights.Length; i++)
        {
            for (int j = 0; j < NN.NeuronWeights[i].Length; j++)
            {
                for (int k = 0; k < NN.NeuronWeights[i][j].Length; k++)
                {
                    
                    Vector3 differenceVector = neurons[i][k].transform.position - neurons[i + 1][j].transform.position;
                    GameObject t = new GameObject();
                    t.transform.SetParent(dataPanel.transform);
                    dataChildren.Add(t);
                    t.AddComponent<Image>();
                    RectTransform rt = t.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(differenceVector.magnitude, 4f);
                    rt.pivot = new Vector2(0, 0.5f);
                    rt.position = neurons[i + 1][j].transform.position;
                   
                    float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
                    rt.rotation = Quaternion.Euler(0, 0, angle);

                    Image im = rt.GetComponent<Image>();
                    if (NN.NeuronWeights[i][j][k] > 0)
                    {
                        Color32 green = new Color32(161, 255, 171, (byte)(NN.NeuronWeights[i][j][k] * 255));
                        im.color = green;

                    }
                    else
                    {

                        Color32 red = new Color32(255, 161, 161, (byte)(NN.NeuronWeights[i][j][k] * -255));
                        im.color = red;
                    }
                }
            }
        }
    }

    public void GenGraph(List<float> eval,float max)
    {
        for(int i = 0; i < graphChildren.Count; i++)
        {
            Destroy(graphChildren[i]);
        }

        Color32[] gCol = new Color32[]
        {
            new Color32(161,255,171,255),
            new Color32(255,161,161,255),
            new Color32(255,252,161,255),
        };

        
        float graphBarWidth = graphPanel.GetComponent<RectTransform>().rect.width / eval.Count;
        float topOfGraph = graphPanel.GetComponent<RectTransform>().rect.height;
     

        for (int i = 0; i < eval.Count; i++)
        {
            GameObject t = new GameObject();
            t.transform.SetParent(graphPanel.transform);
            graphChildren.Add(t);
            t.AddComponent<Image>();
            RectTransform rt = t.GetComponent<RectTransform>();
            rt.transform.localPosition = Vector3.zero + (Vector3.right * (i * graphBarWidth));
            rt.transform.localScale = Vector3.one;
            rt.pivot = Vector2.zero;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.localEulerAngles = Vector3.zero;
            rt.sizeDelta = new Vector2(graphBarWidth, (eval[i] / max) * topOfGraph);

            Image im = rt.GetComponent<Image>();

            if (eval[i] == max)
            {
                im.color = gCol[2];
            }
            else if (i == 0 || eval[i] > eval[i - 1])
            {
                im.color = gCol[0];
            }
            else
            {
                im.color = gCol[1];
            }

        }
    }

}
