using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumGene : MonoBehaviour {

    public System.Random rand;

	// Use this for initialization
	void Awake () {
        rand = new System.Random();
	}

}
