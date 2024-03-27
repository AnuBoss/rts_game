using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviour : MonoBehaviour
{
    [SerializeField] protected float weightMultiplier = 1;
    public float WeightMultiplier { get { return weightMultiplier; } }

    [SerializeField] protected float timePassed = 0;
    public float TimePassed { get { return timePassed; } set { timePassed = value; } }

    public abstract float GetWeight();
    public abstract void Execute();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
