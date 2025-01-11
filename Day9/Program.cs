// https://adventofcode.com/2024/day/9

const string FreeSpace = ".";

var input = File.ReadAllText("Input.txt");

var diskmap = new List<string>();
var idBlocks = new List<(int BlockId, int BlockCount)>();
{
    var id = 0;
    for (var i = 0; i < input.Length; i++)
    {
        // Default to freespace.
        var nextBlock = FreeSpace;
        var blockCount = int.Parse(input[i].ToString());

        // Even index represents file which consumes the current id.
        if (i % 2 == 0)
        {
            nextBlock = id.ToString();
            idBlocks.Add((id++, blockCount));
        }
        diskmap.AddRange(Enumerable.Repeat(nextBlock, blockCount));
    }
}

long part1Total;
{
    var part1Blocks = new List<string>(diskmap);
    var moveCount = 0;
    for (var i = 0; i < part1Blocks.Count; i++)
    {
        if (part1Blocks[i] == FreeSpace)
        {
            for (var search = part1Blocks.Count - 1 - moveCount; search >= i; search--)
            {
                if (part1Blocks[search] != FreeSpace)
                {
                    part1Blocks[i] = part1Blocks[search];
                    part1Blocks[search] = FreeSpace;
                    moveCount++;
                    break;
                }
            }
        }
    }
    part1Total = getTotal(part1Blocks);
}

long part2Total;
{
    var part2Blocks = new List<string>(diskmap);
    foreach (var (blockId, blockCount) in Enumerable.Reverse(idBlocks))
    {
        var currentIndex = part2Blocks.IndexOf(blockId.ToString());

        var firstFreespace = part2Blocks.IndexOf(FreeSpace);
        while (firstFreespace < currentIndex)
        {
            if (Enumerable.Range(firstFreespace, blockCount).All(i => part2Blocks[i] == FreeSpace))
            {
                for (var i = 0; i < blockCount; i++)
                {
                    part2Blocks[firstFreespace + i] = blockId.ToString();
                    part2Blocks[currentIndex + i] = FreeSpace;
                }
                break;
            }
            firstFreespace = part2Blocks.IndexOf(FreeSpace, firstFreespace + 1);
        }
    }
    part2Total = getTotal(part2Blocks);
}

static long getTotal(List<string> sequence) => sequence
    .Select((block, i) => (block, i))
    .Where(data => data.block != FreeSpace)
    .Sum(data => data.i * long.Parse(data.block));

Console.WriteLine(part1Total);
Console.WriteLine(part2Total);
