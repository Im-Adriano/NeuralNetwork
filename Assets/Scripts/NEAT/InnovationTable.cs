using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnovationTable {
    public enum NeuronType { input, hidden, bias, output, none }

    public enum InnovationType { NewLink, NewNeuron}

    public enum ParentType { A, B }

    public struct Innovation
    {
        public InnovationType innovationType;
        public int InnovationID;
        public int NeuronIN;
        public int NeuronOUT;
        public int NeuronID;
        public NeuronType neuronType;

        public Innovation(InnovationType t, int iID, int nIN, int nOUT, int nID, NeuronType nT)
        {
            innovationType = t;
            InnovationID = iID;
            NeuronIN = nIN;
            NeuronOUT = nOUT;
            NeuronID = nID;
            neuronType = nT;
        }
    }

    public List<Innovation> Innovations;
    public int InnovationNumber;
    public int NeuronNumber;

    public InnovationTable()
    {
        Innovations = new List<Innovation>();
        InnovationNumber = 0;
        NeuronNumber = 0;
    }

    public int CreateNewInnovation(int nIN, int nOUT, InnovationType type, NeuronType nType)
    {
        if (type == InnovationType.NewNeuron)
        {
            Innovations.Add(new Innovation(type, InnovationNumber + 1, nIN, nOUT, NeuronNumber, NeuronType.hidden));
            NeuronNumber++;
            InnovationNumber++;
            return NeuronNumber;
        }
        else
        {
            Innovations.Add(new Innovation(type, InnovationNumber + 1, nIN, nOUT, -1, NeuronType.none));
            InnovationNumber++;
            return InnovationNumber;
        }
    }

    public int GetInnovationNumber(int nIN, int nOUT, InnovationType type)
    {
        int iID = -1;
        for (int i = 0; i < Innovations.Count; i++)
        {
            if (Innovations[i].innovationType == type)
            {
                if (Innovations[i].NeuronIN == nIN && Innovations[i].NeuronOUT == nOUT)
                {
                    iID = Innovations[i].InnovationID;
                }
            }
        }
        return iID;
    }

    public int GetNeuronID(int InnovationNum)
    {
        int nID = -1;
        for (int i = 0; i < Innovations.Count; i++)
        {
            if (Innovations[i].innovationType == InnovationType.NewNeuron)
            {
                if (Innovations[i].InnovationID == InnovationNum)
                {
                    nID = Innovations[i].NeuronID;
                }
            }
        }
        return nID;
    }
}
