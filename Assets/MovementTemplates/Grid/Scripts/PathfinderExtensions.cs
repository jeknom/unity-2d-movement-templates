using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinder
{
    public static class PathfinderExtensions
    {
        public static Node PopLowestCostNode(this HashSet<Node> openNodes)
        {
            var lowestCostNode = openNodes.Aggregate((curMin, node) => node.cost < curMin.cost ? node : curMin);
            openNodes.Remove(lowestCostNode);

            return lowestCostNode;
        }

        public static int GetManhattanDistance(this Vector2 a, Vector2 b) =>
            (int)(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }
}

