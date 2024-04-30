using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    public float timeLimit = 0.5f;
    private float time = 0;

    List<MiniMapBlip> myUnitBlips = new List<MiniMapBlip>();
    List<MiniMapBlip> enemyUnitBlips = new List<MiniMapBlip>();
    List<MiniMapBlip> enemyBuildingBlips = new List<MiniMapBlip>();

    public static VisibilityManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        AssignBlips();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime;
        if (time >= timeLimit) 
        {
            time = 0;
            AssignBlips();
        }
    }

    private void CheckVisibility(List<MiniMapBlip> myUnitBlips, List<MiniMapBlip> targetBlips)
    {
        foreach (MiniMapBlip targetBlip in targetBlips)
        {
            bool seen = false;

            foreach (MiniMapBlip myBlip in myUnitBlips)
            {
                float distance = Vector3.Distance(targetBlip.transform.position, myBlip.transform.position);
                float visibleRange = myBlip.gameObject.GetComponent<Unit>().VisualRange;

                if (distance <= visibleRange) //if there is any of my units seeing this
                {
                    seen = true;
                    break;
                }
            }

            if (targetBlip.Blip != null)
                targetBlip.Blip.SetActive(seen); //set Blip to be seen or not

            //set the unit's 3D model to be seen or not
            foreach (Renderer r in targetBlip.GetComponentsInChildren<Renderer>())
                r.enabled = seen;

           
        }
    }

    public void AssignBlips() //Determine Minimap Blips to be seen or not
    {
        myUnitBlips.Clear();
        enemyUnitBlips.Clear();
        enemyBuildingBlips.Clear();

        // adding all blips into both lists
        foreach (Factions f in GameManager.instance.Factions)
        {
            foreach (Unit u in f.AliveUnits) //Check units
            {
                if (u == null)
                    continue;

                MiniMapBlip blip = u.GetComponent<MiniMapBlip>();

                if (f == GameManager.instance.MyFaction) //if it is my unit
                    myUnitBlips.Add(blip);
                else //if it is an enemy unit
                    enemyUnitBlips.Add(blip);
            }

            foreach (Building b in f.AliveBuildings) //Check buildings
            {
                if (b == null)
                    continue;

                MiniMapBlip blip = b.GetComponent<MiniMapBlip>();

                if (f == GameManager.instance.MyFaction) //if it is my building
                    continue;
                else //if it is an enemy building
                    enemyBuildingBlips.Add(blip);
            }
        }
        CheckVisibility(myUnitBlips, enemyUnitBlips);
        CheckVisibility(myUnitBlips, enemyBuildingBlips);

    }
}
