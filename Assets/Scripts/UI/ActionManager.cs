using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // จำเป็นต้องมีเพื่อเข้าถึง ScrollRect

public class ActionManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button[] unitBtns;
    [SerializeField] private Button[] buildingBtns;
    private CanvasGroup cg;

    public static ActionManager instance;

    private void Awake()
    {
        instance = this;
        cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // [เพิ่ม] ป้องกันไม่ให้ ActionPanel ถูกลาก (Drag) ได้
        // โดยการเช็คว่ามี ScrollRect หรือไม่ ถ้ามีให้ปิดการใช้งาน
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
            scrollRect.enabled = false;
        }

        // เริ่มเกมมาสั่งซ่อน Panel ปุ่มกดไปก่อน
        ClearAllInfo();
    }

    private void HideCreateUnitButtons()
    {
        for (int i = 0; i < unitBtns.Length; i++)
            unitBtns[i].gameObject.SetActive(false);
    }

    private void HideCreateBuildingButtons()
    {
        for (int i = 0; i < buildingBtns.Length; i++)
            buildingBtns[i].gameObject.SetActive(false);
    }

    public void ClearAllInfo()
    {
        HideCreateUnitButtons();
        HideCreateBuildingButtons();

        // สั่งซ่อน Panel หลักด้วย CanvasGroup
        if (cg != null)
        {
            cg.alpha = 0; // ปรับให้โปร่งใส
            cg.blocksRaycasts = false; // ปิดการคลิก
            cg.interactable = false;
        }
    }

    private void ShowPanel()
    {
        if (cg != null)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }

    private void ShowCreateUnitButtons(Building b)
    {
        if (b.IsFunctional)
        {
            for (int i = 0; i < b.UnitPrefabs.Length; i++)
            {
                unitBtns[i].gameObject.SetActive(true);
                Unit unit = b.UnitPrefabs[i].GetComponent<Unit>();
                unitBtns[i].image.sprite = unit.UnitPic;
            }
        }
    }

    private void ShowCreateBuildingButtons(Unit u)
    {
        if (u.IsBuilder && u.Factions != null)
        {
            // ใช้ Building Prefabs จาก Faction แทน
            GameObject[] buildingPrefabs = u.Factions.BuildingPrefabs;

            if (buildingPrefabs == null || buildingPrefabs.Length == 0)
                return;

            for (int i = 0; i < buildingPrefabs.Length && i < buildingBtns.Length; i++)
            {
                buildingBtns[i].gameObject.SetActive(true);

                if (buildingPrefabs[i] != null)
                {
                    buildingBtns[i].GetComponent<Button>().interactable = true;
                    buildingBtns[i].image.color = Color.white;
                    Building building = buildingPrefabs[i].GetComponent<Building>();
                    if (building != null)
                        buildingBtns[i].image.sprite = building.StructurePic;
                }
                else
                {
                    buildingBtns[i].GetComponent<Button>().interactable = false;
                    buildingBtns[i].image.color = Color.clear;
                }
            }
        }
    }

    public void ShowCreateUnitMode(Building b)
    {
        HideCreateUnitButtons();
        HideCreateBuildingButtons();
        ShowPanel();
        ShowCreateUnitButtons(b);
    }

    public void ShowBuilderMode(Unit unit)
    {
        HideCreateUnitButtons();
        HideCreateBuildingButtons();
        ShowPanel();
        ShowCreateBuildingButtons(unit);
    }

    public void CreateUnitButton(int n)
    {
        Debug.Log("Create " + n);
        Command.instance.CurBuilding.ToCreateUnit(n);
    }

    public void CreateBuildingButton(int n)
    {
        Unit unit = Command.instance.CurUnits[0];
        if (unit.IsBuilder)
            unit.Builder.ToCreateNewBuilding(n);
    }
}