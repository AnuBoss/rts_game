using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICreateBarrack : AICreateHQ
{
    // Start is called before the first frame update
    void Start()
    {
        support = gameObject.GetComponent<AISupport>();

        buildingPrefab = support.Faction.BuildingPrefabs[3];
        buildingGhostPrefab = support.Faction.GhostBuildingPrefabs[3];


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CheckIfAnyUnfinishedHouseAndBarrack()
    {
        foreach (GameObject houseObj in support.Housea)
        {
            Building h = houseObj.GetComponent<Building>();

            if (!h.IsFunctional && (h.CurHP < h.MaxHP)) //This house is not yet finished
                return true;
        }

        foreach (GameObject barrackObj in support.Barracks)
        {
            Building b = barrackObj.GetComponent<Building>();

            if (!b.IsFunctional && (b.CurHP < b.MaxHP)) //This barrack is not yet finished
                return true;
        }
        return false;
    }

    public override float GetWeight()
    {
        Building b = buildingPrefab.GetComponent<Building>();

        if (!support.Faction.CheckBuildingCost(b)) //Don't have enough resource to build a barrack
            return 0;

        if (CheckIfAnyUnfinishedHouseAndBarrack()) //Check if there is any unfinished house or barrack
            return 0;

        if (support.Barracks.Count < 2 && support.Housea.Count > 0) // If there are less than 2 barracks and there are some houses
            return 2;

        return 0;
    }

}
