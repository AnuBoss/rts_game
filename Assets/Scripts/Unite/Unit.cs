using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    Move,
    Attack,
    Building,
    MoveToBuild,
    BuildProgress,
    MoveToResource,
    Gather,
    DeliverToHQ,
    StoretoHQ,
    Die
}

[System.Serializable]
public struct UnitCost
{
    public int food;
    public int wood;
    public int gold;
    public int stone;

}


public class Unit : MonoBehaviour
{

    [SerializeField] private int id;
    public int ID { get { return id; } set { id = value; } }

    [SerializeField] private string unitName;
    public string UnitName { get { return unitName; } }

    [SerializeField] private Sprite unitPic;
    public Sprite UnitPic { get { return unitPic; } }

    [SerializeField] private int curHP;
    public int CurHP { get { return curHP; } set { curHP = value; } }

    [SerializeField] private int maxHP = 100;
    public int MaxHP { get { return maxHP; } }

    [SerializeField] private int moveSpeed = 5;
    public int MoveSpeed { get { return moveSpeed; } }

    [SerializeField] private int minWpnDamage;
    public int MinWpnDamage { get { return minWpnDamage; } }

    [SerializeField] private int maxWpnDamage;
    public int MaxWpnDamage { get { return maxWpnDamage; } }

    [SerializeField] private int armour;
    public int Armour { get { return armour; } }

    [SerializeField] private float visualRange;
    public float VisualRange { get { return visualRange; } }

    [SerializeField] private float weaponRange;
    public float WeaponRange { get { return weaponRange; } }

    [SerializeField] private UnitState state;
    public UnitState State { get { return state; } set { state = value; } }

    private NavMeshAgent navAgent;
    public NavMeshAgent NavAgent { get { return navAgent; } }

    [SerializeField] private Factions faction;
    public Factions Factions { get { return faction; } set { faction = value; } }

    [SerializeField] private GameObject selectionVisual;
    public GameObject SelectionVisual { get { return selectionVisual; } }

    [SerializeField] private UnitCost unitCost;
    public UnitCost UnitCost { get { return unitCost; } }
    [SerializeField] private float unitWaitTime = 0.1f;
    public float UnitWaitTime { get { return unitWaitTime; } }

    [SerializeField] private bool isBuilder;
    public bool IsBuilder { get { return isBuilder; } set { isBuilder = value; } }

    [SerializeField] private Builder builder;
    public Builder Builder { get { return builder; } }
    
    [SerializeField] private bool isWorker;
    public bool IsWorker { get { return isWorker; } set { isWorker = value; } }

    [SerializeField] private Worker worker;
    public Worker Worker { get { return worker; } }
    
    [SerializeField]
    private float pathUpdateRate = 1.0f;
    public float PathUpdateRate { get { return pathUpdateRate; } }

    [SerializeField]
    private float lastPathUpdateTime;
    public float LastPathUpdateTime { get { return lastPathUpdateTime; } set { lastPathUpdateTime = value; } }



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case UnitState.Move:
                MoveUpdate();
                break;
        }

    }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (IsBuilder)
            builder = GetComponent<Builder>();
        if (IsWorker)
        {
            worker = GetComponent<Worker>();
        }
    }

    public void ToggleSelectionVisual(bool flag)
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(flag);
    }

    public void SetState(UnitState toState)
    {
        state = toState;

        if (state == UnitState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    

    public void MoveToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
        }

        SetState(UnitState.Move);
    }

   


    private void MoveUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);

        if (distance <= 1f)
            SetState(UnitState.Idle);
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

}
