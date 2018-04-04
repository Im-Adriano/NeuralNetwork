using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synapse{
    public int IN;
    public int OUT;
    public float Weight;
    public bool Enabled;
    public int Innovation;


    public Synapse(int i, int o, float w, int inn)
    {
        IN = i;
        OUT = o;
        Weight = w;
        Innovation = inn;
        Enabled = true;
    }

}
