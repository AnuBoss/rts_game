using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretState
{
    Idle,
    Defending
}
public class Turret : Building
{
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private GameObject targetUnit;

    [SerializeField] private float detectRange = 60f;
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private int shootDamage = 10;
    public int ShootDamage { get { return shootDamage; } }

    [SerializeField] TurretState state = TurretState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // checks for nearby enemies with a sphere cast
    protected Unit CheckForNearbyEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectRange, Vector3.up, unitLayerMask);

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
            else if (faction.IsMyUnit(target))
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

}
