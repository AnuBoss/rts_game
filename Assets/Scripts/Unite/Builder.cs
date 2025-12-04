using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    [SerializeField] private bool toBuild = false;
    [SerializeField] private bool showGhost = false;

    [SerializeField] private GameObject[] buildingList;
    public GameObject[] BuildingList { get { return buildingList; } }
    [SerializeField] private GameObject[] ghostBuildingList;

    [SerializeField] private GameObject newBuilding;
    public GameObject NewBuilding { get { return newBuilding; } set { newBuilding = value; } }

    [SerializeField] private GameObject ghostBuilding;
    public GameObject GhostBuilding { get { return ghostBuilding; } set { ghostBuilding = value; } }

    [SerializeField] private GameObject inProgressBuilding;
    public GameObject InProgressBuilding { get { return inProgressBuilding; } set { inProgressBuilding = value; } }

    private Unit unit;
    private bool building = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip constructionSound;
    [SerializeField] private AudioClip finishBuildingSound;

    void Start()
    {
        unit = GetComponent<Unit>();
    }

    void Update()
    {
        if (unit.State == UnitState.Die)
            return;

        if (toBuild)
        {
            // 1. อัปเดตตำแหน่ง Ghost Building ตลอดเวลา (ถ้าไม่ได้กด UI)
            GhostBuildingFollowsMouse();

            // 2. เช็คการคลิกซ้ายเพื่อสร้าง
            if (Input.GetMouseButtonDown(0))
            {
                // ถ้าเมาส์ชี้บน UI (ปุ่ม/Panel) ให้ยกเลิกการคลิกวางสิ่งก่อสร้าง
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                CheckClickOnGround();
            }

            // 3. ยกเลิกการสร้าง
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

    public void ToCreateNewBuilding(int i)
    {
        if (buildingList[i] == null) return;

        Building b = buildingList[i].GetComponent<Building>();

        // เช็คทรัพยากร
        if (!unit.Factions.CheckBuildingCost(b))
            return;

        // ถ้ามี Ghost ตัวเก่าค้างอยู่ ให้ทำลายก่อนสร้างตัวใหม่
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
        }

        // สร้าง Ghost Building (เช็คว่า unit.Factions.GhostBuildingParent มีค่าหรือไม่ ถ้าไม่มีให้ใส่ null หรือ transform)
        Transform parentTransform = (unit.Factions != null) ? unit.Factions.GhostBuildingParent : null;

        // **จุดสำคัญ**: ใช้ Raycast หาจุดเริ่มต้น เพื่อไม่ให้ Ghost ไปโผล่ที่ (0,0,0) หรือจุดแปลกๆ
        Vector3 spawnPos = Input.mousePosition;
        Ray ray = CameraController.instance.Cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Ground")))
        {
            spawnPos = hit.point;
        }

        ghostBuilding = Instantiate(ghostBuildingList[i], spawnPos, Quaternion.identity, parentTransform);

        toBuild = true;
        newBuilding = buildingList[i];
        showGhost = true;
    }

    private void GhostBuildingFollowsMouse()
    {
        if (showGhost && ghostBuilding != null)
        {
            Ray ray = CameraController.instance.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ตรวจสอบ Layer "Ground" ให้แน่ใจว่าพื้นในฉากเป็น Layer นี้จริง
            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
            {
                ghostBuilding.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
            }
        }
    }

    // ... (ส่วนอื่นๆ ของโค้ดคงเดิม) ...

    private void CheckClickOnGround()
    {
        if (ghostBuilding == null) return;

        // เช็ค Component FindBuildingSite ว่าพื้นที่นี้สร้างได้ไหม
        var findSite = ghostBuilding.GetComponent<FindBuildingSite>();
        bool canBuild = (findSite != null) ? findSite.CanBuild : true; // ถ้าไม่มี script นี้ให้ถือว่าสร้างได้ไปก่อน (เพื่อทดสอบ)

        Ray ray = CameraController.instance.Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
        {
            // ตรวจสอบ Tag ของพื้นที่คลิก
            if (hit.collider.CompareTag("Ground") && canBuild)
            {
                CreateBuildingSite(hit.point);
            }
        }
    }

    // ... (ส่วนที่เหลือของโค้ดคงเดิม: CancelToBuild, CreateBuildingSite, MoveToBuild, etc.) ...
    private void CancelToBuild()
    {
        toBuild = false;
        showGhost = false;

        newBuilding = null;
        if (ghostBuilding != null) Destroy(ghostBuilding);
        ghostBuilding = null;
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

    public void CreateBuildingSite(Vector3 pos)
    {
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);
            ghostBuilding = null;
        }

        GameObject buildingObj = Instantiate(newBuilding,
                                            new Vector3(pos.x, newBuilding.transform.position.y, pos.z),
                                            Quaternion.identity);

        newBuilding = null;

        Building building = buildingObj.GetComponent<Building>();

        buildingObj.transform.position = new Vector3(buildingObj.transform.position.x,
                                    buildingObj.transform.position.y - building.IntoTheGround,
                                    buildingObj.transform.position.z);

        // เช็ค Factions ก่อนใช้งาน
        if (unit.Factions != null)
        {
            buildingObj.transform.parent = unit.Factions.BuildingsParent.transform;
            unit.Factions.AliveBuildings.Add(building);
            building.Factions = unit.Factions;
            unit.Factions.DeductBuildingCost(building);
            if (unit.Factions == GameManager.instance.MyFaction)
            {
                MainUI.instance.UpdateAllResource(unit.Factions);
            }
        }

        building.IsFunctional = false;
        building.CurHP = 1;

        toBuild = false;
        showGhost = false;

        StartConstruction(inProgressBuilding = buildingObj);
    }

    // ... (ส่วน MoveToBuild, BuildProgress, OnTriggerStay, OnDestroy คงเดิม) ...
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

        if ((b.CurHP >= b.MaxHP) && b.IsFunctional)
        {

            inProgressBuilding = null;
            unit.SetState(UnitState.Idle);
            return;
        }

        b.Timer += Time.deltaTime;

        if (b.Timer >= b.WaitTime)
        {
            building = true;
            b.Timer = 0;
            b.CurHP++;
            unit.SetState(UnitState.Building);

            if (unit.SelectionVisual != null && unit.SelectionVisual.activeSelf)
            {
                if (unit.AudioSourceRef != null && constructionSound != null)
                {
                    unit.AudioSourceRef.pitch = Random.Range(0.8f, 1.2f);
                    unit.AudioSourceRef.PlayOneShot(constructionSound);
                    unit.AudioSourceRef.pitch = 1f;
                }
            }

            if (b.IsFunctional == false)
                inProgressBuilding.transform.position += new Vector3(0f, b.IntoTheGround / (b.MaxHP - 1), 0f);

            if (b.CurHP >= b.MaxHP)

            {
                b.CurHP = b.MaxHP;
                b.IsFunctional = true;

                if (unit.AudioSourceRef != null && finishBuildingSound != null)
                {
                    unit.AudioSourceRef.PlayOneShot(finishBuildingSound);
                }

                inProgressBuilding = null;
                unit.SetState(UnitState.Idle);

                if (unit.Factions != null)
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