using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Command : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private List<Unit> curUnits = new List<Unit>();
    public List<Unit> CurUnits
    {
        get { return curUnits; }
    }

    private Camera cam;
    private Factions faction;
    public static Command instance;

    [SerializeField]
    private Building curBuilding;
    public Building CurBuilding { get { return curBuilding; } }

    [SerializeField]
    private ResourceSource curResource;

    [SerializeField]
    private RectTransform selectionBox;
    private Vector2 oldAnchoredPos;
    private Vector2 startPos;

    [SerializeField]
    private Unit curEnemy;

    private float timer = 0f;
    private float timeLimit = 0.5f;

    // เพิ่มตัวแปรสำหรับตรวจจับการลาก
    private bool isDragging = false;
    private float dragThreshold = 5f; // ระยะขั้นต่ำที่ถือว่าเป็นการลาก (pixels)
    private bool isPointerOverUI = false; // เก็บสถานะว่าเริ่มคลิกที่ UI หรือไม่

    void Start()
    {
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Unit", "Building", "Resource", "Ground");
        selectionBox = MainUI.instance.SelectionBox;
        instance = this;

        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            timer = 0f;
            UpdateUI();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;

            // เช็คว่าเริ่มคลิกที่ UI หรือไม่
            isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

            // ถ้าไม่ได้คลิกที่ UI ถึงจะ Clear
            if (!isPointerOverUI)
            {
                ClearEverything();
            }
        }

        if (Input.GetMouseButton(0))
        {
            // คำนวณระยะที่เมาส์เคลื่อนที่
            float distance = Vector2.Distance(startPos, Input.mousePosition);

            // ถ้าเคลื่อนที่เกิน threshold ถึงจะถือว่าเป็นการลาก
            if (distance > dragThreshold && !isPointerOverUI)
            {
                isDragging = true;
                UpdateSelectionBox(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // ถ้าเริ่มต้นที่ UI ให้ข้ามการประมวลผลทั้งหมด
            if (isPointerOverUI)
                return;

            if (isDragging)
            {
                // กรณีลาก Selection Box
                ReleaseSelectionBox(Input.mousePosition);
            }
            else
            {
                // กรณีคลิกธรรมดา (ไม่ได้ลาก)
                if (!IsPointerOverUIObject())
                {
                    TrySelect(Input.mousePosition);
                }
            }

            isDragging = false;
        }
    }

    void Awake()
    {
        faction = GetComponent<Factions>();
    }

    private void SelectUnit(RaycastHit hit)
    {
        Unit unit = hit.collider.GetComponent<Unit>();

        Debug.Log("Selected Unit");

        if (GameManager.instance.MyFaction.IsMyUnit(unit))
        {
            curUnits.Add(unit);
            unit.ToggleSelectionVisual(true);
            ShowUnit(unit);
        }
        else
        {
            curEnemy = unit;
            unit.ToggleSelectionVisual(true);
            showEnemyUnit(unit);
        }
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Unit":
                    SelectUnit(hit);
                    break;

                case "Building":
                    BuildingSelect(hit);
                    break;

                case "Resource":
                    ResourceSelect(hit);
                    break;
            }
        }
    }

    private void ClearAllSelectionVisual()
    {
        foreach (Unit u in curUnits)
        {
            u.ToggleSelectionVisual(false);
        }
        if (curBuilding != null)
            curBuilding.ToggleSelectionVisual(false);
        if (curResource != null)
        {
            curResource.ToggleSelectionVisual(false);
        }
        if (curEnemy != null)
        {
            curEnemy.ToggleSelectionVisual(false);
        }
    }

    private void ClearEverything()
    {
        ClearAllSelectionVisual();
        curUnits.Clear();
        curBuilding = null;
        curResource = null;
        curEnemy = null;
        InfoManager.instance.ClearAllInfo();
    }

    private void ShowUnit(Unit u)
    {
        InfoManager.instance.ShowAllInfo(u);
        if (u.IsBuilder)
            ActionManager.instance.ShowBuilderMode(u);
    }

    private void ShowBuilding(Building b)
    {
        InfoManager.instance.ShowAllInfo(b);
        ActionManager.instance.ShowCreateUnitMode(b);
    }

    private void BuildingSelect(RaycastHit hit)
    {
        curBuilding = hit.collider.GetComponent<Building>();
        curBuilding.ToggleSelectionVisual(true);

        if (GameManager.instance.MyFaction.IsMyBuilding(curBuilding))
        {
            ShowBuilding(curBuilding);
        }
        else
        {
            ShowEnemyBuilding(curBuilding);
        }
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void ShowResource()
    {
        InfoManager.instance.ShowAllInfo(curResource);
    }

    private void ResourceSelect(RaycastHit hit)
    {
        curResource = hit.collider.GetComponent<ResourceSource>();
        if (curResource == null)
            return;

        curResource.ToggleSelectionVisual(true);
        ShowResource();
    }

    private void UpdateSelectionBox(Vector3 mousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy && curBuilding == null)
            selectionBox.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

        oldAnchoredPos = selectionBox.anchoredPosition;
    }

    private void ReleaseSelectionBox(Vector2 mousePos)
    {
        Vector2 min;
        Vector2 max;

        selectionBox.gameObject.SetActive(false);

        min = oldAnchoredPos - (selectionBox.sizeDelta / 2);
        max = oldAnchoredPos + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in GameManager.instance.MyFaction.AliveUnits)
        {
            Vector2 unitPos = cam.WorldToScreenPoint(unit.transform.position);

            if (unitPos.x > min.x && unitPos.x < max.x && unitPos.y > min.y && unitPos.y < max.y)
            {
                curUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
        selectionBox.sizeDelta = new Vector2(0, 0);
    }

    private void showEnemyUnit(Unit u)
    {
        InfoManager.instance.ShowEnemyAllInfo(u);
    }

    private void ShowEnemyBuilding(Building b)
    {
        InfoManager.instance.ShowEnemyAllInfo(b);
    }

    private void UpdateUI()
    {
        if (curUnits.Count == 1)
            ShowUnit(curUnits[0]);
        else if (curEnemy != null)
            showEnemyUnit(curEnemy);
        else if (curResource != null)
            ShowResource();
        else if (curBuilding != null)
        {
            if (GameManager.instance.MyFaction.IsMyBuilding(curBuilding))
                ShowBuilding(curBuilding);
            else
                ShowEnemyBuilding(curBuilding);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}