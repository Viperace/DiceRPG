using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using CubeMapGenerator;

public class JourneySceneLoader : MonoBehaviour
{
    public GameObject blacksmithPrefab;
    public GameObject battlePrefab;
    public GameObject innPrefab;
    public GameObject shopPrefab;
    public GameObject shrinePrefab;
    public GameObject rewardPrefab;
    public GameObject START_PREFAB;

    RouteGenerator routeGenerator;
    GridMapGenerator mapGenerator;
    Map map;
    PopulatePointOfInterest poiGenerator;

    GraphTestor graphTestor;
    void Start()
    {
        routeGenerator = FindObjectOfType<RouteGenerator>();
        mapGenerator = FindObjectOfType<GridMapGenerator>();
        poiGenerator = FindObjectOfType<PopulatePointOfInterest>();

        GenerateMap();
    }

    IEnumerator GenerateMapProcedure()
    {
        // Spawn empty map
        mapGenerator.GeneratePlains();        
        map = mapGenerator.map;

        yield return null;

        // Create Graph
        graphTestor = new GraphTestor(blacksmithPrefab,
              battlePrefab,
              innPrefab,
              shopPrefab,
              shrinePrefab,
              rewardPrefab,
              START_PREFAB);
        GraphWithDepthNode graph = graphTestor.CreateGraph();

        yield return null;

        // Spawn POI based on graph (and routes)
        poiGenerator.SpawnPOIbyGraph(graph);

    }

    [Button("Spawn Paths", ButtonSizes.Large)]
    public void GenerateMap()
    {
        routeGenerator = FindObjectOfType<RouteGenerator>();
        mapGenerator = FindObjectOfType<GridMapGenerator>();
        poiGenerator = FindObjectOfType<PopulatePointOfInterest>();


        StartCoroutine(GenerateMapProcedure());
    }
}

/* Implement Graph
 */

public class GraphTestor
{
    protected GameObject startPointPrefab = null;
    protected GameObject battlePrefab = null;
    protected GameObject shopPrefab = null;
    protected GameObject blacksmithPrefab = null;
    protected GameObject rewardPrefab = null;
    protected GameObject innPrefab = null;
    protected GameObject shrinePrefab = null;

    public GraphTestor() { }
    public GraphTestor(
             GameObject blacksmithPrefab,
             GameObject battlePrefab,
             GameObject innPrefab,
             GameObject shopPrefab,
             GameObject shrinePrefab,
             GameObject rewardPrefab,
             GameObject startPointPrefab)
    {
        this.battlePrefab = battlePrefab;
        this.innPrefab = innPrefab;
        this.shopPrefab = shopPrefab;
        this.shrinePrefab = shrinePrefab;
        this.rewardPrefab = rewardPrefab;
        this.blacksmithPrefab = blacksmithPrefab;
        this.startPointPrefab = startPointPrefab;
    }

    class BattleNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.CombatEncounter;
        public BattleNode(int d, GameObject gameObject) : base(d, gameObject) 
        { }

