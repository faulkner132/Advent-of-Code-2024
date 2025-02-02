// https://adventofcode.com/2024/day/13

using System.Drawing;

var input = File.ReadAllLines("Input.txt");

var part1Total = 0;
long part2Total = 0;
for (var i = 0; i < input.Length; i += 4)
{
    var buttonA = getPositions(input[i + 0]);
    var buttonB = getPositions(input[i + 1]);
    var prize = getPositions(input[i + 2]);

    static Point getPositions(string text)
    {
        var numbers = text
            .Split(['+', '=', ','])
            .Select(value =>
            {
                _ = int.TryParse(value, out var result);
                return result;
            })
            .Where(value => value > 0)
            .ToArray();

        return new Point(numbers[0], numbers[1]);
    }

    // Part 1
    {
        var winnerCosts = new List<int>();
        var combinationsAttempted = new HashSet<string>();
        calcWinnerCosts(0, 0, 100);
        part1Total += winnerCosts
            .OrderBy(cost => cost)
            .DefaultIfEmpty(0)
            .FirstOrDefault(cost => cost > 0);

        void calcWinnerCosts(int aButtonPresses, int bButtonPresses, int maxPresses)
        {
            if (!combinationsAttempted.Add($"{aButtonPresses} {bButtonPresses}")
                || (aButtonPresses > maxPresses)
                || (bButtonPresses > maxPresses))
            {
                return;
            }

            var position = addPoint(multiplyPoint(buttonA, aButtonPresses), multiplyPoint(buttonB, bButtonPresses));
            if ((position.X > prize.X) || (position.Y > prize.Y))
            {
                return;
            }

            if (position == prize)
            {
                winnerCosts.Add((aButtonPresses * 3) + bButtonPresses);
            }
            else
            {
                foreach (var (aIncrement, bIncrement) in new[] { (1, 0), (0, 1) })
                {
                    calcWinnerCosts(aButtonPresses + aIncrement, bButtonPresses + bIncrement, maxPresses);
                }
            }

            static Point addPoint(Point point1, Point point2) => new(point1.X + point2.X, point1.Y + point2.Y);
            static Point multiplyPoint(Point point, int multiple) => new(point.X * multiple, point.Y * multiple);
        }
    }

    // Part 2
    // Admittedly, I did look up that linear equations should be used here.
    // As such, I am breaking out this portion of the code instead of refactoring Part 1.
    // The requirement to find the cheapest option is a red herring, there is only a singular solution (if one exists).
    {
        // Multi-variable equations.
        // buttonA.X(aPresses) + buttonB.X(bPresses) = prize.X
        // buttonA.Y(aPresses) + buttonB.Y(bPresses) = prize.Y

        const long PrizeCoordinateOffset = 10000000000000;

        // Set X coordinate multiple (aPresses) to common factor in both equations.
        var buttonAMultiple = new Point(buttonA.X * buttonA.Y, buttonA.Y * buttonA.X);
        var buttonBMultiple = new Point(buttonB.X * buttonA.Y, buttonB.Y * buttonA.X);
        var (prizeX, prizeY) =
        (
            (prize.X + PrizeCoordinateOffset) * buttonA.Y,
            (prize.Y + PrizeCoordinateOffset) * buttonA.X
        );

        // Eliminate X coordinate (aPresses variable) to get the bPresses.
        // In this point, the X coordinate should be 0 and only the Y coordinate will have a value.
        var bPressesPoint = new Point(buttonAMultiple.X - buttonAMultiple.Y, buttonBMultiple.X - buttonBMultiple.Y);

        // When solving for the number of presses, it must be evenly divisible or there is no valid solution.

        // Solve for bPresses.
        var rightSide = prizeX - prizeY;
        if (rightSide % bPressesPoint.Y != 0)
        {
            continue;
        }
        var bPresses = rightSide / bPressesPoint.Y;

        // Solve for aPresses by using the [now] known bPresses value.
        var leftSide = (prize.X + PrizeCoordinateOffset) - (buttonB.X * bPresses);
        if (leftSide % buttonA.X != 0)
        {
            continue;
        }
        var aPresses = leftSide / buttonA.X;

        part2Total += (aPresses * 3) + bPresses;
    }
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
