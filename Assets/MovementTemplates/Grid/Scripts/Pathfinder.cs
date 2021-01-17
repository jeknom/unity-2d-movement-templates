using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinder
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pathfinder : MonoBehaviour
    {
        public Transform target;

        [SerializeField] LayerMask blockingLayer;
        [SerializeField] float movementSpeed = 0.1f;
        [SerializeField] int maxNodeSearchCount = 5000;

        Stack<Vector2> currentRoute = new Stack<Vector2>();
        Vector2 currentStep = Vector2.zero;
        Rigidbody2D rb2d;

        bool isMovingTowardTarget = false;

        public Stack<Vector2> GetPath(Transform target)
        {
            var targetPosition = (Vector2)target.position;
            var finishNode = this.GetFinishNode(targetPosition);
            var path = new Stack<Vector2>();

            if (finishNode != default)
            {
                return this.BuildPath(finishNode, path);
            }

            return path;
        }

        Stack<Vector2> BuildPath(Node currentNode, Stack<Vector2> path)
        {
            path.Push(currentNode.position);

            if (currentNode.position == this.rb2d.position)
            {
                return path;
            }

            return this.BuildPath(currentNode.parent, path);
        }

        protected void Start()
        {
            this.rb2d = this.GetComponent<Rigidbody2D>();
            this.currentStep = this.rb2d.position;
        }

        void FixedUpdate()
        {
            var isEmptyRoute = this.currentRoute.Count == 0;
            var hasReachedDestination = (Vector2)this.target.position == this.rb2d.position;
            var hasReachedCurrentStep = this.currentStep == this.rb2d.position;

            if (isEmptyRoute)
            {
                this.currentRoute = this.currentRoute = this.GetPath(this.target);
            }

            if (!isEmptyRoute && hasReachedCurrentStep)
            {
                this.currentStep = this.currentRoute.Pop();
            }

            if (!hasReachedCurrentStep && !hasReachedDestination)
            {
                this.rb2d.position = Vector2.MoveTowards(this.rb2d.position, this.currentStep, this.movementSpeed);
            }
        }

        Node GetFinishNode(Vector2 target)
        {
            var openNodes = new HashSet<Node>();
            var closedNodes = new HashSet<Node>();

            openNodes.Add(new Node { position = this.rb2d.position });

            while (openNodes.Any())
            {
                var lowestCostNode = openNodes.PopLowestCostNode();
                if (lowestCostNode.position == target || this.maxNodeSearchCount <= closedNodes.Count)
                {
                    return lowestCostNode;
                }

                var walkableAdjacentNodes = lowestCostNode.GetAdjacentWalkableNodes(this.blockingLayer, target);

                foreach (var walkableNode in walkableAdjacentNodes)
                {
                    if (closedNodes.Any(closedNode => closedNode.position == walkableNode.position))
                    {
                        continue;
                    }

                    foreach (var openNode in openNodes)
                    {
                        if (openNode.position == walkableNode.position && openNode.cost > walkableNode.cost)
                        {
                            openNode.cost = walkableNode.cost;
                            openNode.parent = walkableNode.parent;

                            continue;
                        }
                    }

                    if (openNodes.Any(openNode => openNode.position == walkableNode.position))
                    {
                        continue;
                    }

                    openNodes.Add(walkableNode);
                }

                closedNodes.Add(lowestCostNode);
            }

            return closedNodes.Aggregate((curMin, node) =>
                curMin.position.GetManhattanDistance(target) > node.position.GetManhattanDistance(target) ? node : curMin);
        }
    }
}

