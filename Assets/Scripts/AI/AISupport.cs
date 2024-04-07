using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISupport : MonoBehaviour
{
    [SerializeField] private List<GameObject> fighters = new List<GameObject>(); //fighter
    public List<GameObject> Fighters { get { return fighters; } }

    [SerializeField] private List<GameObject> builders = new List<GameObject>(); //builder
    public List<GameObject> Builders { get { return builders; } }

    [SerializeField] private List<GameObject> workers = new List<GameObject>(); //worker
    public List<GameObject> Workers { get { return workers; } }

    [SerializeField] private Factions faction;
    public Factions Faction { get { return faction; } }



    [SerializeField] private List<GameObject> hq = new List<GameObject>();
    public List<GameObject> HQ { get { return hq; } }

    [SerializeField] private List<GameObject> houses = new List<GameObject>(); 
    public List<GameObject> Housea { get { return houses; } }

    [SerializeField] private List<GameObject> barracks = new List<GameObject>();
    public List<GameObject> Barracks { get { return barracks; } }





    // Start is called before the first frame update
    void Awake()
    {
        faction = GetComponent<Factions>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Refresh()
    {
        fighters.Clear();
        workers.Clear();
        builders.Clear();

        foreach (Unit u in faction.AliveUnits)
        {
            if (u.gameObject == null)
                continue;

            if (u.IsBuilder) //if it is a builder
                builders.Add(u.gameObject);
            
            if (u.IsWorker) //if it is a worker
                workers.Add(u.gameObject);

            if (!u.IsBuilder && !u.IsWorker) //if it is a fighter
                fighters.Add(u.gameObject);
        }

        hq.Clear();
        houses.Clear();
        barracks.Clear();
        foreach (Building b in faction.AliveBuildings)
        { 
            if(b.gameObject == null)
                continue;

            if(b.IsHQ)
                hq.Add(b.gameObject);

            if(b.IsHousing)
                houses.Add(b.gameObject);

            if(b.IsBarrack)
                barracks.Add(b.gameObject);
        }
    }

}
