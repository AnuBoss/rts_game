using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICreateHQ : AIBehaviour
{
    protected AISupport support;

    public float rangeFromStartPos = 30f;

    protected GameObject buildingPrefab; //HQ's Prefab
    protected GameObject buildingGhostPrefab;

    protected GameObject buildingObj; //Instantiated Object by Prefab
    protected GameObject buildingObjGhost; //Instantiated Ghost Object by Prefab

    // Start is called before the first frame update
    void Start()
    {
        support = gameObject.GetComponent<AISupport>();

        buildingPrefab = support.Faction.BuildingPrefabs[0];
        buildingGhostPrefab = support.Faction.GhostBuildingPrefabs[0];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float GetWeight()
    {
        Building b = buildingPrefab.GetComponent<Building>();

        if (!support.Faction.CheckBuildingCost(b)) //Don't have enough resource to build
            return 0;

        if (support.HQ.Count < 1) //If there are less than 1 HQ
            return 3f;
            return 0;
    }

    protected void ShowHide3DModel(GameObject obj, bool show)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
            r.enabled = show;
    }

    public override void Execute()
    {
        if (buildingObjGhost == null) //if there is no ghost building
        {
            buildingObjGhost = Instantiate(buildingGhostPrefab);
            buildingObjGhost.transform.SetParent(support.Faction.GhostBuildingParent);
            //Debug.Log("Create Ghost Building");
        }

        //Hide Ghost Building's 3D Model
        ShowHide3DModel(buildingObjGhost, false);

        //if there is a ghost building that has been instantiated from the last frame
        if (buildingObjGhost.GetComponent<FindBuildingSite>().CanBuild == true)
        {
            //Debug.Log("Can Built");
            buildingObj = Instantiate(buildingPrefab);

            Building b = buildingObj.GetComponent<Building>();
            buildingObj.transform.position = new Vector3(buildingObjGhost.transform.position.x,
                                                        buildingObj.transform.position.y - b.GetComponent<Building>().IntoTheGround,
                                                        buildingObjGhost.transform.position.z);
            buildingObj.transform.parent = support.Faction.BuildingsParent;

            support.Faction.AliveBuildings.Add(b);

            b.Factions = support.Faction; //set a building's faction
            b.IsFunctional = false;
            b.CurHP = 1;

            support.Faction.DeductBuildingCost(b);

            if (buildingObjGhost != null)
                Destroy(buildingObjGhost);

            foreach (GameObject unit in support.Builders) //try to complete a building
            {
                Builder builder = unit.GetComponent<Builder>();

                if (builder.InProgressBuilding == null)
                    builder.BuilderStartFixBuilding(buildingObj);
            }
            return;
        }
        else //Can not build at this place
        {
            foreach (GameObject unit in support.Builders) //try to find a new building site for the next frame
            {
                //Debug.Log(unit);

                Builder builder = unit.GetComponent<Builder>();
                builder.NewBuilding = buildingPrefab;

                Vector3 pos = unit.transform.position;
                pos += Random.insideUnitSphere * rangeFromStartPos;

                float Corner1PosX = CameraController.instance.Corner1.position.x;
                float Corner2PosX = CameraController.instance.Corner2.position.x;
                float Corner1PosZ = CameraController.instance.Corner1.position.z;
                float Corner2PosZ = CameraController.instance.Corner2.position.z;

                //Clamp pos to be in a map
                pos = new Vector3(Mathf.Clamp(pos.x, Corner1PosX, Corner2PosX),
                                    pos.y,
                                    Mathf.Clamp(pos.z, Corner1PosZ, Corner2PosZ));

                pos.y = buildingGhostPrefab.transform.position.y;

                //Debug.Log("Pos = " + pos);

                buildingObjGhost.transform.position = pos;
            }
        }
    }


}
