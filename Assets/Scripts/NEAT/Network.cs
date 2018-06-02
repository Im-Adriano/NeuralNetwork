using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {
    int ID;
    Dictionary<int,Neuron> Neurons;
    List<Synapse> Synapses;
    float Fitness;
    float AdjustedFitness;
    float NumOffsring;
    int NumInputs, NumOutputs;
    int Speices;
    static System.Random random = new System.Random();

    public Network(int id, Dictionary<int,Neuron> n, List<Synapse> s, int i, int o)
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
            int NewInnovationNum = table.CreateNewInnovation(Neuron1, Neuron2, InnovationTable.InnovationType.NewLink, InnovationTable.NeuronType.none);
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
                S1.Enabled = false;
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
            int newNID = table.CreateNewInnovation(IN, OUT, InnovationTable.InnovationType.NewNeuron, InnovationTable.NeuronType.hidden);
            Neurons.Add(newNID, new Neuron(InnovationTable.NeuronType.hidden, newWidth, newDepth, newNID));

            int Link1InnovationNum = table.CreateNewInnovation(IN, newNID, InnovationTable.InnovationType.NewLink, InnovationTable.NeuronType.none);
            Synapses.Add(new Synapse(IN, newNID, 1, Link1InnovationNum));

            int Link2InnovationNum = table.CreateNewInnovation(newNID, OUT, InnovationTable.InnovationType.NewLink, InnovationTable.NeuronType.none);
            Synapses.Add(new Synapse(newNID, OUT, oldWeight, Link2InnovationNum));
        }
        else
        {
            int newNID = table.GetNeuronID(InnovationNum);
            Neurons.Add(newNID, new Neuron(InnovationTable.NeuronType.hidden, newWidth, newDepth, newNID));

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
        Queue<Neuron> queue = new Queue<Neuron>();
        List<Neuron> EvalOrder = new List<Neuron>();
        float output = 0;
        for(int i = NumInputs; i < NumInputs + NumOutputs; i++)
        {
            queue.Enqueue(Neurons[i]);
        }
        
        while(queue.Count > 0)
        {
            Neuron current = queue.Dequeue();

            if (EvalOrder.Contains(current))
            {
                EvalOrder.Remove(current);
            }

            EvalOrder.Insert(0, current);

            for (int k = 0; k < Synapses.Count; k++)
            {
                if (Synapses[k].OUT == current.ID)
                {
                    queue.Enqueue(Neurons[Synapses[k].IN]);
                }
            }
        }


        for(int i = 0; i < EvalOrder.Count; i++)
        {
            Neuron current = EvalOrder[0];
            EvalOrder.RemoveAt(0);

            if (current.Type != InnovationTable.NeuronType.input)
            {
                //sigmoid here
                current.value = current.value / Mathf.Sqrt(1 + Mathf.Pow(current.value, 2));
            }
           
            for (int k = 0; k < Synapses.Count; k++)
            {
                if (Synapses[k].IN == current.ID)
                {
                    Neurons[Synapses[k].OUT].value = current.value * Synapses[k].Weight;
                }
            }
            if(current.Type == InnovationTable.NeuronType.output)
            {
                output = current.value;
            }
        }
        
        return output;
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
    
    int NumGenes()
    {
        return Synapses.Count + Neurons.Count - 2;
    }

    public void CreateInitialNetwork(InnovationTable table)
    {
        for (int i = 0; i < NumInputs; i++)
        {
            int nID = table.CreateNewInnovation(-1, -1, InnovationTable.InnovationType.NewNeuron, InnovationTable.NeuronType.input);
            Neurons.Add(nID, new Neuron(InnovationTable.NeuronType.input, 1 / NumInputs, 0, nID));
        }
        for (int i = 0; i < NumOutputs; i++)
        {
            int nID = table.CreateNewInnovation(-1, -1, InnovationTable.InnovationType.NewNeuron, InnovationTable.NeuronType.output);
            Neurons.Add(nID, new Neuron(InnovationTable.NeuronType.output, 1 / NumOutputs, 0, nID));
        }
    }

    public static Network Crossover(Network A, Network B)
    {
        InnovationTable.ParentType best;
        if (A.Fitness == B.Fitness)
        {
            if(A.NumGenes() == B.NumGenes())
            {
                best = (InnovationTable.ParentType) random.Next(0 , 1);
            }
            else if(A.NumGenes() < B.NumGenes())
            {
                best = InnovationTable.ParentType.A;
            }
            else
            {
                best = InnovationTable.ParentType.B;
            }
        }
        else if(A.Fitness > B.Fitness)
        {
            best = InnovationTable.ParentType.A;
        }
        else
        {
            best = InnovationTable.ParentType.B;
        }

        Dictionary<int,Neuron> babyNeurons = new Dictionary<int, Neuron>();
        List<Synapse> babySynapses = new List<Synapse>();

        Synapse SelectedGene = null;
        List<int> NeuronsToAdd = new List<int>();

        int curA = 0;
        int curB = 0;

        while (!((curA == A.Synapses.Count) && (curB == B.Synapses.Count)))
        {
            if(curA == A.Synapses.Count && curB < B.Synapses.Count)
            {
                if(best == InnovationTable.ParentType.B)
                {
                    SelectedGene = B.Synapses[curB];
                }
                curB++;
            }
            else if (curB == B.Synapses.Count && curA < A.Synapses.Count)
            {
                if(best == InnovationTable.ParentType.A)
                {
                    SelectedGene = A.Synapses[curA];
                }
                curA++;
            }
            else if (A.Synapses[curA].Innovation < B.Synapses[curB].Innovation)
            {
                if (best == InnovationTable.ParentType.A)
                {
                    SelectedGene = A.Synapses[curA];
                }
                curA++;
            }
            else if(B.Synapses[curB].Innovation < A.Synapses[curA].Innovation)
            {
                if(best == InnovationTable.ParentType.B)
                {
                    SelectedGene = B.Synapses[curB];
                }
                curB++;
            }
            else if (B.Synapses[curB].Innovation == A.Synapses[curA].Innovation)
            {
                if(random.NextDouble() < .5f)
                {
                    SelectedGene = A.Synapses[curA];
                }
                else
                {
                    SelectedGene = B.Synapses[curB];
                }
                curA++;
                curB++;
            }


            if(babySynapses.Count == 0)
            {
                babySynapses.Add(SelectedGene);
            }
            else if (babySynapses[babySynapses.Count-1].Innovation != SelectedGene.Innovation)
            {
                babySynapses.Add(SelectedGene);
            }

            if (!NeuronsToAdd.Contains(SelectedGene.IN))
            {
                NeuronsToAdd.Add(SelectedGene.IN);
            }

            if (!NeuronsToAdd.Contains(SelectedGene.OUT))
            {
                NeuronsToAdd.Add(SelectedGene.OUT);
            }
        }

        NeuronsToAdd.Sort();

        for(int i = 0; i < NeuronsToAdd.Count; i++)
        {
            if (best == InnovationTable.ParentType.A)
            {
                for (int k = 0; k < A.Neurons.Count; k++)
                {
                    if (A.Neurons[k].ID == NeuronsToAdd[i])
                    {
                        babyNeurons.Add(NeuronsToAdd[i], new Neuron(A.Neurons[k].Type, A.Neurons[k].SplitX, A.Neurons[k].SplitY, NeuronsToAdd[i]));
                        break;
                    }
                }
            }
            else
            {
                for (int k = 0; k < B.Neurons.Count; k++)
                {
                    if (B.Neurons[k].ID == NeuronsToAdd[i])
                    {
                        babyNeurons.Add(NeuronsToAdd[i], new Neuron(B.Neurons[k].Type, B.Neurons[k].SplitX, B.Neurons[k].SplitY, NeuronsToAdd[i]));
                        break;
                    }
                }
            }
            
        }

        return new Network(0,babyNeurons,babySynapses,A.NumInputs,A.NumOutputs);
    }
}
