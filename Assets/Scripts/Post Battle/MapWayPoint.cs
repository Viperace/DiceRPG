using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubeMapGenerator;

public class MapWayPoint : MonoBehaviour
{
    [SerializeField] GameObject baseUIprefab;
    [SerializeField] bool isPutBaseUI = true;
    public bool allowLoadScene { get; set; }
    bool IsReachableByPlayer = true;
    MeshRenderer baseMeshRenderer;
    
    static GameObject baseUIholder;

    DepthNode node = null;

    void Awake()
    {
        // Attach Parent
        baseUIholder = GameObject.Find("Base UI Holder");
        if (!baseUIholder)
            baseUIholder = new GameObject("Base UI Holder");
    }

    void Start()
    {
        if (isPutBaseUI)
            PutBaseUI();

        allowLoadScene = true;
    }

    void Update()
    {
        if (baseMeshRenderer)
        {
            if (IsReachableByPlayer)
                baseMeshRenderer.enabled = true;
            else
                baseMeshRenderer.enabled = false;
        }
    }

    void PutBaseUI()
    {
        GameObject baseUI = Instantiate(baseUIprefab);
        baseMeshRenderer = baseUI.GetComponent<MeshRenderer>();

        baseUI.transform.position = this.transform.position; // Dont parent it so its animation is separated
        baseUI.transform.SetParent(baseUIholder.transform);
    }

    public void UpdatePlayerLocation(DepthNode playerNode)
    {
        // Check if it is reachable by playear, which has these criteria
        // 1) its connected to this node
        // 2) its depth larger than current player node
        if (node.IsConnectedTo(playerNode) && playerNode.depth < this.node.depth)
            IsReachableByPlayer = true;
        else
            IsReachableByPlayer = false;
    }

    public void SetupNode(Node node)
    {
        this.node = (DepthNode) node;
    }

    public Node GetNode { get { return node; } }

    public static GameObject BaseUIholder { get { return baseUIholder; } }

    [SerializeField] JourneyEnum representedJourney;
    public void ReachDestinationAction()
    {
        Debug.Log("Hit Reach ReachDestinationAction ");
        if (!allowLoadScene) return;

        UpdateJourney();

        switch (representedJourney)
        {
            case JourneyEnum.CombatEncounter:
                MySceneManager.Instance.LoadBattleScene();
                break;
            case JourneyEnum.BossEncounter:
                break;
            case JourneyEnum.Tavern:
                MySceneManager.Instance.LoadTavernScene();
                break;
            case JourneyEnum.Shop:
                MySceneManager.Instance.LoadShopScene();
                break;
            case JourneyEnum.Shrine:
                MySceneManager.Instance.LoadShrineScene();
                break;
            case JourneyEnum.Blacksmith:
                MySceneManager.Instance.LoadBlacksmithScene();
                break;
            case JourneyEnum.ChoiceReward:
                MySceneManager.Instance.LoadRewardScene();
                break;
            default:
                break;
        }
    }

    void UpdateJourney()
    {
        JourneyUIManager journeyUImanager = FindObjectOfType<JourneyUIManager>();

        switch (representedJourney)
        {
            case JourneyEnum.CombatEncounter:
                journeyUImanager.AddFightJourneyRecord();
                break;
            case JourneyEnum.BossEncounter:
                journeyUImanager.AddFightJourneyRecord();
                break;
            case JourneyEnum.Tavern:
                journeyUImanager.AddTavernJourneyRecord();
                break;
            case JourneyEnum.Shop:
                journeyUImanager.AddShopJourneyRecord();
                break;
            case JourneyEnum.Shrine:
                journeyUImanager.AddShrineJourneyRecord();
                break;
            case JourneyEnum.Blacksmith:
                journeyUImanager.AddBlacksmithJourneyRecord();
                break;
            case JourneyEnum.ChoiceReward:
                journeyUImanager.AddRewardJourneyRecord();
                break;
            default:
                break;
        }
    }
}
