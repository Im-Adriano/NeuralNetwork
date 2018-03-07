using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    GameObject[] Cars;
    System.Random random = new System.Random();
    public int GenSize = 50;

    // Use this for initialization
    private void Awake()
    {
        Cars = new GameObject[GenSize];
        for (int i = 0; i < GenSize; i++)
        {
            Cars[i] = GameObject.Instantiate(Resources.Load("Car") as GameObject);
            Cars[i].GetComponent<CarMovement>().InitBrain(random);
        }
    }
    void Start () {
        
        
        for (int i = 0; i < GenSize; i++)
        {
            Cars[i].GetComponent<CarMovement>().StartCar();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
