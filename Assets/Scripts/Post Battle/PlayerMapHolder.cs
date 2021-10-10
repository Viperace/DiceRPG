using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CubeMapGenerator;

public class PlayerMapHolder : MonoBehaviour
{    
    string activateScene = "Journey Scene";  // the GO will only activate at this scene, and deactivate at all other scene

    Graph graph;
    Node playerNode;

    void Awake()
    {
        // Destroy all other instance
        PlayerMapHolder[] allMaps = FindObjectsOfType<PlayerMapHolder>();
        foreach (var item in allMaps)
            if (item != this)
                Destroy(item);

        DontDestroyOnLoad(gameObject);

        // Check scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

   
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != activateScene)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    public void SetGraph(Graph graph) => this.graph = graph;    
    public void SetPlayerNode(Node node) => this.playerNode = node;

    public Vector3 GetPlayerStartingPosition()
    {
        return playerNode.gameObject.transform.position;
    }
    public Node GetPlayerNode()
    {
        return this.playerNode;
    }

    public Graph GetGraph { get { return this.graph; } }
}
