using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICreateHouse : AICreateHQ
{
    // Start is called before the first frame update
    void Start()
    {
        support = gameObject.GetComponent<AISupport>();

        buildingPrefab = support.Faction.BuildingPrefabs[1];
        buildingGhostPrefab = support.Faction.GhostBuildingPrefabs[1];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CheckIfAnyUnfinishedHouse()
    {
        foreach (GameObject houseObj in support.Housea)
        {
            Building h = houseObj.GetComponent<Building>();

            if (!h.IsFunctional && (h.CurHP < h.MaxHP)) //This house is not yet finished
                return true;
        }
        return false;
    }

    public override float GetWeight()
    {
        Building b = buildingPrefab.GetComponent<Building>();

        if (!support.Faction.CheckBuildingCost(b)) //Don't have enough resource to build a house
            return 0;

        if (support.Faction.UnitLimit >= 100) //if unit limit >= 100, stop building a house
            return 0;

        if (support.Housea.Count == 0 && support.Faction.AliveUnits.Count >= 6) //There is no house at all and there are 6 units
            return 1;

        if (CheckIfAnyUnfinishedHouse()) //Check if there is any unfinished house
            return 0;

        if (support.Faction.AliveUnits.Count >= support.Faction.UnitLimit) //If units reach or exceed unit limit
            return 2;

        return 0;
    }


}
