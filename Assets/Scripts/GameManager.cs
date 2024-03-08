using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Factions myFaction;
    public Factions MyFaction { get { return myFaction; } }

    [SerializeField] private Factions enemyFaction;
    public Factions EnemyFaction { get { return enemyFaction; } }

    //All factions in this game (2 factions for now)
    [SerializeField] private Factions[] factions;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MainUI.instance.UpdateAllResource(myFaction);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