        public BattleNode InstatiateNew(int depth)
        {
            return new BattleNode(depth, this.prefab);
        }
    }

    class ShopNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.Shop;
        public ShopNode(int d, GameObject gameObject) : base(d, gameObject)
        { }
        public ShopNode InstatiateNew(int depth)
        {
            return new ShopNode(depth, this.prefab);
        }
    }

    class BlacksmithNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.Blacksmith;
        public BlacksmithNode(int d, GameObject gameObject) : base(d, gameObject)
        { }

        public BlacksmithNode InstatiateNew(int depth)
        {
            return new BlacksmithNode(depth, this.prefab);
        }
    }

    class RewardNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.ChoiceReward;
        public RewardNode(int d, GameObject gameObject) : base(d, gameObject)
        { }
        public RewardNode InstatiateNew(int depth)
        {
            return new RewardNode(depth, this.prefab);
        }
    }

    class ShrineNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.Shrine;
        public ShrineNode(int d, GameObject gameObject) : base(d, gameObject)
        { }
        public ShrineNode InstatiateNew(int depth)
        {
            return new ShrineNode(depth, this.prefab);
        }
    }
    class InnNode : DepthNode
    {
        public JourneyEnum type = JourneyEnum.Tavern;
        public InnNode(int d, GameObject gameObject) : base(d, gameObject)
        { }

        public InnNode InstatiateNew(int depth)
        {
            return new InnNode(depth, this.prefab);
        }
    }

    
    public GraphWithDepthNode CreateGraph()
    {
        Dictionary<JourneyEnum, GameObject> myDict = new Dictionary<JourneyEnum, GameObject>();
        
        GraphWithDepthNode graph = new GraphWithDepthNode();

        // Initialize
        InnNode START = new InnNode(0, startPointPrefab);
        BattleNode battle = new BattleNode(0, battlePrefab);
        InnNode inn = new InnNode(0, innPrefab);
        ShrineNode shrine = new ShrineNode(0, shrinePrefab);
        BlacksmithNode blacksmith = new BlacksmithNode(0, blacksmithPrefab);
        RewardNode reward = new RewardNode(0, rewardPrefab);
        ShopNode shop = new ShopNode(0, shopPrefab);

        // Write actual graph here
        BattleNode n1 = battle.InstatiateNew(1);
        n1.ConnectTo(START);
        graph.Add(START, n1);

        List<DepthNode> pastNodes = new List<DepthNode>();
        pastNodes.Add(n1);
        for (int i = 2; i < 27; i++)
        {
            List<DepthNode> thisLevelNodes = new List<DepthNode>();
            
            if (i == 3)
            {
                BlacksmithNode bs = blacksmith.InstatiateNew(i);
                ShopNode sh = shop.InstatiateNew(i);
                thisLevelNodes.Add(bs);
                thisLevelNodes.Add(sh);
            }
            else if (i == 6 | i == 14 |i ==21)
            {
                thisLevelNodes.Add(reward.InstatiateNew(i));
            }
            else if (i == 7)
            {
                thisLevelNodes.Add(blacksmith.InstatiateNew(i));
                thisLevelNodes.Add(battle.InstatiateNew(i));
                thisLevelNodes.Add(inn.InstatiateNew(i));
            }
            else if (i == 10)
            {
                thisLevelNodes.Add(shrine.InstatiateNew(i));
            }
            else if (i == 15)
            {
                thisLevelNodes.Add(blacksmith.InstatiateNew(i));
                thisLevelNodes.Add(shop.InstatiateNew(i));
            }
            else if (i == 19)
            {
                thisLevelNodes.Add(blacksmith.InstatiateNew(i));
                thisLevelNodes.Add(shrine.InstatiateNew(i));
                thisLevelNodes.Add(inn.InstatiateNew(i));
            }
            else if (i == 23)
            {
                thisLevelNodes.Add(battle.InstatiateNew(i));
                thisLevelNodes.Add(inn.InstatiateNew(i));
            }
            else if (i == 25)
            {
                thisLevelNodes.Add(battle.InstatiateNew(i));
                thisLevelNodes.Add(shop.InstatiateNew(i));
            }
            else if (i == 26)
            {
                thisLevelNodes.Add(blacksmith.InstatiateNew(i));
                thisLevelNodes.Add(inn.InstatiateNew(i));
            }
            else if (i == 27) // Supposedly boss
            {
                thisLevelNodes.Add(battle.InstatiateNew(i));
            }
            else
            {
                // Create node
                BattleNode bb = battle.InstatiateNew(i);
                thisLevelNodes.Add(bb);
            }


            // Link to past
            foreach (var past in pastNodes)
                foreach (var curr in thisLevelNodes)
                    past.ConnectTo(curr);

            // Add
            graph.Add(thisLevelNodes.ToArray());

            // Update
            pastNodes = new List<DepthNode>();
            foreach (var curr in thisLevelNodes)
                pastNodes.Add(curr);
        }

        return graph;
    }
}