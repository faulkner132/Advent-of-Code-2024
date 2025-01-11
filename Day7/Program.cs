// https://adventofcode.com/2024/day/7

Func<long, int, long>[] operations =
[
    (value1, value2) => value1 + value2,
    (value1, value2) => value1 * value2,
    (value1, value2) => long.Parse($"{value1}{value2}"), // ||
];

long part1Total = 0;
long part2Total = 0;
foreach (var (total, factors) in File.ReadLines("Input.txt")
    .Select(line => line.Split(':'))
    .Select(data =>
    (
        total: long.Parse(data[0]),
        factors: data[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
    ))
{
    var part1Match = isMatch(0, factors, operations[..2]);
    if (part1Match)
    {
        part1Total += total;
    }
    if (part1Match || isMatch(0, factors, operations))
    {
        part2Total += total;
    }

    bool isMatch(long currentTotal, int[] evalFactors, Func<long, int, long>[] availableOperations)
    {
        if (evalFactors.Length == 0)
        {
            return total == currentTotal;
        }

        foreach (var operation in availableOperations)
        {
            if (isMatch(operation(currentTotal, evalFactors[0]), [.. evalFactors[1..]], availableOperations))
            {
                return true;
            }
        }
        return false;
    }
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
