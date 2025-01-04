// https://adventofcode.com/2024/day/2

var part1Safe = 0;
var part2Safe = 0;
foreach (var level in File.ReadLines("Input.txt")
    .Select(line => line.Split([' '], StringSplitOptions.RemoveEmptyEntries)))
{
    var values = level.Select(int.Parse).ToList();

    Func<IList<int>, int, bool> orderCheck = values.First() > values.Last()
        ? (list, index) => list[index] > list[index + 1]
        : (list, index) => list[index] < list[index + 1];

    bool indexIsValid(IList<int> list, int index) => true
        && orderCheck(list, index)
        && Math.Abs(list[index] - list[index + 1]) is >= 1 and <= 3;

    bool isSafe(IList<int> list) => Enumerable.Range(0, list.Count - 1).All(i => indexIsValid(list, i));

    if (isSafe(values))
    {
        part1Safe++;
    }
    else
    {
        // Find the first index where trouble appears.
        var troubleIndex = Enumerable.Range(0, values.Count - 1).First(i => !indexIsValid(values, i));

        // Since we can only drop one level, try without the two indexes in question (trouble and the following).
        // If the list is without either one is safe, all is well.
        if (Enumerable.Range(troubleIndex, 2).Any(i => isSafe([.. values[..i], .. values[(i + 1)..]])))
        {
            part2Safe++;
        }
    }
}

Console.WriteLine(part1Safe);
Console.WriteLine(part1Safe + part2Safe);