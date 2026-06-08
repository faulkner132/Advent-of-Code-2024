// https://adventofcode.com/2024/day/15

using System.Drawing;

const char Wall = '#';
const char Box = 'O';
const char BoxLeft = '[';
const char BoxRight = ']';
const char Empty = '.';
const char Robot = '@';

var part1Map = new Dictionary<Point, char>();
var part2Map = new Dictionary<Point, char>();
var movementSequence = "";

foreach (var (line, y) in File.ReadAllLines("Input.txt").Select((line, y) => (line, y)))
{
    if (line.StartsWith(Wall))
    {
        for (var x = 0; x < line.Length; x++)
        {
            part1Map.Add(new Point(x, y), line[x]);

            char[] part2Chars = line[x] switch
            {
                Wall => [Wall, Wall],
                Box => [BoxLeft, BoxRight],
                Empty => [Empty, Empty],
                Robot => [Robot, Empty],
                _ => ['X', 'X'],
            };
            part2Map.Add(new Point(x * 2, y), part2Chars[0]);
            part2Map.Add(new Point((x * 2) + 1, y), part2Chars[1]);
        }
    }
    else
    {
        movementSequence += line;
    }
}

static Point getRobotPosition(Dictionary<Point, char> map) => map.Single(entry => entry.Value == Robot).Key;

static Point getNextPosition(Point startingPoint, char direction)
{
    var result = startingPoint;
    if (direction == '<')
    {
        result.X--;
    }
    else if (direction == '>')
    {
        result.X++;
    }
    else if (direction == '^')
    {
        result.Y--;
    }
    else if (direction == 'v')
    {
        result.Y++;
    }
    return result;
}

static bool tryMove(Dictionary<Point, char> map, List<Point> currentPositions, char direction, List<(Point Position, char NewSymbol)> movementList)
{
    var potentialMovement = currentPositions
        .Select(currentPosition => (currentPosition, Empty))
        .ToList();

    var movementPositions = new List<Point>();
    var isRobotMovement = false;
    var canMove = true;
    foreach (var currentPosition in currentPositions)
    {
        var currentSymbol = map[currentPosition];
        isRobotMovement = currentSymbol == Robot;

        var moveToPosition = getNextPosition(currentPosition, direction);
        var moveToSymbol = map[moveToPosition];

        // Hitting a wall at any point is the only condition where a move cannot be done.
        if (moveToSymbol == Wall)
        {
            return false;
        }

        // Queue all the box positions which are impacted by this movement.
        // All impacted boxes by the current positions needs to be processed as part of the same pass.
        movementPositions.AddRange(moveToSymbol switch
        {
            Box => [moveToPosition],
            // Two-sided boxes, when pushed from the side, need to ignore the other side of the box when detecting collisions.
            BoxLeft when direction != '<' => [moveToPosition, getNextPosition(moveToPosition, '>')],
            BoxRight when direction != '>' => [moveToPosition, getNextPosition(moveToPosition, '<')],
            _ => [],
        });

        potentialMovement.Add((moveToPosition, currentSymbol));
    }

    if (movementPositions.Count > 0)
    {
        canMove = tryMove(map, movementPositions, direction, movementList);
    }

    if (canMove)
    {
        movementList.AddRange(potentialMovement);

        // Call with the robot movement should process the queued movements.
        if (isRobotMovement)
        {
            foreach (var (position, newSymbol) in movementList)
            {
                map[position] = newSymbol;
            }
        }
    }

    return canMove;
}

foreach (var direction in movementSequence)
{
    tryMove(part1Map, [getRobotPosition(part1Map)], direction, []);
    tryMove(part2Map, [getRobotPosition(part2Map)], direction, []);
}

var part1Total = part1Map
    .Where(entry => entry.Value == Box)
    .Select(entry => entry.Key)
    .Sum(point => (point.Y * 100) + point.X);

var part2Total = part2Map
    .Where(entry => entry.Value == BoxLeft)
    .Select(entry => entry.Key)
    .Sum(point => (point.Y * 100) + point.X);

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
