// https://adventofcode.com/2024/day/11

var input = File.ReadAllText("Input.txt")
    .Split(' ')
    .Select(long.Parse)
    .ToArray();

// Optimize by building a lookup of common value results.
var valueBlinkResultsCache = new Dictionary<string, long>();

valueBlinkResultsCache.Clear();
var part1Count = getSequenceLength(input, 0, 25);

valueBlinkResultsCache.Clear();
var part2Count = getSequenceLength(input, 0, 75);

long getSequenceLength(long[] sequence, int blinkCount, int maxBlinks)
{
    if (blinkCount == maxBlinks)
    {
        return sequence.Length;
    }

    long sequenceTotal = 0;
    foreach (var value in sequence)
    {
        var valueBlinkKey = $"{value},{blinkCount}";
        if (!valueBlinkResultsCache.TryGetValue(valueBlinkKey, out var result))
        {
            long[] valueSequence;
            if (value == 0)
            {
                valueSequence = [1];
            }
            else if (value.ToString() is { } numberString && (numberString.Length % 2 == 0))
            {
                var splitIndex = numberString.Length / 2;
                valueSequence = [long.Parse(numberString[..splitIndex]), long.Parse(numberString[splitIndex..])];
            }
            else
            {
                valueSequence = [value * 2024];
            }
            result = getSequenceLength(valueSequence, blinkCount + 1, maxBlinks);
            valueBlinkResultsCache[valueBlinkKey] = result;
        }
        sequenceTotal += result;
    }
    return sequenceTotal;
}

Console.WriteLine(part1Count);
Console.WriteLine(part2Count);
