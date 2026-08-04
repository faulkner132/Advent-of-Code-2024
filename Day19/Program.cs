// https://adventofcode.com/2024/day/19

var input = File.ReadAllLines("Input.txt");

var availablePatterns = input[0].Split(',')
    .Select(pattern => pattern.Trim())
    .GroupBy(pattern => pattern[0])
    .ToDictionary(group => group.Key, group => group.ToArray());
var matchPatterns = input.Skip(2).ToArray();

var part1 = 0;
long part2 = 0;
foreach (var matchPattern in matchPatterns)
{
    if (getTotalMatchCount(matchPattern, []) is var matchCount and > 0)
    {
        part1 += 1;
        part2 += matchCount;
    }
}

long getTotalMatchCount(string toMatch, Dictionary<string, long> patternMatchCount)
{
    // When we get to empty, it is a new match.
    if (string.IsNullOrEmpty(toMatch))
    {
        return 1;
    }
    // If the pattern has already been processed, we know the number of matches.
    if (patternMatchCount.TryGetValue(toMatch, out var matchesFound))
    {
        return matchesFound;
    }

    long matchCount = 0;
    if (availablePatterns.TryGetValue(toMatch[0], out var patterns))
    {
        foreach (var subPattern in patterns.Where(toMatch.StartsWith).Select(pattern => toMatch[pattern.Length..]))
        {
            matchCount += getTotalMatchCount(subPattern, patternMatchCount);
        }
    }

    // Cache the pattern. Anytime this reappears we know the number of matches instantly.
    patternMatchCount[toMatch] = matchCount;
    return matchCount;
}

Console.WriteLine(part1);
Console.WriteLine(part2);
