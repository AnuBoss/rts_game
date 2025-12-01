using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{

    [SerializeField] private bool toBuild = false; //this builder has duty to build things
    [SerializeField] private bool showGhost = false; //ghost building is showing

    [SerializeField] private GameObject[] buildingList; // Buildings that this unit can build
    public GameObject[] BuildingList { get { return buildingList; } }
    [SerializeField] private GameObject[] ghostBuildingList; // Transparent buildings according to building list

    [SerializeField] private GameObject newBuilding; // Current building to build
    public GameObject NewBuilding { get { return newBuilding; } set { newBuilding = value; } }

    [SerializeField] private GameObject ghostBuilding; // Tranparent building to check site to build
    public GameObject GhostBuilding { get { return ghostBuilding; } set { ghostBuilding = value; } }

    [SerializeField] private GameObject inProgressBuilding;

    public GameObject InProgressBuilding { get { return inProgressBuilding; } set { inProgressBuilding = value; } }

    private Unit unit;
    private bool building = false;
   

    // Start is called before the first frame update
    void Start()
    {
        unit=GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unit.State == UnitState.Die)
            return;

        if (toBuild) // if this unit is to build something
        {
            GhostBuildingFollowsMouse();

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                CheckClickOnGround();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                CancelToBuild();



        }
        switch (unit.State)
        {
            case UnitState.MoveToBuild:
                MoveToBuild(inProgressBuilding);
                break;
            case UnitState.BuildProgress:
            case UnitState.Building:
                BuildProgress();
                break;
        }


    }

    public void ToCreateNewBuilding(int i) //Start call from ActionManager UI Btns
    {
        if (buildingList[i] == null)
            return;

        Building b = buildingList[i].GetComponent<Building>();

        if (!unit.Factions.CheckBuildingCost(b)) //don't have enough resource to build
            return;
        else
        {
            //Create ghost building at the mouse position
            ghostBuilding = Instantiate(ghostBuildingList[i],
                                        Input.mousePosition,
                                        Quaternion.identity, unit.Factions  .GhostBuildingParent);

            toBuild = true;
            newBuilding = buildingList[i]; //Set prefab into new building
            showGhost = true;
        }
    }

    private void GhostBuildingFollowsMouse()
    {
        if (showGhost)
        {
            Ray ray = CameraController.instance.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
            {
                if (ghostBuilding != null)
                {
                    ghostBuilding.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
                }
            }
        }
    }

    private void CancelToBuild()
    {
        toBuild = false;
        showGhost = false;

        newBuilding = null;
        Destroy(ghostBuilding);
        ghostBuilding = null;
        //Debug.Log("Cancel Building");
    }


    public void BuilderStartFixBuilding(GameObject target)
    {
        inProgressBuilding = target;
        unit.SetState(UnitState.MoveToBuild);
    }

    private void StartConstruction(GameObject buildingObj)
    {
        BuilderStartFixBuilding(buildingObj);
    }

    public void CreateBuildingSite(Vector3 pos) //Set a building site
    {
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
            ghostBuilding = null;
        }

        //We use prefab position.y when instantiating.
        GameObject buildingObj = Instantiate(newBuilding,
                                            new Vector3(pos.x, newBuilding.transform.position.y, pos.z),
                                            Quaternion.identity);

        newBuilding = null; //Clear 

        Building building = buildingObj.GetComponent<Building>();

        //Set building to be underground
        buildingObj.transform.position = new Vector3(buildingObj.transform.position.x,
                                    buildingObj.transform.position.y - building.IntoTheGround,
                                    buildingObj.transform.position.z);
        buildingObj.transform.parent = unit.Factions.BuildingsParent.transform;

        inProgressBuilding = buildingObj; //set a new clone building object to be a building in Unit's mind
        unit.Factions.AliveBuildings.Add(building);

        building.Factions = unit.Factions; //set a building's faction to be belong to this player
        building.IsFunctional = false;
        building.CurHP = 1;

        unit.Factions.DeductBuildingCost(building);

        toBuild = false; //Disable flag at the builder
        showGhost = false; //Disable to show ghost building

        if (unit.Factions == GameManager.instance.MyFaction)
        {
            MainUI.instance.UpdateAllResource(unit.Factions);
        }
        //Debug.Log("Building site created.");

        //order builders to build together
        StartConstruction(inProgressBuilding);
    }

    private void CheckClickOnGround()
    {
        Ray ray = CameraController.instance.Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
        {
            bool canBuild = ghostBuilding.GetComponent<FindBuildingSite>().CanBuild;
            //Debug.Log(hit.collider.tag);
            if ((hit.collider.tag == "Ground") && canBuild)
            {
                //Debug.Log("Click Ground to Build");
                CreateBuildingSite(hit.point); //Create building site with 1 HP
            }
        }
    }

    private void MoveToBuild(GameObject b)
    {
        if (b == null)
            return;

        unit.NavAgent.SetDestination(b.transform.position);
        unit.NavAgent.isStopped = false;
    }

    private void BuildProgress()
    {
        if (inProgressBuilding == null)
            return;

        unit.LookAt(inProgressBuilding.transform.position);
        Building b = inProgressBuilding.GetComponent<Building>();

        //building is already finished
        if ((b.CurHP >= b.MaxHP) && b.IsFunctional)
        {
           
            inProgressBuilding = null; //Clear this job off his mind
            unit.SetState(UnitState.Idle);
            return;
        }
        //constructing
        b.Timer += Time.deltaTime;

        if (b.Timer >= b.WaitTime)
        {
            building = true;
            b.Timer = 0;
            b.CurHP++;
            unit.SetState(UnitState.Building);


            if (b.IsFunctional == false) //if this building is being built, not being fixed
                //Raise up building from the ground
                inProgressBuilding.transform.position += new Vector3(0f, b.IntoTheGround / (b.MaxHP - 1), 0f);

            if (b.CurHP >= b.MaxHP) //finish

            {
                b.CurHP = b.MaxHP;
                b.IsFunctional = true;

                inProgressBuilding = null; //Clear this job off his mind
                unit.SetState(UnitState.Idle);

                unit.Factions.UpdateHousingLimit();
            }

            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (unit.State == UnitState.Die)
            return;

        if (unit != null)
        {
            if (other.gameObject == inProgressBuilding)
            {
                unit.NavAgent.isStopped = true;
                unit.SetState(UnitState.BuildProgress);
            }
        }
    }

    private void OnDestroy()
    {
        if (ghostBuilding != null)
            Destroy(ghostBuilding);
    }


}
