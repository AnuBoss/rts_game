using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InfoManager instance;
    [Header("UI Control")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Image unitPic, hpIcon, rsrcIcon, moveIcon, atkIcon, amrIcon, vslRngIcon, wpRngIcon;
    [SerializeField] private TextMeshProUGUI nameTxt, hpTxt, rsrcTxt, moveTxt, atkTxt, amrTxt, vslRngTxt, wpRngTxt;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetPic(Sprite pic)
    {
        unitPic.color = Color.white;
        unitPic.sprite = pic;
    }

    private void OpenPanel()
    {
        if (infoPanel != null) infoPanel.SetActive(true);
    }
    public void ShowAllInfo(Unit unit)
    {
        OpenPanel(); // [เพิ่ม] สั่งเปิด

        SetPic(unit.UnitPic);
        nameTxt.text = unit.UnitName;

        hpIcon.color = Color.white;
        hpTxt.text = $"{unit.CurHP}/{unit.MaxHP}";

        moveIcon.color = Color.white;
        moveTxt.text = unit.MoveSpeed.ToString();

        atkIcon.color = Color.white;
        atkTxt.text = $"{unit.MinWpnDamage}-{unit.MaxWpnDamage}";

        amrIcon.color = Color.white;
        amrTxt.text = $"{unit.Armour}";

        vslRngIcon.color = Color.white;
        vslRngTxt.text = $"{unit.VisualRange}";

        wpRngIcon.color = Color.white;
        wpRngTxt.text = $"{unit.WeaponRange}";
    }

    public void ShowAllInfo(Building building)
    {
        OpenPanel();

        SetPic(building.StructurePic);
        nameTxt.text = building.StructureName;

        hpIcon.color = Color.white;
        hpTxt.text = $"{building.CurHP}/{building.MaxHP}";
    }
    
    public void ShowAllInfo(ResourceSource r)
    {
        OpenPanel();

        SetPic(r.RsrcPic);
        nameTxt.text = r.RsrcName;
        hpIcon.color = Color.white;
        hpTxt.text = $"{r.Quantity}/{r.MaxQuantity}";
    }




    public void ClearAllInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
            return; // ปิดแล้วจบเลย ไม่ต้องไปเคลียร์ค่าข้างล่างก็ได้เพื่อประหยัดแรง
        }

        //Clear Pic
        unitPic.color = Color.clear;
        nameTxt.text = "";

        hpIcon.color = Color.clear;
        hpTxt.text = "";

        moveIcon.color = Color.clear;
        moveTxt.text = "";

        atkIcon.color = Color.clear;
        atkTxt.text = "";

        amrIcon.color = Color.clear;
        amrTxt.text = "";

        vslRngIcon.color = Color.clear;
        vslRngTxt.text = "";

        wpRngIcon.color = Color.clear;
        wpRngTxt.text = "";
    }
    
    public void ShowEnemyAllInfo(Unit unit)
    {
        SetPic(unit.UnitPic);
        nameTxt.text = unit.UnitName;

        hpIcon.color = Color.white;
        hpTxt.text = $"{unit.CurHP}/{unit.MaxHP}";
    }
    
    public void ShowEnemyAllInfo(Building b)
    {
        SetPic(b.StructurePic);
        nameTxt.text = b.StructureName;

        hpIcon.color = Color.white;
        hpTxt.text = $"{b.CurHP}/{b.MaxHP}";
    }



}
