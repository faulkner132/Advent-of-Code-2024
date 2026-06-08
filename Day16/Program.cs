// https://adventofcode.com/2024/day/16

using System.Drawing;
using static Helpers;

const char Wall = '#';
const char Start = 'S';
const char End = 'E';

var maze = new Dictionary<Point, char>();

foreach (var (line, y) in File.ReadAllLines("Input.txt").Select((line, y) => (line, y)))
{
    for (var x = 0; x < line.Length; x++)
    {
        maze.Add(new Point(x, y), line[x]);
    }
}

var startPosition = maze.First(position => position.Value == Start).Key;
// Offset/delta for movement. The start is always facing east/moving right.
var startDirection = new Point(1, 0);

static Point getOppositeDirection(Point direction) => new(direction.X * -1, direction.Y * -1);

var directions = new List<Point>()
{
    new(-1, 0), // Left
    new(1, 0), // Right
    new(0, -1), // Up
    new(0, 1), // Down
};

var graph = new Dictionary<Point, HashSet<Line>>();
void buildGraph(Point startPoint, Point currentPoint, Point currentDirection)
{
    if (graph.TryGetValue(startPoint, out var lines) && lines.Any(existingLine => existingLine.Direction == currentDirection))
    {
        // Already included, no need to traverse.
        return;
    }
    
    var moveDirections = directions
        .Where(moveDirection => moveDirection != getOppositeDirection(currentDirection)) // Exclude backwards
        .Select(moveDirection =>
        (
            Direction: moveDirection,
            Point: Move(currentPoint, moveDirection)
        ))
        .Where(current => maze.TryGetValue(current.Point, out var currentChar) && currentChar != Wall)
        .ToArray();

    // There is at most only a single path forward. This is not a node point.
    if (moveDirections.Length == 1 && moveDirections.First() is var forwardPoint && forwardPoint.Direction == currentDirection)
    {
        buildGraph(startPoint, forwardPoint.Point, forwardPoint.Direction);
        return;
    }

    // An intersection, change of direction, and/or deadend exists. This is a node point.
    if (new Line(startPoint, currentPoint) is { Cost: > 0 } line)
    {
        // Add in both directions.
        _ = graph.TryAdd(startPoint, []);
        graph[startPoint].Add(line);
        
        _ = graph.TryAdd(currentPoint, []);
        graph[currentPoint].Add(new Line(currentPoint, startPoint));
    }

    // Start new line definitions for the remaining segments.
    foreach (var next in moveDirections)
    {
        buildGraph(currentPoint, next.Point, next.Direction);
    }
}
buildGraph(startPosition, startPosition, startDirection);

var endPosition = maze.First(position => position.Value == End).Key;


Dictionary<Point, CostPath> initializeCosts() => graph
    .Select(entry => entry.Key)
    .Distinct()
    .ToDictionary(point => point, _ => new CostPath(int.MaxValue, new Point(), []));

var part1Costs = initializeCosts();
part1Costs[startPosition] = new CostPath(0, startDirection, []);

void dijkstra(Dictionary<Point, CostPath> pointMap, IEnumerable<Point>? preprocessedPoints = null)
{
    var processedPoints = preprocessedPoints?.ToHashSet() ?? [];
    var processingQueue = new PriorityQueue<Point, int>();
    foreach (var pointEntry in pointMap.Where(entry => entry.Value.Cost < int.MaxValue))
    {
        processingQueue.Enqueue(pointEntry.Key, pointEntry.Value.Cost);
    }
    
    Line[] getUnvisitedNeighbors(Point point) => [..graph[point].Where(line => !processedPoints.Contains(line.End))];

    //pointMap.Keys.Any(point => ))
    while (processingQueue.TryDequeue(out var currentPoint, out _))
    {
        if (currentPoint == endPosition || pointMap[currentPoint].Cost >= int.MaxValue)
        {
            return;
        }
        
        processedPoints.Add(currentPoint);

        foreach (var nextLine in getUnvisitedNeighbors(currentPoint))
        {
            var possibleCost = pointMap[currentPoint].Cost + nextLine.CostFrom(pointMap[currentPoint].Direction);

            if (possibleCost < pointMap[nextLine.End].Cost)
            {
                pointMap[nextLine.End] = new CostPath(possibleCost, nextLine.Direction, [..pointMap[currentPoint].Path.Append(nextLine)]);
                processingQueue.Enqueue(nextLine.End, possibleCost);
            }
        }
    }
}

