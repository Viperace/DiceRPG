using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubeMapGenerator;

public class MapWayPoint : MonoBehaviour
{
    [SerializeField] GameObject baseUIprefab;
    [SerializeField] bool isPutBaseUI = true;
    bool IsReachableByPlayer = true;
    MeshRenderer baseMeshRenderer;
    GameObject baseUI;

    Node node = null;
    void Start()
    {
        if (isPutBaseUI)
            PutBaseUI();
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
        //baseUI.transform.SetParent(this.transform);
        //baseUI.transform.localPosition = Vector3.zero;
        baseMeshRenderer = baseUI.GetComponent<MeshRenderer>();

        baseUI.transform.position = this.transform.position; // Dont parent it so its animation is separated
    }

    public void UpdatePlayerLocation(Node playerNode)
    {
        if (node.IsConnectedTo(playerNode))
            IsReachableByPlayer = true;
        else
            IsReachableByPlayer = false;
    }

    public void SetupNode(Node node)
    {
        this.node = node;
    }

    public Node GetNode { get { return node; } }

    [SerializeField] JourneyEnum representedJourney;
    public void ReachDestinationAction()
    {
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
}
