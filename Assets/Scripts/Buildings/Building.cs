using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Structure
{
    // Start is called before the first frame update

    [SerializeField] private Transform spawnPoint;
    public Transform SpawnPoint { get { return spawnPoint; } }
    [SerializeField] private Transform rallyPoint;
    public Transform RallyPoint { get { return rallyPoint; } }

    [SerializeField] private GameObject[] unitPrefabs;

    [SerializeField] private List<Unit> recruitList = new List<Unit>();

    [SerializeField] private float unitTimer = 0f;
    [SerializeField] private int curUnitProgress = 0;

    [SerializeField] private float curUnitWaitTime = 0f;

    [SerializeField] private bool isFunctioal;

    [SerializeField] private bool isHQ;
    public bool IsHQ { get { return isHQ; } }
    public bool IsFunctional { get { return isFunctioal; } set { isFunctioal = value; } }

    [SerializeField] private float intoTheGround = 5f;
    public float IntoTheGround { get { return intoTheGround; } }
    
    public GameObject[] UnitPrefabs { get { return unitPrefabs; } }

    private float timer = 0f; //Constructing timer
    public float Timer { get { return timer; } set { timer = value; } }
    private float waitTime = 0.5f; //How fast it will be construct, higher is longer
    public float WaitTime { get { return waitTime; } set { waitTime = value; } }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        
            ToCreateUnit(0);
            if ((recruitList.Count > 0) && (recruitList[0] != null))
            {
                unitTimer += Time.deltaTime;
                curUnitWaitTime = recruitList[0].UnitWaitTime;
                if (unitTimer >= curUnitWaitTime)
                {
                    curUnitProgress++;
                    unitTimer = 0f;

                    if (curUnitProgress >= 10)
                    {
                        curUnitProgress = 0;
                        curUnitWaitTime = 0f;
                        CreateUnitCompleted();
                    }
                }
            }
        if (Input.GetKeyDown(KeyCode.F))

            ToCreateUnit(1);
        if ((recruitList.Count > 0) && (recruitList[0] != null))
        {
            unitTimer += Time.deltaTime;
            curUnitWaitTime = recruitList[0].UnitWaitTime;
            if (unitTimer >= curUnitWaitTime)
            {
                curUnitProgress++;
                unitTimer = 0f;

                if (curUnitProgress >= 10)
                {
                    curUnitProgress = 0;
                    curUnitWaitTime = 0f;
                    CreateUnitCompleted();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.H))

            ToCreateUnit(2);
        if ((recruitList.Count > 0) && (recruitList[0] != null))
        {
            unitTimer += Time.deltaTime;
            curUnitWaitTime = recruitList[0].UnitWaitTime;
            if (unitTimer >= curUnitWaitTime)
            {
                curUnitProgress++;
                unitTimer = 0f;

                if (curUnitProgress >= 10)
                {
                    curUnitProgress = 0;
                    curUnitWaitTime = 0f;
                    CreateUnitCompleted();
                }
            }
        }



    }

    public void ToCreateUnit(int i)
    {
        Debug.Log(structureName + " creates " + i + ":" + unitPrefabs.Length);
        if (unitPrefabs.Length == 0)
            return;

        if (unitPrefabs[i] == null)
            return;

        Unit unit = unitPrefabs[i].GetComponent<Unit>();

        if (unit == null)
            return;

        if (!faction.CheckUnitCost(unit)) //not enough resources
            return;

        //Deduct Resource
        faction.DeductUnitCost(unit);

        //If it's me, update UI
        if (faction == GameManager.instance.MyFaction)
            MainUI.instance.UpdateAllResource(faction);

        //Add unit into faction's recruit list
        recruitList.Add(unit);

        Debug.Log("Adding" + i + "to Recruit List");
    }

    public void CreateUnitCompleted()
    {
        int id = recruitList[0].ID;

        if (unitPrefabs[id] == null)
            return;

        GameObject unitObj = Instantiate(unitPrefabs[id], spawnPoint.position, Quaternion.Euler(0f, 180f, 0f),faction.UnitsParent);

        recruitList.RemoveAt(0);

        Unit unit = unitObj.GetComponent<Unit>();
        unit.Factions = faction;
        unit.MoveToPosition(rallyPoint.position); //Go to Rally Point

        //Add unit into faction's Army
        faction.AliveUnits.Add(unit);

        Debug.Log("Unit Recruited");
        //If it's me, update UI
        if (faction == GameManager.instance.MyFaction)
            MainUI.instance.UpdateAllResource(faction);
    }

    public void ToggleSelectionVisual(bool flag)
    {
        if (SelectionVisual != null)
            SelectionVisual.SetActive(flag);
    }

}
