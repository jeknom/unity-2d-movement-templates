using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [Serializable]
    public class Node
    {
        public int movementCost;
        public int estimatedMovementCost;
        public int cost;
        public Vector3 position;
        public Node parent;
    }
    [SerializeField] LayerMask blockingLayer;
    [SerializeField] float movementSpeed = 0.1f;
    [SerializeField] Vector3 testTarget;
    Stack<Vector3> currentRoute = new Stack<Vector3>();
    Vector3 currentStep = Vector3.zero;

    protected void Start()
    {
        this.currentStep = this.transform.position;
        this.MoveTo(this.testTarget);
    }

    void FixedUpdate()
    {
        if (this.currentStep == this.transform.position && this.currentRoute.Count > 0)
        {
            this.currentStep = this.currentRoute.Pop();
        }

        if (this.currentStep != this.transform.position)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.currentStep, this.movementSpeed);
        }
    }

    protected void MoveTo(Vector3 target)
    {
        this.currentRoute = this.GetRoute(target);
    }

    Stack<Vector3> GetRoute(Vector3 target)
    {
        var openSteps = new List<Node>();
        var closedSteps = new List<Node>();

        openSteps.Add(new Node { position = this.transform.position });

        while (openSteps.Any())
        {
            var minCostNode = openSteps.Aggregate((curMin, node) => node.cost < curMin.cost ? node : curMin);
            openSteps.Remove(minCostNode);

            var successors = new List<Node>
            {
                this.GetSuccessor(minCostNode, Vector3.left, target),
                this.GetSuccessor(minCostNode, Vector3.right, target),
                this.GetSuccessor(minCostNode, Vector3.up, target),
                this.GetSuccessor(minCostNode, Vector3.down, target)
            };

            foreach (var successor in successors)
            {
                if (closedSteps.Any(step => step.position == successor.position) || this.IsLayerBlocked(successor.position))
                {
                    continue;
                }
                
                foreach (var step in openSteps.Where(step => step.position == successor.position &&
                    successor.cost < step.cost))
                {
                    step.parent = minCostNode;
                    step.cost = successor.cost;

                    continue;
                }

                if (openSteps.Any(step => step.position == successor.position))
                {
                    continue;
                }

                openSteps.Add(successor);
            }

            closedSteps.Add(minCostNode);

            Debug.Log("I have gone through " + closedSteps.Count + " nodes!");

            if (minCostNode.position == target)
            {
                break;
            }
        }

        var finalPath = new Stack<Vector3>();
        var currentStep = closedSteps.FirstOrDefault(step => step.position == target);

        if (currentStep != default)
        {
            while (!finalPath.Contains(this.transform.position))
            {
                finalPath.Push(currentStep.position);
                currentStep = currentStep.parent;
            }
        }

        return finalPath;
    }

    Node GetSuccessor(Node parent, Vector3 direction, Vector3 target)
    {
        var successorPosition = parent.position + direction;
        var successorCostEstimation = (int)(Mathf.Abs(successorPosition.x - target.x) + Mathf.Abs(successorPosition.y - target.y));
        var successorMovementCost = parent.movementCost + 1;
        var successor = new Node
        {
            movementCost = successorMovementCost,
            estimatedMovementCost = successorCostEstimation,
            cost = successorMovementCost + successorCostEstimation,
            position = successorPosition,
            parent = parent
        };

        return successor;
    }

    bool IsLayerBlocked(Vector3 position)
    {
        var rayCast = Physics2D.Raycast(position, Vector2.zero, this.blockingLayer);

        return rayCast;
    }
}
