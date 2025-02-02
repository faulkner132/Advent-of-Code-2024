// https://adventofcode.com/2024/day/14

using System.Drawing;
using System.Text.RegularExpressions;

var robotInfo = new List<(Point Start, Point Movement)>();
foreach (var line in File.ReadAllLines("Input.txt"))
{
    var data = line.Split(['=', ',', ' '])
        .Where(value => int.TryParse(value, out _))
        .Select(int.Parse)
        .ToArray();

    robotInfo.Add((new Point(data[0], data[1]), new Point(data[2], data[3])));
}

// Zero based.
var maxPoint = new Point(101 - 1, 103 - 1);

var part1Total = 0;
var part2Seconds = 0;

// Part 1
{
    var positions = robotInfo.Select(info => info.Start).ToArray();
    for (var second = 1; second <= 100; second++)
    {
        moveRobots(positions);
    }

    var (xSplit, ySplit) = (maxPoint.X / 2, maxPoint.Y / 2);
    part1Total = positions
        .Select(point =>
        {
            if ((point.X < xSplit) && (point.Y < ySplit))
            {
                return 1;
            }
            if ((point.X > xSplit) && (point.Y < ySplit))
            {
                return 2;
            }
            if ((point.X < xSplit) && (point.Y > ySplit))
            {
                return 3;
            }
            if ((point.X > xSplit) && (point.Y > ySplit))
            {
                return 4;
            }
            return 0;
        })
        .Where(quadrant => quadrant > 0)
        .GroupBy(quadrant => quadrant)
        .Select(group => group.Count())
        .Aggregate((total, count) => total * count);
}

// Part 2
{
    // In order to form a Christmas tree there would have to have several long continuous lines.
    // The longest would be the base of the tree while and others would be shorter.
    // An assumption is made that this will form through "normal" orientation as though you were looking at it top down.
    
    var positions = robotInfo.Select(info => info.Start).ToArray();
    const int MinContinuousLength = 5;
    while (true)
    {
        var yContinuous = new List<(int YValue, int[] XValues)>();
        foreach (var group in positions
            .GroupBy(point => point.Y)
            .Where(group => group.Count() >= MinContinuousLength))
        {
            var xValues = group.Select(point => point.X).OrderBy(x => x).Distinct().ToArray();
            var orderedX = new List<int>();
            for (var i = 0; i < xValues.Length - 1; i++)
            {
                // Current point and next point must be adjacent or we start a new list.
                if (xValues[i + 1] - xValues[i] == 1)
                {
                    orderedX.Add(xValues[i]);
                }
                else
                {
                    if (orderedX.Count >= MinContinuousLength)
                    {
                        yContinuous.Add((group.Key, orderedX.ToArray()));
                    }
                    // Start a new list with the current point as the first.
                    orderedX = [xValues[i]];
                }
            }
        }

        // There should be this many levels to do deeper inspection.
        if (yContinuous.Count >= 10)
        {
            // Pick the middle point and of the continuous coordinates and make sure [almost] all have this coordinate.
            // If they do, then we very likely have a tree.

            // Ordering by largest and picking from the top should ensure we eliminate "non-branch" continuities.
            yContinuous = [.. yContinuous.OrderByDescending(data => data.XValues.Length)];

            var yMiddleBranch = yContinuous.First().XValues;
            var xMiddle = yMiddleBranch[yMiddleBranch.Length / 2];

            var branchesWithMiddle = yContinuous.Count(data => data.XValues.Contains(xMiddle));
            // If a high percentage have this coordinate, we have (probably) found it.
            if ((double)branchesWithMiddle / yContinuous.Count > .9)
            {
                break;
            }
        }

        part2Seconds++;
        moveRobots(positions);
    }
}


void moveRobots(Point[] positions)
{
    for (var i = 0; i < positions.Length; i++)
    {
        var newPosition = new Point(positions[i].X + robotInfo[i].Movement.X, positions[i].Y + robotInfo[i].Movement.Y);
        if (newPosition.X > maxPoint.X)
        {
            newPosition.X = (newPosition.X - (maxPoint.X + 1)) % maxPoint.X;
        }
        else if (newPosition.X < 0)
        {
            newPosition.X += maxPoint.X + 1;
        }
        if (newPosition.Y > maxPoint.Y)
        {
            newPosition.Y = (newPosition.Y - (maxPoint.Y + 1)) % maxPoint.Y;
        }
        else if (newPosition.Y < 0)
        {
            newPosition.Y += maxPoint.Y + 1;
        }
        positions[i] = newPosition;
    }
}


Console.WriteLine(part1Total);
Console.WriteLine(part2Seconds);
