using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Command : MonoBehaviour
{

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Unit curUnit; //current selected single unit
    public Unit CurUnit { get { return curUnit; } }

    private Camera cam;
    private Factions faction;
    public static Command instance;

    [SerializeField]
    private Building curBuilding; //current selected single building
    public Building CurBuilding { get { return curBuilding; } }
    
    [SerializeField]
    private ResourceSource curResource; //current selected resource



    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Unit", "Building", "Resource", "Ground");

        instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ClearEverything();
        }

            
        

        // mouse up
        if (Input.GetMouseButtonUp(0))
        {
            TrySelect(Input.mousePosition);
        }

    }

    void Awake()
    {
        faction = GetComponent<Factions>();

    }

    private void SelectUnit(RaycastHit hit)
    {
        curUnit = hit.collider.GetComponent<Unit>();

        curUnit.ToggleSelectionVisual(true);

        Debug.Log("Selected Unit");

        if(GameManager.instance.MyFaction.IsMyUnit(curUnit))
        {
            ShowUnit(curUnit);
        }

       
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        //if we left-click something
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
        if (curUnit != null)
            curUnit.ToggleSelectionVisual(false);
        if (curBuilding != null)
            curBuilding.ToggleSelectionVisual(false);



    }

    private void ClearEverything()
    {
        ClearAllSelectionVisual();
        curUnit = null;
        curBuilding = null;
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
            //Debug.Log("my building");
            ShowBuilding(curBuilding);//Show building info
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
        InfoManager.instance.ShowAllInfo(curResource);//Show resource info in Info Panel

    }

    private void ResourceSelect(RaycastHit hit)
    {
        curResource = hit.collider.GetComponent<ResourceSource>();
        if (curResource == null)
            return;

        curResource.ToggleSelectionVisual(true);
        ShowResource();//Show resource info
    }




}
