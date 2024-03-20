using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    protected float checkRate = 1.0f;

    protected LayerMask unitLayerMask;
    protected LayerMask buildingLayerMask;

    protected Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Check", 0.0f, checkRate);
        unitLayerMask = LayerMask.GetMask("Unit");
        buildingLayerMask = LayerMask.GetMask("Building");

        if (unit == null)
            unit = GetComponent<Unit>();

    }

    // checks for nearby enemies with a sphere cast
    protected Unit CheckForNearbyEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, unit.DefendRange, Vector3.up, unitLayerMask);

        GameObject closest = null;
        float closestDist = 0.0f;

        for (int x = 0; x < hits.Length; x++)
        {
            //Debug.Log("Test - " + hits[x].collider.gameObject.ToString());
            Unit target = hits[x].collider.GetComponent<Unit>();

            // skip if this is not a unit or if it is a corpse
            if ((target == null) || (target.CurHP <= 0))
                continue;

            // skip if this is myself
            if (hits[x].collider.gameObject == gameObject)
                continue;

            // skip if it is a natural unit
            if (target.Factions == null)
                continue;

            // is this a team mate?
            else if (unit.Factions.IsMyUnit(target))
                continue;

            // if it is not the closest enemy or the distance is less than the closest distance it currently has
            else if (!closest || (Vector3.Distance(transform.position, hits[x].transform.position) < closestDist))
            {
                closest = hits[x].collider.gameObject;
                closestDist = Vector3.Distance(transform.position, hits[x].transform.position);
            }
            
        }
        if (closest != null)
        {
            //Debug.Log(closest.gameObject.ToString() + ", " + closestDist.ToString());
            return closest.GetComponent<Unit>();
        }
        else
            return null;
    }
    
    protected void Check()
    {
        if (!unit.IsWorker && !unit.IsBuilder)// if a unit is a fighter
        {
            // check if we have nearby enemies - if so, attack them
            if (unit.State != UnitState.AttackUnit && unit.State != UnitState.MoveToEnemy)
            {
                Unit enemy = CheckForNearbyEnemies(); //check if we have nearby enemies 

                if (enemy != null) //if so, attack them
                {
                    unit.ToAttackUnit(enemy);
                }
            }
        }
    }



}
