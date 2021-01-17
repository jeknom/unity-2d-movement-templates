using System;
using UnityEngine;

namespace Pathfinder
{
    [Serializable]
    public class Node
    {
        public int movementCost;
        public int estimatedMovementCost;
        public int cost;
        public Vector2 position;
        public Node parent;
    }
}

