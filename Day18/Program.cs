// https://adventofcode.com/2024/day/18

using System.Drawing;

const int Bounds = 70;
const int Part1Bytes = 1024;
const char Corrupt = '#';

var input = File.ReadAllLines("Input.txt")
    .Select(line => line.Split(','))
    .Select(corruptPoint => new Point(int.Parse(corruptPoint[0]), int.Parse(corruptPoint[1])))
    .ToArray();

var map = new Dictionary<Point, char>();
var graph = new Dictionary<Point, List<Point>>();

void generateMapAndGraph(int inputLines)
{
    map.Clear();
    for (var x = 0; x <= Bounds; x++)
    {
        for (var y = 0; y <= Bounds; y++)
        {
            map[new Point(x, y)] = '.';
        }
    }

    foreach (var corruptPoint in input.Take(inputLines))
    {
        map[corruptPoint] = Corrupt;
    }

    var directions = new[]
    {    
        new Point(-1, 0),
        new Point(1, 0),
        new Point(0, -1), 
        new Point(0, 1),
    };

    graph.Clear();
    for (var x = 0; x <= Bounds; x++)
    {
        for (var y = 0; y <= Bounds; y++)
        {
            var point = new Point(x, y);
            graph.TryAdd(point, []);
            foreach (var direction in directions)
            {
                if (tryMove(point, direction, out var movePoint))
                {
                    graph[point].Add(movePoint);
                }
            }
        }
    }
    
    bool tryMove(Point position, Point direction, out Point movePoint)
    {
        movePoint = new Point(position.X + direction.X, position.Y + direction.Y);
        return movePoint.X is >= 0 and <= Bounds
            && movePoint.Y is >= 0 and <= Bounds
            && map[movePoint] != Corrupt;
    }
}

var destination = new Point(Bounds, Bounds);

// Performs pathfinding and returns if a path was found along with its cost.
// An optional heuristic can be provided which takes in an unvisited point and returns its priority.
// If no heuristic is provided, Dijkstra's is used to ensure the optimal path.
bool tryFindPath(out int totalCost, Func<Point, int>? heuristic = null)
{
    var visited = new Dictionary<Point, int>() { { new Point(0, 0), 0 } };
    var unvisitedQueue = new PriorityQueue<(Point position, int cost), int>();
    var unvisitedQueuePoints = new HashSet<Point>();
    var position = new Point(0, 0);
    while (position != destination)
    {    
        foreach (var unvisitedPoint in graph[position].Where(point => !visited.ContainsKey(point)))
        {
            if (unvisitedQueuePoints.Add(unvisitedPoint))
            {
                var heuristicValue = heuristic?.Invoke(unvisitedPoint) ?? visited[position] + 1;
                unvisitedQueue.Enqueue((unvisitedPoint, visited[position] + 1), heuristicValue);
            }
        }

        if (unvisitedQueuePoints.Count == 0)
        {
            totalCost = 0;
            return false;
        }

        if (unvisitedQueue.TryDequeue(out var next, out _))
        {
            unvisitedQueuePoints.Remove(next.position);
            
            if (!visited.TryGetValue(next.position, out var currentCost) || next.cost < currentCost)
            {
                visited[next.position] = next.cost;
            }
            
            position = next.position;
        }
    }

    totalCost = visited[destination];
    return true;
}

generateMapAndGraph(Part1Bytes);
_ = tryFindPath(out var part1Cost);
Console.WriteLine(part1Cost);


// Use a binary search to find the last byte where a path is found.
var (minBytes, maxBytes) = (Part1Bytes, input.Length);
var findPathResult = Enumerable.Repeat((bool?)null, input.Length).ToArray();
findPathResult[Part1Bytes - 1] = true;
while (true)
{
    var midBytes = minBytes + ((maxBytes - minBytes) / 2);
    if (midBytes == minBytes)
    {
        break;
    }
    generateMapAndGraph(midBytes);

    // We only need to find a path, not the best.
    // Use a heuristic to prioritize Manhattan distance.
    findPathResult[midBytes] = tryFindPath(out _, point => destination.X - point.X + destination.Y - point.Y);
    if (findPathResult[midBytes] == true)
    {
        minBytes = midBytes;
    }
    else
    {
        maxBytes = midBytes;
    }
}

var part2InputIndex = maxBytes - 1;
Console.WriteLine($"{input[part2InputIndex].X},{input[part2InputIndex].Y}");
