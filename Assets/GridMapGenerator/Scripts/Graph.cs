using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeMapGenerator
{
    /// <summary>
    /// Node and connectors
    /// </summary>
    public class Graph
    {
        public HashSet<Node> nodes { get; protected set; }
        public HashSet<Connector> connectors { get; protected set; }

        public Graph() { this.nodes = new HashSet<Node>(); }
        public Graph(Node[] nodes)
        {
            this.nodes = new HashSet<Node>(nodes);
        }

        public void Add(Node node) => nodes.Add(node);
        public void Add(params Node[] nodes)
        {
            foreach (var item in nodes)
                this.Add(item);
        }

        // Go through all nodes and find the unique connectrs
        public void MapConnectors()
        {
            connectors = new HashSet<Connector>();
            foreach (var start in nodes)
            {
                foreach (var end in start.connectedNodes)
                {
                    Connector connector = new Connector(start, end);
                    if (!IsConnectionExists(connector))
                        connectors.Add(connector);
                }
            }
        }

        bool IsConnectionExists(Connector connector)
        {
            foreach (var con in connectors)
                if (con.EqualsTo(connector))
                    return true;
            return false;
        }
        
        public int NumberOfNodes { get { return nodes.Count; } }

        
    }

    public class GraphWithDepthNode : Graph
    {
        public GraphWithDepthNode() : base() { }
        public List<DepthNode> GetNodesAtDepth(int d)
        {
            List<DepthNode> output = new List<DepthNode>();
            foreach (var n in nodes)
            {
                DepthNode depthNode = (DepthNode)n;
                if (depthNode.depth == d)
                    output.Add(depthNode);
            }
            return output;
        }

        public int GetMaxDepth()
        {
            int maxDepth = 0;
            foreach (var item in nodes)
            {
                DepthNode depthNode = (DepthNode)item;
                if (depthNode.depth > maxDepth)
                    maxDepth = depthNode.depth;
            }
            return maxDepth;
        }

        // Number of unique depth level
        public int GetNumberOfDepths()
        {
            HashSet<int> depths = new HashSet<int>();
            foreach (var item in nodes)
            {
                DepthNode depthNode = (DepthNode)item;
                depths.Add(depthNode.depth);
            }
            return depths.Count;
        }
    }

    public class Node
    {
        public HashSet<Node> connectedNodes { get; protected set; }
        public GameObject prefab { get; protected set; }
        public GameObject gameObject { get; protected set; }
        
        public Node(GameObject prefab) 
        {
            connectedNodes = new HashSet<Node>();
            this.prefab = prefab;
        }
        public void AssignGameObject(GameObject go) => this.gameObject = go;

        public void ConnectTo(Node node)
        {
            if(node != this & !connectedNodes.Contains(node))
            {
                connectedNodes.Add(node);
            }
        }

        public void ConnectTo(params Node[] nodes)
        {
            foreach (var item in nodes)
                ConnectTo(item);
        }
    }

    public class DepthNode : Node
    {
        public int depth { get; private set; }
        public DepthNode(int depth, GameObject gameObject) : base(gameObject)
        {
            this.depth = depth;
        }
    }

    public class Connector
    {
        public Node node1;
        public Node node2;
        public Connector(Node node1, Node node2) 
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public bool EqualsTo(Connector other)
        {
            if (this.node1 == other.node1 & this.node2 == other.node2)
                return true;
            else if (this.node1 == other.node2 & this.node2 == other.node1)
                return true;
            else
                return false;
        }
    }

   
    public class GraphTestor
    {
        enum LocationType
        {
            Battle,
            Store,
            Boss
        }

        class BattleNode : DepthNode
        {
            public LocationType type = LocationType.Battle;
            public BattleNode(int d, GameObject gameObject) :base(d, gameObject) 
            {}
        }

        class StoreNode : DepthNode
        {
            public LocationType type = LocationType.Store;
            public StoreNode(int d, GameObject gameObject) : base(d, gameObject)
            { }
        }

        class BossNode : DepthNode
        {
            public LocationType type = LocationType.Boss;
            public BossNode(int d, GameObject gameObject) : base(d, gameObject)
            { }
        }
        public GraphWithDepthNode CreateGraph()
        {
            Dictionary<LocationType, GameObject> myDict = new Dictionary<LocationType, GameObject>();
            GameObject battlePrefab = null;
            GameObject bossPrefab = null;
            GameObject storePrefab = null;

            myDict.Add(LocationType.Battle, battlePrefab);
            myDict.Add(LocationType.Boss, bossPrefab);
            myDict.Add(LocationType.Store, storePrefab);

            GraphWithDepthNode graph = new GraphWithDepthNode();
            BattleNode start0 = new BattleNode(0, myDict[LocationType.Battle]);

            // Start
            BattleNode n1 = new BattleNode(1, myDict[LocationType.Battle]);
            StoreNode s1 = new StoreNode(1, myDict[LocationType.Store]);
            start0.ConnectTo(n1, s1);
            graph.Add(start0, n1, s1);

            BattleNode n2 = new BattleNode(2, myDict[LocationType.Battle]);
            n1.ConnectTo(n2);
            s1.ConnectTo(n2);
            graph.Add(n2);

            BattleNode n3 = new BattleNode(3, myDict[LocationType.Battle]);
            StoreNode s3 = new StoreNode(3, myDict[LocationType.Store]);
            n2.ConnectTo(n3, s3);
            graph.Add(n3, s3);

            BattleNode a4 = new BattleNode(4, myDict[LocationType.Battle]);
            BattleNode b4 = new BattleNode(4, myDict[LocationType.Battle]);
            n3.ConnectTo(a4);
            s3.ConnectTo(b4);
            graph.Add(a4, b4);

            BossNode boss = new BossNode(5, myDict[LocationType.Boss]);
            a4.ConnectTo(boss);
            b4.ConnectTo(boss);
            graph.Add(boss);

            return graph;
        }
    }
}