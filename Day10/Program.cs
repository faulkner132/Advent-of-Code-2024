// https://adventofcode.com/2024/day/10

using System.Drawing;

var input = File.ReadLines("Input.txt").ToArray();
var map = new int[input.First().Length, input.Length];
var entryPoints = new List<Point>();
for (var y = 0; y < map.GetLength(1); y++)
{
    for (var x = 0; x < map.GetLength(0); x++)
    {
        map[x, y] = int.Parse(input[y][x].ToString());
        if (map[x, y] == 0)
        {
            entryPoints.Add(new Point(x, y));
        }
    }
}

var part1Total = 0;
var part2Total = 0;
foreach (var entryPoint in entryPoints)
{
    var summitPoints = new HashSet<Point>();
    part2Total += determineSummitPoints([entryPoint]);
    part1Total += summitPoints.Count;

    // Updates 'summitPoints' for unique count, but returns the total number of distinct trails which lead to each summit.
    int determineSummitPoints(HashSet<Point> travelledPath)
    {
        var currentPoint = travelledPath.Last();
        if (map[currentPoint.X, currentPoint.Y] == 9)
        {
            summitPoints.Add(currentPoint);
            return 1;
        }

        var nextPoints = new[]
            {
                currentPoint with { Y = currentPoint.Y - 1 }, // Up
                currentPoint with { X = currentPoint.X + 1 }, // Right
                currentPoint with { Y = currentPoint.Y + 1 }, // Down
                currentPoint with { X = currentPoint.X - 1 }, // Left
            }
            .Where(point => !travelledPath.Contains(point))
            .Where(point => true
                && (point.X >= 0) && (point.X < map.GetLength(0))
                && (point.Y >= 0) && (point.Y < map.GetLength(1)))
            .Where(point => map[point.X, point.Y] == map[currentPoint.X, currentPoint.Y] + 1)
            .ToArray();

        var trailCount = 0;
        foreach (var nextPoint in nextPoints)
        {
            trailCount += determineSummitPoints([..travelledPath, nextPoint]);
        }
        return trailCount;
    }
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
