using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommand : MonoBehaviour
{

    public LayerMask layerMask;
    private Command unitSelect;

    private Camera cam;

    void Awake()
    {
        unitSelect = GetComponent<Command>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        layerMask = LayerMask.GetMask("Unit", "Building", "Resource", "Ground");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            TryCommand(Input.mousePosition);
        }

    }

    private void UnitsMoveToPosition(Vector3 dest, List<Unit> units)
    {
        foreach (Unit u in units)
        {
            if (u != null)
            {
                u.MoveToPosition(dest);
            }
        }
    }

    private void CommandToGround(RaycastHit hit, List<Unit> units)
    {
        UnitsMoveToPosition(hit.point, units);
        CreateVFXMarker(hit.point, MainUI.instance.SelectionMaker);

    }

    private void TryCommand(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        //if we left-click something
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    CommandToGround(hit, unitSelect.CurUnits);
                    break;
                case "Resource":
                    ResourceCommand(hit,unitSelect.CurUnits);
                    break;
            }
        }
    }

    private void CreateVFXMarker(Vector3 pos, GameObject vfxPrefab)
    {
        if (vfxPrefab == null)
            return;

        Instantiate(vfxPrefab, new Vector3(pos.x, 0.1f, pos.z), Quaternion.identity);
    }
    
    // called when we command units to gather a resource
    private void UnitsToGatherResource(ResourceSource resource, List<Unit> units)
    {
        foreach (Unit u in units)
        {
            if (u.IsWorker)
                u.Worker.ToGatherResource(resource, resource.transform.position);
            else
                u.MoveToPosition(resource.transform.position);
        }
        
    }


    private void ResourceCommand(RaycastHit hit, List<Unit> units)
    {
        UnitsToGatherResource(hit.collider.GetComponent<ResourceSource>(), units);
        CreateVFXMarker(hit.transform.position, MainUI.instance.SelectionMaker);
    }



   

}
