using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapBlip : MonoBehaviour
{
    private GameObject blip;
    public GameObject Blip { get { return blip; } }

    Unit unit;
    Building building;
    Factions factions;

    private void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
        building = GetComponent<Building>();
    }
    // Start is called before the first frame update
    void Start()
    {
        blip = Instantiate(MiniMap.instance.blipPrefab);
        blip.transform.SetParent(MiniMap.instance.blipParent.transform);
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        blip.transform.position = MiniMap.instance.worldPosToMinimapPos(transform.position);
        blip.transform.position = MainUI.instance.ScalePosition(blip.transform.position);
    }

    void OnDestroy()
    {
        Destroy(blip);
    }

    private void SetColor()
    {
        if (unit != null)
            factions = unit.Factions;

        if (building != null)
        {
            factions = building.Factions;
            blip.GetComponent<RectTransform>().sizeDelta = new Vector2(6f, 6f);
        }

        if (factions != null)
            blip.GetComponent<Image>().color = factions.GetNationColor();
        else
            blip.GetComponent<Image>().color = Color.white;

    }
}
