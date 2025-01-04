// https://adventofcode.com/2024/day/1

var list1 = new List<int>();
var list2 = new List<int>();

foreach (var line in File.ReadLines("Input.txt")
    .Select(line => line.Split([' '], StringSplitOptions.RemoveEmptyEntries)))
{
    list1.Add(int.Parse(line[0]));
    list2.Add(int.Parse(line[1]));
}

// Part 1
{
    list1 = [.. list1.OrderBy(x => x)];
    list2 = [.. list2.OrderBy(x => x)];
    Console.WriteLine(Enumerable.Range(0, list1.Count).Sum(i => Math.Abs(list1[i] - list2[i])));
}

// Part 2
{
    var total = 0;
    foreach (var matchValue in list1)
    {
        total += matchValue * list2.Count(value => value == matchValue);
    }
    Console.WriteLine(total);
}
