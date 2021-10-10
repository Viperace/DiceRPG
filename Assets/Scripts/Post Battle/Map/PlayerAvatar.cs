using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubeMapGenerator;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerAvatar : MonoBehaviour
{
    JourneySceneLoader journeySceneLoader;
    DepthNode currentNode;
    Sequence seq;
    MapWayPoint[] wayPoints;
    PlayerMapHolder mapHolder;

    void Start()
    {
        journeySceneLoader = FindObjectOfType<JourneySceneLoader>();
        mapHolder = FindObjectOfType<PlayerMapHolder>();

        StartCoroutine(InitializePosition());
    }

    bool _hasArrive = false;
    MapWayPoint _targetWP = null;
    void Update()
    {
        if (_hasArrive)
        {
            _targetWP.ReachDestinationAction();
            _hasArrive = false;
        }
    }
    IEnumerator InitializePosition()
    {
        // Wait till map ready
        //yield return new WaitForSeconds(0.5f);

        while (journeySceneLoader.GetGraph() == null)
            yield return null;

        yield return null;

        if (mapHolder)
        {
            this.transform.position = mapHolder.GetPlayerStartingPosition();
            this.currentNode = (DepthNode) mapHolder.GetPlayerNode();
        }
        else
        {
            currentNode = journeySceneLoader.GetMapStartingNode();
            this.transform.position = currentNode.gameObject.transform.position;

            wayPoints = FindObjectsOfType<MapWayPoint>();
            UpdateNode(currentNode);
        }
    }

    public float jumpPower = 2;
    public float moveSpeed = 5f;
    public float jumpDistance = 5f;
    //TODO: Write to separate component class ?
   
    public bool MoveToNode(Node target, MapWayPoint waypoint)
    {
        _targetWP = waypoint;

        // Find all RouteMono
        RouteMonobehavior[] routesMono = FindObjectsOfType<RouteMonobehavior>();

        if (currentNode.IsConnectedTo(target) & target != currentNode &  // Must be 1) connected, 2) not own node, 3) must be more deep
            ( ((DepthNode) target).depth > ((DepthNode)currentNode).depth))
        {
            // Find which routeMono contain this two nodes
            RouteMonobehavior routeMono = null;
            foreach (var route in routesMono)
                if (route.Contains(currentNode, target))
                    routeMono = route;

            // Decide number of steps
            float distance = routeMono.Spline.CalculateLength();
            int numberOfSteps = Mathf.RoundToInt(distance / jumpDistance);
            if (numberOfSteps <= 0)
                numberOfSteps = 1;

            // Grab its spline value on each route
            List<Vector3> positions = AddSplinePosition(routeMono, numberOfSteps);

            // Calc duration per steps
            float durationPerJump = distance / numberOfSteps / moveSpeed;

            // Tween it to move
            seq.Kill();
            seq = DOTween.Sequence();
            foreach (var pos in positions)
                seq.Append(this.transform.DOJump(pos, jumpPower, 1, durationPerJump));
            seq.AppendCallback(() => {
                UpdateNode(target);
                _hasArrive = true;
                Debug.Log("Player has arrive " + _hasArrive);
                //waypoint.ReachDestinationAction();  // <- This for some reason, CANNOT trigger AFTER a new scene. Only works for the original scene in editor.
            });

            return true;
        }
        else
        {
            Debug.Log("Target node not connected");
            return false;
        }

    }


    void UpdateNode(Node newNode)
    {
        this.currentNode = (DepthNode) newNode;

        // Tell all waypoint to update
        foreach (var wp in wayPoints)
            wp.UpdatePlayerLocation(this.currentNode);

        // Tell map holder
        if (!mapHolder)
            mapHolder = FindObjectOfType<PlayerMapHolder>();
        mapHolder.SetPlayerNode(newNode);

    }

    List<Vector3> AddSplinePosition(RouteMonobehavior routeMono, int numberOfSteps)
    {
        // Find whether the spline head. If not need to reverse the position
        Vector3 head = routeMono.Spline.EvaluatePosition(0);
        Vector3 tail = routeMono.Spline.EvaluatePosition(1);
        float distToHead = Vector3.Distance(head, currentNode.gameObject.transform.position);
        float distToTail = Vector3.Distance(tail, currentNode.gameObject.transform.position);

        bool isReverse = distToHead > distToTail ? true : false;

        //
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numberOfSteps + 1; i++)
        {
            if(isReverse)
                positions.Add(routeMono.Spline.EvaluatePosition(1f - i / ((float)numberOfSteps)));
            else
                positions.Add(routeMono.Spline.EvaluatePosition(i / ((float)numberOfSteps)));
        }

        return positions;
    }
}
