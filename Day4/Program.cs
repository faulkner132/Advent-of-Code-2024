// https://adventofcode.com/2024/day/4

#pragma warning disable IDE0079
#pragma warning disable CA1861

using System.Drawing;

const string Part1Word = "XMAS";
const string Part2Word = "MAS";

var input = File.ReadLines("Input.txt").ToArray();
var grid = new char[input.First().Length, input.Length];
for (var y = 0; y < grid.GetLength(1); y++)
{
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        grid[x, y] = input[y][x];
    }
}

var part1Total = 0;
var part2Total = 0;
for (var x = 0; x < grid.GetLength(0); x++)
{
    for (var y = 0; y < grid.GetLength(1); y++)
    {
        var source = new Point(x, y);

        // Part 1
        {
            // Start at 12 o'clock search pattern.
            foreach (var (xOffset, yOffset) in new[] { 0, 1, 1, 1, 0, -1, -1, -1 }.Zip([-1, -1, 0, 1, 1, 1, 0, -1]))
            {
                var candidate = new[] { source }
                    .Concat(Enumerable.Range(1, Part1Word.Length - 1)
                        .Select(i => new Point(source.X + (xOffset * i), source.Y + (yOffset * i)))
                        .Where(point => (point.X >= 0) && (point.X < grid.GetLength(0)))
                        .Where(point => (point.Y >= 0) && (point.Y < grid.GetLength(1))))
                    .Select(point => grid[point.X, point.Y]);

                if (string.Join("", candidate) == Part1Word)
                {
                    part1Total++;
                }
            }
        }

        // Part 2
        {
            // 2 distinct offset patterns both must match.
            if (true
                && isMatch(new[] { 0, 1, 2 }.Zip([0, 1, 2]))
                && isMatch(new[] { 2, 1, 0 }.Zip([0, 1, 2])))
            {
                part2Total++;
            }

            bool isMatch(IEnumerable<(int X, int Y)> pattern)
            {
                var candidate = pattern
                    .Select(offset => new Point(source.X + offset.X, source.Y + offset.Y))
                    .Where(point => (point.X >= 0) && (point.X < grid.GetLength(0)))
                    .Where(point => (point.Y >= 0) && (point.Y < grid.GetLength(1)))
                    .Select(point => grid[point.X, point.Y]);

                return new[] { Part2Word, string.Join("", Part2Word.Reverse()) }.Contains(string.Join("", candidate));
            }
        }
    }
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);