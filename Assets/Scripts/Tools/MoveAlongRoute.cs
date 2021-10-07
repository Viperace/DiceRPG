using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using CubeMapGenerator;
using DG.Tweening;
using Node = CubeMapGenerator.Node;

public class MoveAlongRoute : MonoBehaviour
{
    Sequence seq;

    void Start()
    {
        seq = DOTween.Sequence();
    }

    public bool MoveToNode(Node current, Node target)
    {
        // Find all RouteMono
        RouteMonobehavior[] routesMono = FindObjectsOfType<RouteMonobehavior>();

        if (current.connectedNodes.Contains(target))
        {
            // Find which routeMono contain this two nodes
            foreach (var routeMono in routesMono)
                if (routeMono.Contains(current, target))
                {
                    MoveAlongSpline(routeMono.Spline);
                    return true;
                }

            //
            Debug.Log("Spline not found ?!");
            return false;
        }
        else
        {
            Debug.Log("Target node not connected");
            return false;
        }
    }

    void MoveAlongSpline(SplineComputer spline)
    {

    }
}
