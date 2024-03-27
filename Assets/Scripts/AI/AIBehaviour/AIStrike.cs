using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrike : AIBehaviour  
{
    private AISupport support = null;

    public int fightersRequired = 4; //all fighters required to start an attack
    public int increasePerWave = 1; //number to increase fightersRequired in the next wave
    public float squadSize = 0.5f; //Percentage of all fighters assigned to attack as a wave

    public float timeLimit = 5f;
    
    public override float GetWeight()
    {
        if (timePassed < timeLimit)
            return 0f;

        timePassed = 0f; //reset a timer

        if (support.Fighters.Count >= fightersRequired)
            return 1f;
        return 0f;
    }

    public override void Execute()
    {
        //Debug.Log(support.Faction.ToString() + " is attacking");

        int wave = (int)(support.Fighters.Count * squadSize);
        fightersRequired += increasePerWave;

        //Debug.Log("this wave: " + wave);

        for (int i = 0; i < wave; i++)
        {
            //Debug.Log(support.Fighters[i]);

            Unit fighter = support.Fighters[i].GetComponent<Unit>();
            fighter.MoveToPosition(GameManager.instance.MyFaction.StartPosition.position);
        }
    }

    void Start()
    {
        support = gameObject.GetComponent<AISupport>();
    }

}
