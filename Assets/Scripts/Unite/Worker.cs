using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField]
    private ResourceSource curResourceSource;
    public ResourceSource CurResourceSource { get { return curResourceSource; } set { curResourceSource = value; } }

    [SerializeField]
    private float gatherRate = 0.5f;

    [SerializeField]
    private int gatherAmount = 1; // An amount unit can gather every "gatherRate" second(s)

    [SerializeField]
    private int amountCarry; //amount currently carried
    public int AmountCarry { get { return amountCarry; } set { amountCarry = value; } }

    [SerializeField]
    private int maxCarry = 25; //max amount to carry
    public int MaxCarry { get { return maxCarry; } set { maxCarry = value; } }

    [SerializeField]
    private ResourceType carryType;
    public ResourceType CarryType { get { return carryType; } set { carryType = value; } }

    private float lastGatherTime;
    private Unit unit;
    
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (unit.State)
        {
            case UnitState.MoveToResource:
                MoveToResourceUpdate();
                break;
            case UnitState.Gather:
                GatherUpdate();
                break;
        }
    }
    
    public void ToGatherResource(ResourceSource resource, Vector3 pos)
    {
        curResourceSource = resource;

        //if gather a new type of resource, reset amount to 0
        if (curResourceSource.RsrcType != carryType)
        {
            carryType = CurResourceSource.RsrcType;
            amountCarry = 0;
        }

        unit.SetState(UnitState.MoveToResource);

        unit.NavAgent.isStopped = false;
        unit.NavAgent.SetDestination(pos);
    }
    
    private void MoveToResourceUpdate()
    {
        if (Vector3.Distance(transform.position, unit.NavAgent.destination) <= 2f)
        {
            if (curResourceSource != null)
            {
                unit.LookAt(curResourceSource.transform.position);
                unit.NavAgent.isStopped = true;
                unit.SetState(UnitState.Gather);
            }
        }
    }
    
    private void GatherUpdate()
    {
        if (Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;

            if (amountCarry < maxCarry)
            {
                if (curResourceSource != null)
                {
                    curResourceSource.GatherResource(gatherAmount);

                    carryType = curResourceSource.RsrcType;
                    amountCarry += gatherAmount;
                }
            }
            else //amount is full, go back to deliver at HQ
                unit.SetState(UnitState.DeliverToHQ);
        }
    }


}
