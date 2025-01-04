// https://adventofcode.com/2024/day/5

var pageRules = File.ReadLines("Input.txt")
    .Select(line => line.Split('|') is { Length: 2 } parts ? parts.Select(int.Parse).ToList() : null)
    .Where(parts => parts != null)
    .ToArray();

var pageOrders = File.ReadLines("Input.txt")
    .Select(line => line.Split(',') is { Length: > 1 } parts ? parts.Select(int.Parse).ToList() : null)
    .Where(parts => parts != null)
    .ToArray();

var part1Total = 0;
var part2Total = 0;
foreach (var pageOrder in pageOrders)
{
    var applicableRules = pageRules
        .Where(pages => pages.All(pageOrder.Contains))
        .ToList();

    if (applicableRules.All(pages => pageOrder.IndexOf(pages[0]) < pageOrder.IndexOf(pages[1])))
    {
        part1Total += pageOrder[pageOrder.Count / 2];
    }
    else
    {
        // Part 2

        // List will be rebuilt in the correct order.
        var newOrder = new List<int>();
        while (newOrder.Count != pageOrder.Count)
        {
            // To be in the correct order, the page needs to never appears the second number in remaining applicable pages.
            foreach (var page in pageOrder.Except(newOrder))
            {
                if (applicableRules.All(pages => pages[1] != page))
                {
                    newOrder.Add(page);

                    // Remove all pages which the current is the first.
                    // These rules do not apply anymore.
                    for (var i = applicableRules.Count - 1; i >= 0; i--)
                    {
                        if (applicableRules[i][0] == page)
                        {
                            applicableRules.RemoveAt(i);
                        }
                    }

                    break;
                }
            }
        }

        part2Total += newOrder[newOrder.Count / 2];
    }
}

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);//6624 too high
