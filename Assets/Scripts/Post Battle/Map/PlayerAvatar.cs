using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CubeMapGenerator;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerAvatar : MonoBehaviour
{
    JourneySceneLoader journeySceneLoader;
    Node currentNode;
    Sequence seq;
    MapWayPoint[] wayPoints;

    void Start()
    {
        journeySceneLoader = FindObjectOfType<JourneySceneLoader>();

        StartCoroutine(InitializePosition());
    }

    IEnumerator InitializePosition()
    {
        // Wait till map ready
        yield return new WaitForSeconds(0.5f);

        currentNode = journeySceneLoader.GetMapStartingNode();
        this.transform.position = currentNode.gameObject.transform.position;

        wayPoints = FindObjectsOfType<MapWayPoint>();
        UpdateNode(currentNode);
    }

    public float jumpPower = 2;
    public float moveSpeed = 5f;
    public float jumpDistance = 5f;
    //TODO: Write to separate component class
    public bool MoveToNode(Node target, System.Action OnArriveFunc)
    {
        // Find all RouteMono
        RouteMonobehavior[] routesMono = FindObjectsOfType<RouteMonobehavior>();

        if(currentNode.IsConnectedTo(target) & target != currentNode)
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
                if (OnArriveFunc != null)
                    OnArriveFunc();
            });

            return true;
        }
        else
        {
            Debug.Log("Target node not connected");
            return false;
        }
    }

    [Button("MovePlayer", ButtonSizes.Large)]
    public void _TestMove()
    {
        MapWayPoint[] waypoints = FindObjectsOfType<MapWayPoint>();
        PlayerAvatar avatar = FindObjectOfType<PlayerAvatar>();
        //MapWayPoint wp = waypoints[Random.Range(0, waypoints.Length)];
        foreach (var wp in waypoints)
        {
            if (avatar.currentNode.IsConnectedTo(wp.GetNode))
            {
                MoveToNode(wp.GetNode, null);
                return;
            }
        }

        Debug.Log("Cant find connected node to player avatar");
    }

    void UpdateNode(Node newNode)
    {
        this.currentNode = newNode;

        // Tell all waypoint to update
        foreach (var wp in wayPoints)
            wp.UpdatePlayerLocation(this.currentNode);
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
