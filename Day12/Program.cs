// https://adventofcode.com/2024/day/12

using System.Drawing;

var input = File.ReadAllLines("Input.txt");
var grid = new char[input.First().Length, input.Length];
{
    for (var y = 0; y < input.Length; y++)
    {
        for (var x = 0; x < input[y].Length; x++)
        {
            grid[x, y] = input[y][x];
        }
    }
}

var plotMappings = new List<(char Plant, HashSet<Point> Plots)>();
for (var x = 0; x < grid.GetLength(0); x++)
{
    for (var y = 0; y < grid.GetLength(1); y++)
    {
        var plotPoint = new Point(x, y);
        if (plotMappings.Any(mapping => mapping.Plots.Contains(plotPoint)))
        {
            continue;
        }

        var plant = grid[x, y];
        plotMappings.Add((plant, determinePlots([plotPoint])));
        HashSet<Point> determinePlots(HashSet<Point> plots)
        {
            var adjacentPoints = getAdjacentPoints(plots.Last())
                .Where(point => true
                    && (point.X >= 0) && (point.X < grid.GetLength(0))
                    && (point.Y >= 0) && (point.Y < grid.GetLength(1)))
                .Where(point => !plots.Contains(point))
                .Where(point => grid[point.X, point.Y] == plant);

            foreach (var adjacentPoint in adjacentPoints)
            {
                plots.Add(adjacentPoint);
                determinePlots(plots);
            }
            return plots;
        }
    }
}

var part1Total = 0;
var part2Total = 0;
foreach (var (_, plots) in plotMappings)
{
    // It is expected that the same point will appear multiple times in the list.
    var fenceNeighbors = new List<Point>();

    foreach (var point in plots
        .SelectMany(getAdjacentPoints)
        .Where(point => !plots.Contains(point)))
    {
        fenceNeighbors.Add(point);
    }

    part1Total += plots.Count * fenceNeighbors.Count;


    // The count of each distinct run is the total number of continuous straight sections.
    // The length of each entry does not matter.
    // Each entry within the neighbors list will be accounted for by an entry in this list.
    var sections = new List<HashSet<Point>>();

    // Track points and offsets already processed so they do not get double counted.
    var processed = new HashSet<string>();
    static string getProcessedHash(Point point, Point offset) => $"{point}_{offset}";
    
    while (fenceNeighbors.Count > 0)
    {
        var startPoint = fenceNeighbors.First();

        // Determine the direction/offset of this neighbor to the plots themselves. 
        var offsets = getAdjacentPoints(startPoint)
            .Where(plots.Contains)
            // This is the offset to use for a continuous section.
            .Select(point => getPointDifference(startPoint, point))
            // If this combination of point and offset has already been processed then exclude it so we do not double count.
            .Where(offset => !processed.Contains(getProcessedHash(startPoint, offset)))
            .ToArray();

        foreach (var offset in offsets)
        {
            var section = determineSection([startPoint], offset);
            sections.Add(section);
            foreach (var point in section)
            {
                processed.Add(getProcessedHash(point, offset));
                fenceNeighbors.Remove(point);
            }
        }

        HashSet<Point> determineSection(HashSet<Point> sectionRun, Point offset)
        {
            var currentPoint = sectionRun.Last();

            var adjacentPoints = getAdjacentPoints(currentPoint)
                .Where(point => !sectionRun.Contains(point))
                .Where(fenceNeighbors.Contains)
                // Only process points which border the adjacent neighbors with the same offset.
                .Where(point => plots.Contains(getPointDifference(point, offset)))
                .ToArray();

            foreach (var adjacentPoint in adjacentPoints)
            {
                sectionRun.Add(adjacentPoint);
                determineSection(sectionRun, offset);
            }
            return sectionRun;
        }
    }

    part2Total += plots.Count * sections.Count;
}

static Point getPointDifference(Point point1, Point point2) => new(point1.X - point2.X, point1.Y - point2.Y);

static IEnumerable<Point> getAdjacentPoints(Point startPoint) =>
[
    startPoint with { Y = startPoint.Y - 1 }, // Up
    startPoint with { X = startPoint.X + 1 }, // Right
    startPoint with { Y = startPoint.Y + 1 }, // Down
    startPoint with { X = startPoint.X - 1 }, // Left
];

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
