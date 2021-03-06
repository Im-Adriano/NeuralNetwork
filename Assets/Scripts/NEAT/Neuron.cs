﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Neuron {
    
    public InnovationTable.NeuronType Type;
    public int ID;
    public float SplitX, SplitY;
    public float ActivationResponse;
    public float value;

    public Neuron(InnovationTable.NeuronType type, float x, float y, int id)
    {
        Type = type;
        SplitX = x;
        SplitY = y;
        ID = id;
        ActivationResponse = 0;
        value = 0;
    }

    public override bool Equals(object obj)
    {
        if(GetType() != obj.GetType())
        {
            return false;
        }
        Neuron temp = (Neuron)obj;
        return temp.ID == this.ID;
    }
}
