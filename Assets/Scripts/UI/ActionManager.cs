using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        // [เพิ่ม] เริ่มเกมมาสั่งซ่อน Panel ปุ่มกดไปก่อน
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

        // [เพิ่ม] สั่งซ่อน Panel หลักด้วย CanvasGroup
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

    private void ShowCreateBuildingButtons(Unit u) //Showing list of buildings when selecting a single unit
    {
        if (u.IsBuilder)
        {
            for (int i = 0; i < u.Builder.BuildingList.Length; i++)
            {
                buildingBtns[i].gameObject.SetActive(true);

                if (u.Builder.BuildingList[i] != null)
                {
                    buildingBtns[i].GetComponent<Button>().interactable = true;
                    buildingBtns[i].image.color = Color.white;
                    Building building = u.Builder.BuildingList[i].GetComponent<Building>();
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
       

        HideCreateUnitButtons();     // เคลียร์ปุ่มเก่า
        HideCreateBuildingButtons(); // เคลียร์ปุ่มเก่า
        ShowPanel();                 // [เพิ่ม] สั่งเปิด Panel ขึ้นมา
        ShowCreateUnitButtons(b);
    }

    public void ShowBuilderMode(Unit unit)
    {
        HideCreateUnitButtons();
        HideCreateBuildingButtons();
        ShowPanel();                // [เพิ่ม] สั่งเปิด Panel ขึ้นมา
        ShowCreateBuildingButtons(unit);
    }

    public void CreateUnitButton(int n)//Map with Create Unit Btns
    {
        Debug.Log("Create " + n);
       Command.instance.CurBuilding.ToCreateUnit(n);

    }

    public void CreateBuildingButton(int n)//Map with Create Building Btns
    {
        //Debug.Log("1 - Click Button: " + n);

        Unit unit = Command.instance.CurUnits[0];
        if (unit.IsBuilder)
            unit.Builder.ToCreateNewBuilding(n );

    }

}
