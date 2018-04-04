using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {
    int ID;
    List<Neuron> Neurons;
    List<Synapse> Synapses;
    float Fitness;
    float AdjustedFitness;
    float NumOffsring;
    int NumInputs, NumOutputs;
    int Speices;
    System.Random random = new System.Random();

    public Network(int id, List<Neuron> n, List<Synapse> s, int i, int o)
    {
        ID = id;
        Neurons = n;
        Synapses = s;
        NumInputs = i;
        NumOutputs = o;
    }

    void AddLink(float mutationRate, int numTrys, InnovationTable table)
    {
        if (random.NextDouble() > mutationRate) return;
        int Neuron1 = -1;
        int Neuron2 = -1;
        while (numTrys > 0)
        {
            Neuron N1 = Neurons[random.Next(0, Neurons.Count - 1)];
            Neuron N2 = Neurons[random.Next(NumInputs + 1, Neurons.Count - 1)];
            Neuron1 = N1.ID;
            Neuron2 = N2.ID;

            if(!DuplicateLink(Neuron1,Neuron2) && (Neuron1 != Neuron2) && (N1.SplitY < N2.SplitY))
            {
                numTrys = 0;
            }
            else
            {
                Neuron1 = -1;
                Neuron2 = -1;
            }
            numTrys--;
        }

        if(Neuron1 < 0 || Neuron2 < 0)
        {
            return;
        }

        int InnovationNum = table.GetInnovationNumber(Neuron1,Neuron2,InnovationTable.InnovationType.NewLink);

        if (InnovationNum > 0)
        {
            Synapses.Add(new Synapse(Neuron1,Neuron2,1,InnovationNum));
        }
        else
        {
            int NewInnovationNum = table.CreateNewInnovation(Neuron1, Neuron2, InnovationTable.InnovationType.NewLink);
            Synapses.Add(new Synapse(Neuron1, Neuron2, 1, NewInnovationNum));
        }
    }

    void AddNeuron(float mutationRate, int numTrys, InnovationTable table)
    {
        if (random.NextDouble() > mutationRate) return;
        bool done = false;
        int IN = -1;
        int OUT = -1;
        float oldWeight = 1;
        while (numTrys > 0)
        {
            Synapse S1 = Synapses[random.Next(0, Synapses.Count - 1 - (int)Mathf.Sqrt(Synapses.Count))];
            
            if (S1.Enabled)
            {
                numTrys = 0;
                done = true;
                IN = S1.IN;
                OUT = S1.OUT;
                oldWeight = S1.Weight;
            }
           
            numTrys--;
        }

        if (!done)
        {
            return;
        }

        float newDepth = (Neurons[GetElementPos(IN)].SplitY + Neurons[GetElementPos(OUT)].SplitY) / 2;
        float newWidth = (Neurons[GetElementPos(IN)].SplitX + Neurons[GetElementPos(OUT)].SplitX) / 2;

        int InnovationNum = table.GetInnovationNumber(IN, OUT, InnovationTable.InnovationType.NewNeuron);

        if(InnovationNum >= 0)
        {
            int nID = table.GetNeuronID(InnovationNum);

            if (AlreadyHaveThisNeuron(nID))
            {
                InnovationNum = -1;
            }
        }

        if(InnovationNum < 0)
        {
            int newNID = table.CreateNewInnovation(IN, OUT, InnovationTable.InnovationType.NewNeuron);
            Neurons.Add(new Neuron(InnovationTable.NeuronType.hidden, newWidth, newDepth, newNID));

            int Link1InnovationNum = table.CreateNewInnovation(IN, newNID, InnovationTable.InnovationType.NewLink);
            Synapses.Add(new Synapse(IN, newNID, 1, Link1InnovationNum));

            int Link2InnovationNum = table.CreateNewInnovation(newNID, OUT, InnovationTable.InnovationType.NewLink);
            Synapses.Add(new Synapse(newNID, OUT, oldWeight, Link2InnovationNum));
        }
        else
        {
            int newNID = table.GetNeuronID(InnovationNum);
            Neurons.Add(new Neuron(InnovationTable.NeuronType.hidden, newWidth, newDepth, newNID));

            int link1 = table.GetInnovationNumber(IN, newNID, InnovationTable.InnovationType.NewLink);
            Synapses.Add(new Synapse(IN, newNID, 1, link1));

            int link2 = table.GetInnovationNumber(newNID, OUT, InnovationTable.InnovationType.NewLink);
            Synapses.Add(new Synapse(newNID, OUT, oldWeight, link2));
        }
    }

    void MutateWights(float mutationRate)
    {
        for(int i = 0; i < Synapses.Count; i++)
        {
            if(random.NextDouble() < mutationRate)
            {
                Synapses[i].Weight = (float)random.NextDouble();
            }
        }
    }

    void MutateActivationResponse(float mutationRate)
    {
        for (int i = 0; i < Neurons.Count; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                Neurons[i].ActivationResponse = (float)random.NextDouble();
            }
        }
    }

    public float Compute()
    {
        return 0f;
    }

    public void Mutate(float mutationRate,int numTrys, InnovationTable table)
    {
        AddLink(mutationRate, numTrys, table);
        AddNeuron(mutationRate, numTrys, table);
        MutateWights(mutationRate);
        MutateActivationResponse(mutationRate);
    }

    bool DuplicateLink(int n1, int n2)
    {
        for (int i = 0; i < Synapses.Count; i++)
        {
            if(Synapses[i].IN == n1 && Synapses[i].OUT == n2)
            {
                return true;
            }
            else if(Synapses[i].IN == n2 && Synapses[i].OUT == n1)
            {
                return true;
            }
        }
        return false;
    }

    bool AlreadyHaveThisNeuron(int nID)
    {
        for(int i = 0; i < Neurons.Count; i++)
        {
            if(Neurons[i].ID == nID)
            {
                return true;
            }
        }
        return false;
    }

    int GetElementPos(int ID)
    {
        for(int i = 0; i < Neurons.Count; i++)
        {
            if(Neurons[i].ID == ID)
            {
                return i;
            }
        }
        return -1;
    }

}
