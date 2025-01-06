// https://adventofcode.com/2024/day/6

using System.Drawing;

// Lookup by the current direction which is moving (key) and the direction to turn when a barrier is hit (value).
var turnDirections = new Dictionary<Point, Point>()
{
    { new Point(0, -1), new Point(1, 0) }, // Moving up, turn right.
    { new Point(1, 0), new Point(0, 1) }, // Moving right, turn down.
    { new Point(0, 1), new Point(-1, 0) }, // Moving down, turn left.
    { new Point(-1, 0), new Point(0, -1) }, // Moving left, turn up.
};

(Point Position, Point Direction) start = default;

var input = File.ReadLines("Input.txt").ToArray();
var grid = new char[input.First().Length, input.Length];
for (var y = 0; y < grid.GetLength(1); y++)
{
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        grid[x, y] = input[y][x];
        if (grid[x, y] == '^')
        {
            start = (new Point(x, y), turnDirections.Keys.First());
        }
    }
}

List<Point> getPath(Point? hypotheticalBarrier, out bool infiniteLoop)
{
    infiniteLoop = false;

    var startingPosition = start.Position;
    var currentDirection = turnDirections.Keys.First();

    var path = new List<Point> { startingPosition };
    var traveled = new HashSet<string> { getPathTraveledKey(startingPosition, currentDirection) };

    while (true)
    {
        var nextPosition = new Point(startingPosition.X + currentDirection.X, startingPosition.Y + currentDirection.Y);

        // Check if still in bounds.
        if (false
            || (nextPosition.X < 0) || (nextPosition.X >= grid.GetLength(0))
            || (nextPosition.Y < 0) || (nextPosition.Y >= grid.GetLength(1)))
        {
            return path;
        }

        // Check for next position running into a barrier.
        if (false
            || (grid[nextPosition.X, nextPosition.Y] == '#')
            || (nextPosition == hypotheticalBarrier))
        {
            currentDirection = turnDirections[currentDirection];
        }
        else
        {
            // Ensure this is a unique travel point, otherwise an infinite loop results.
            if (!traveled.Add(getPathTraveledKey(nextPosition, currentDirection)))
            {
                infiniteLoop = true;
                return path;
            }

            path.Add(nextPosition);
            startingPosition = nextPosition;
        }
    }

    static string getPathTraveledKey(Point position, Point direction) => $"{position}_{direction}";
}

// Infinite loop is not possible without additional barriers.
var distinctPositions = getPath(null, out _)
    .Distinct()
    .ToArray();

var part2Count = distinctPositions
    .Count(position => getPath(position, out var infiniteLoop) is { } _ && infiniteLoop);

Console.WriteLine(distinctPositions.Length);
Console.WriteLine(part2Count);
