using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace GridSystem
{
    public static class PathFinderHandler
    {
        private static readonly List<Vector2Int> Directions = new()
        {
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0,-1)    // Down
        };
        
        public static bool HasAValidPath(in PathTile startTile, in PathTile finishTile, out List<Vector2Int> path)
        {
            path = new List<Vector2Int>();
            if (startTile == null || finishTile == null) return false;

            var startID = startTile.ID;
            var finishID = finishTile.ID;

            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            
            queue.Enqueue(startID);
            visited.Add(startID);
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == finishID)
                {
                    var temp = finishID;

                    while (temp != startID)
                    {
                        path.Add(temp);
                        temp = cameFrom[temp];
                    }
                    
                    path.Add(startID);
                    path.Reverse();
                    return true;
                }

                foreach (var neighbor in Directions.Select(direction => current + direction))
                {
                    if (visited.Contains(neighbor) ||
                        !GridManager.GridField.TryGetValue(neighbor, out var neighborTile)) continue;
                    
                    if (neighborTile is not PathTile) continue;
                    
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
            
            return false;
        }
    }
}
