// https://adventofcode.com/2024/day/3

const bool NoRegex = true;
const string KeyText = "mul(";

var part1Total = 0;
var part2Total = 0;
var part2Enabled = true;
foreach (var line in File.ReadLines("Input.txt"))
{
    List<int> getEnableToggleIndexes(string command)
    {
        var indexes = new List<int>();
        while (!indexes.Contains(-1))
        {
            indexes.Add(line.IndexOf(command, indexes.DefaultIfEmpty(-1).LastOrDefault() + 1));
        }
        indexes.Sort();
        return indexes;
    }
    var dos = getEnableToggleIndexes("do()");
    var donts = getEnableToggleIndexes("don't()");

    var startIndex = 0;
    while (NoRegex)
    {
        var openIndex = line.IndexOf(KeyText, startIndex);
        var closeIndex = line.IndexOf(')', openIndex + 1);
        if (openIndex == -1)
        {
            break;
        }

        // Close parenthesis is afterward and there are exactly 2 parts with only numbers.
        if ((openIndex < closeIndex)
            && line[openIndex..closeIndex].Replace(KeyText, "").Split(',') is { Length: 2 } parts
            && parts.SelectMany(part => part).All(char.IsDigit))
        {
            var number = int.Parse(parts[0]) * int.Parse(parts[1]);
            part1Total += number;

            // Latest enablement position determines the result.
            // In the event of a tie, defer to what the previous line had set.
            var lastDoIndex = dos.Last(index => index < openIndex);
            var lastDontIndex = donts.Last(index => index < openIndex);
            if ((lastDoIndex > lastDontIndex)
                || ((lastDoIndex == lastDontIndex) && part2Enabled))
            {
                part2Total += number;
            }
        }

        startIndex = openIndex + 1;
    }

    // Set the default enabled state for the start of the new line.
    part2Enabled = dos.Last() > donts.Last();
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);