dijkstra(part1Costs);
var part1Total = part1Costs[endPosition].Cost;


var uniquePoints = new HashSet<Point>();
var optimalPoints = new HashSet<Point>();
void addPointsOnPath(IEnumerable<Line> lines)
{
    foreach (var line in lines)
    {
        optimalPoints.Add(line.Start);
        optimalPoints.Add(line.End);
        foreach (var point in line.GetPoints())
        {
            uniquePoints.Add(point);
        }
    }
}
addPointsOnPath(part1Costs[endPosition].Path);


var processedAlternatePoints = new HashSet<Point>();
foreach (var possibleAlternatePoint in part1Costs)
{
    if (optimalPoints.Contains(possibleAlternatePoint.Key) || processedAlternatePoints.Contains(possibleAlternatePoint.Key))
    {
        continue;
    }
    
    var alternatePointCosts = initializeCosts();
    alternatePointCosts[possibleAlternatePoint.Key] = possibleAlternatePoint.Value;
    dijkstra(alternatePointCosts, possibleAlternatePoint.Value.Path.Select(line => line.Start));

    processedAlternatePoints.Add(possibleAlternatePoint.Key);
    if (alternatePointCosts[endPosition].Cost == part1Total)
    {
        addPointsOnPath(alternatePointCosts[endPosition].Path);
    }
}

// At this point, all optimal points are known.
// Any remaining paths would be direct connections between these points which were not picked up (because the cost is the same).
// A DFS can now be used since we can limit the nodes to known optimal points.
var optimalGraph = graph
    .Where(entry => optimalPoints.Contains(entry.Key))
    .ToDictionary(entry => entry.Key, entry => entry.Value);
void traverseOptimalGraph(int cost, Line[] path)
{
    if (cost > part1Total)
    {
        return;
    }

    var (currentPoint, currentDirection) = path.LastOrDefault() is { } lastLine
        ? (lastLine.End, lastLine.Direction)
        : (startPosition, startDirection);

    if (!optimalGraph.TryGetValue(currentPoint, out var neighbors))
    {
        return;
    }
    
    if (currentPoint == endPosition)
    {
        addPointsOnPath(path);
        return;
    }

    foreach (var nextLine in neighbors.Where(line => !path.Contains(line)))
    {
        traverseOptimalGraph(cost + nextLine.CostFrom(currentDirection), [.. path.Append(nextLine)]);
    }
}
traverseOptimalGraph(0, []);
var part2Total = uniquePoints.Count;


Console.WriteLine(part1Total);
Console.WriteLine(part2Total);


internal sealed class Line(Point Point1, Point Point2)
{
    public Point Start => Point1;
    public Point End => Point2;
    public char Axis => Point1.Y == Point2.Y ? 'X' : 'Y';
    public int Cost => Axis == 'X' ? Math.Abs(Point1.X - Point2.X) : Math.Abs(Point1.Y - Point2.Y);
    public Point Direction => Axis == 'X'
        ? new Point(Point1.X < Point2.X ? 1 : -1, 0)
        : new Point(0, Point1.Y < Point2.Y ? 1 : -1);
    
    public IEnumerable<Point> GetPoints()
    {
        var currentPoint = Start;
        while (true)
        {
            yield return currentPoint;
            if (currentPoint == End)
            {
                break;
            }
            currentPoint = Move(currentPoint, Direction);
        }
    }

    public int CostFrom(Point direction) => (new Line(new Point(0, 0), direction).Axis != Axis ? 1000 : 0) + Cost;

    public override bool Equals(object? obj) => obj is Line line && line.GetHashCode() == GetHashCode();
    public override int GetHashCode() => Point1.GetHashCode() + Point2.GetHashCode();
    public override string ToString() => $"{Start} -> {End} [{nameof(Cost)}: {Cost}]";
}

internal record CostPath(int Cost, Point Direction, Line[] Path);

static class Helpers
{
    public static Point Move(Point position, Point direction) => new(position.X + direction.X, position.Y + direction.Y);
}