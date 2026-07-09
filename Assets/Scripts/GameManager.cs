using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Factions myFaction;
    public Factions MyFaction { get { return myFaction; } }

    [SerializeField] private Factions enemyFaction;
    public Factions EnemyFaction { get { return enemyFaction; } }

    //All factions in this game (2 factions for now)
    [SerializeField] private Factions[] factions;
    public Factions[] Factions { get { return factions; } }

    [Header("Win/Lose Settings")]
    [SerializeField] private string victorySceneName = "VictoryScene";
    [SerializeField] private string defeatSceneName = "DefeatScene";
    [SerializeField] private float checkInterval = 1f; // เช็คทุกๆ 1 วินาที

    private bool gameEnded = false;
    private float lastCheckTime;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        SetupPlayers(Settings.mySide, Settings.EnemySide);
    }

    void Start()
    {
        MainUI.instance.UpdateAllResource(myFaction);
        CameraController.instance.FocusOnPosition(myFaction.StartPosition.position);
        lastCheckTime = Time.time;
    }

    void Update()
    {
        // เช็คเงื่อนไขชนะ-แพ้ทุกๆ checkInterval วินาที
        if (!gameEnded && Time.time - lastCheckTime >= checkInterval)
        {
            lastCheckTime = Time.time;
            CheckWinLoseCondition();
        }
    }

    public void SetupPlayers(Nation myNation, Nation enemyNation)
    {
        foreach (Factions f in factions)
        {
            if (f.Nation == myNation)
            {
                Debug.Log("My Side is :" + f);
                myFaction = f;

                f.gameObject.AddComponent<Command>();
                f.gameObject.AddComponent<UnitCommand>();
            }
            else if (f.Nation == enemyNation)
            {
                Debug.Log("Enemy Side is :" + f);
                enemyFaction = f;

                f.gameObject.AddComponent<FactionAI>();
                f.gameObject.AddComponent<AIController>();
                f.gameObject.AddComponent<AISupport>();
                f.gameObject.AddComponent<AIDoNothing>();
                f.gameObject.AddComponent<AIStrike>();
                f.gameObject.AddComponent<AICreateHQ>();
                f.gameObject.AddComponent<AICreateHouse>();
                f.gameObject.AddComponent<AICreateBarrack>();
            }
        }
    }

    // เช็คเงื่อนไขชนะ-แพ้
    private void CheckWinLoseCondition()
    {
        int myBuildingsCount = CountFactionBuildings(myFaction);
        int enemyBuildingsCount = CountFactionBuildings(enemyFaction);

        // ถ้าตึกศัตรูถูกทำลายหมด = ชนะ
        if (enemyBuildingsCount == 0)
        {
            Victory();
        }
        // ถ้าตึกเราถูกทำลายหมด = แพ้
        else if (myBuildingsCount == 0)
        {
            Defeat();
        }
    }

    // นับจำนวนตึกที่เหลืออยู่ของแต่ละฝ่าย
    private int CountFactionBuildings(Factions faction)
    {
        if (faction == null) return 0;

        int buildingCount = 0;

        // หา Building ทั้งหมดที่เป็นของ faction นี้
        Building[] allBuildings = FindObjectsOfType<Building>();

        foreach (Building building in allBuildings)
        {
            // เช็คว่า building นี้เป็นของ faction นี้หรือไม่
            if (building.transform.IsChildOf(faction.transform) ||
                building.GetComponent<Unit>()?.Factions == faction)
            {
                buildingCount++;
            }
        }

        return buildingCount;
    }

    // เรียกเมื่อชนะ
    private void Victory()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Victory! All enemy buildings destroyed!");

        // รอ 2 วินาทีก่อนเปลี่ยน scene
        StartCoroutine(LoadSceneAfterDelay(victorySceneName, 2f));
    }

    // เรียกเมื่อแพ้
    private void Defeat()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Defeat! All your buildings destroyed!");

        // รอ 2 วินาทีก่อนเปลี่ยน scene
        StartCoroutine(LoadSceneAfterDelay(defeatSceneName, 2f));
    }

    // โหลด scene หลังจากหน่วงเวลา
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    // Method สำหรับเรียกจาก Building เมื่อถูกทำลาย (optional - ใช้เพื่อเช็คทันทีแทนที่จะรอ checkInterval)
    public void OnBuildingDestroyed()
    {
        if (!gameEnded)
        {
            CheckWinLoseCondition();
        }
    }
}