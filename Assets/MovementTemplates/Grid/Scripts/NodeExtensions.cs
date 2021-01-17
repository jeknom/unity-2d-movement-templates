using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public static class NodeExtensions
    {
        public static List<Node> GetAdjacentWalkableNodes(this Node parentNode, LayerMask blockingLayer, Vector3 target)
        {
            var nodePosition = parentNode.position;
            var nodes = new List<Node>();

            if (!Physics2D.Raycast(nodePosition + Vector2.left, Vector2.zero, blockingLayer))
                nodes.Add(CreateNode(parentNode, Vector2.left, target));

            if (!Physics2D.Raycast(nodePosition + Vector2.right, Vector2.zero, blockingLayer))
                nodes.Add(CreateNode(parentNode, Vector2.right, target));

            if (!Physics2D.Raycast(nodePosition + Vector2.up, Vector2.zero, blockingLayer))
                nodes.Add(CreateNode(parentNode, Vector2.up, target));

            if (!Physics2D.Raycast(nodePosition + Vector2.down, Vector2.zero, blockingLayer))
                nodes.Add(CreateNode(parentNode, Vector2.down, target));

            return nodes;
        }

        static Node CreateNode(Node parent, Vector2 direction, Vector2 target)
        {
            var nodePosition = parent.position + direction;
            var nodeCostEstimation = nodePosition.GetManhattanDistance(target);
            var nodeMovementCost = parent.movementCost + 1;
            var node = new Node
            {
                movementCost = nodeMovementCost,
                estimatedMovementCost = nodeCostEstimation,
                cost = nodeMovementCost + nodeCostEstimation,
                position = nodePosition,
                parent = parent
            };

            return node;
        }
    }
}

