// https://adventofcode.com/2024/day/8

using System.Drawing;

var signalPointLookup = new Dictionary<char, List<Point>>();

var input = File.ReadLines("Input.txt").ToArray();
var maxPoint = new Point(input.First().Length - 1, input.Length - 1);

for (var y = 0; y < input.Length; y++)
{
    for (var x = 0; x < input[y].Length; x++)
    {
        var key = input[y][x];
        if (char.IsLetterOrDigit(key))
        {
            signalPointLookup.TryAdd(key, []);
            signalPointLookup[key].Add(new Point(x, y));
        }
    }
}

var part1Points = new HashSet<Point>();
var part2Points = new HashSet<Point>();
foreach (var signalPoints in signalPointLookup.Values)
{
    // Process each point pairing.
    for (var index1 = 0; index1 < signalPoints.Count - 1; index1++)
    {
        for (var index2 = index1 + 1; index2 < signalPoints.Count; index2++)
        {
            var offset = new Point(
                signalPoints[index2].X - signalPoints[index1].X,
                signalPoints[index2].Y - signalPoints[index1].Y);

            // First position moves up, second position moves down.
            foreach (var (pointIndex, direction) in new[] { (index1, 1), (index2, -1) })
            {
                // The points themselves are extended anti-nodes as well.
                part2Points.Add(signalPoints[index1]);
                part2Points.Add(signalPoints[index2]);

                var distance = 1;
                Point getNextAnitnodePoint() => new(
                    signalPoints[pointIndex].X - (offset.X * distance * direction),
                    signalPoints[pointIndex].Y - (offset.Y * distance * direction));

                var antinodePoint = getNextAnitnodePoint();
                while ((antinodePoint.X >= 0) && (antinodePoint.X <= maxPoint.X)
                    && (antinodePoint.Y >= 0) && (antinodePoint.Y <= maxPoint.Y))
                {
                    // Anti-node immediately beyond each point.
                    if (distance == 1)
                    {
                        part1Points.Add(antinodePoint);
                    }
                    part2Points.Add(antinodePoint);

                    distance++;
                    antinodePoint = getNextAnitnodePoint();
                }
            }
        }
    }
}

Console.WriteLine(part1Points.Count);
Console.WriteLine(part2Points.Count);
