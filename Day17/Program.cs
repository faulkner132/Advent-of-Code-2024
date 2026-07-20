// https://adventofcode.com/2024/day/17

const int A = 0;
const int B = 1;
const int C = 2;

var registers = new Dictionary<int, long>();
int[] program;
string programOutput;
{
    var input = File.ReadAllLines("Input.txt");

    void setRegisterValue(int register) => registers[register] = long.Parse(input[register].Split(':')[1].Trim());
    setRegisterValue(A);
    setRegisterValue(B);
    setRegisterValue(C);

    programOutput = input.First(line => line.StartsWith("Program")).Split(':')[1].Trim();
    program = [.. programOutput.Split(',').Select(int.Parse)];
}

// Providing 'targetOutput' provides a way to short-circuit if there is output which must be matched.
string getOutput(int[]? targetOutput = null)
{
    var output = new List<long>();
    var position = 0;
    while (position < program.Length)
    {
        var result = runInstruction(program[position], program[position + 1]);

        if (result.Output is { } outputValue)
        {
            output.Add(outputValue);
            if (targetOutput != null && targetOutput[Math.Min(output.Count, targetOutput.Length) - 1] != outputValue)
            {
                break;
            }
        }
        position = result.NewPosition ?? (position + result.MoveIncrement);
    }
    return string.Join(",", output);
}

// Part 1 output.
Console.WriteLine(getOutput());


void resetRegisters(long aValue)
{
    registers[A] = aValue;
    registers[B] = 0;
    registers[C] = 0;
}

static string toOctal(long decimalValue) => Convert.ToString(decimalValue, 8);

// Target length to match which signifies when a new output digit was found.
var matchOutputLength = 1;
// The octal values (numbers as strings) which match the target output.
// This records the first/lowest value which was found.
var matchedOctalValues = new List<string>();

// The value to try for the first register.
// It is known that the first digit of the output is the first digit of the first register.
var tryValue = (long)program[0];
// Need to at least increment by a value which will produce additional output.
// This should always be a power of 8, since the output is in octal.
var increment = (long)8;

// This is a bit of brute force, but solves it within a few minutes.
while (true)
{
    resetRegisters(tryValue);
    if (getOutput(program) is { } output && output.Length >= matchOutputLength)
    {
        if (output == programOutput)
        {
            // Part 2 output.
            Console.WriteLine(tryValue);
            break;
        }

        matchedOctalValues.Add(toOctal(tryValue));
        matchOutputLength = Math.Min(programOutput.Length, matchOutputLength + 2);

        // Look for a pattern once there are enough entries.
        // If all the matching values end with the same thing, we can freeze those digits.
        if (matchedOctalValues.Count > 3)
        {
            var patternSearchEntries = matchedOctalValues
                .TakeLast(3)
                .Select(text => new string([.. text.Reverse()]))
                .ToArray();

            // Matching all values is too optimistic, only allow matching up to a certain point.
            var pattern = "";
            for (var i = 0; i < patternSearchEntries[0].Length - 2; i++)
            {
                var matchChar = patternSearchEntries[0][i];
                if (patternSearchEntries.All(text => matchChar == text[i]))
                {
                    pattern += matchChar;
                }
            }
            pattern = new string([.. pattern.Reverse()]);

            // Need to preserve the numbers at the end so the increment can be orders of magnitude in octal.
            increment = Math.Max(increment, (long)Math.Pow(8, pattern.Length));
        }
    }

    tryValue += increment;
}


long getComboValue(int value) => value switch
{
    0 or 1 or 2 or 3 => value,
    4 => registers[A],
    5 => registers[B],
    6 => registers[C],
    _ => throw new NotSupportedException(),
};

(long? Output, int MoveIncrement, int? NewPosition) runInstruction(int opcode, int operand)
{
    long? instructionOutput = null;
    var moveIncrement = 2;
    int? newPosition = null;

    switch (opcode)
    {
        // adv
        case 0:
            registers[A] = (long)(registers[A] / Math.Pow(2, getComboValue(operand)));
            break;
        // bxl
        case 1:
            registers[B] ^= operand;
            break;
        // bst
        case 2:
            registers[B] = getComboValue(operand) % 8;
            break;
        // jnz
        case 3 when registers[A] != 0:
            newPosition = operand;
            moveIncrement = 0;
            break;
        // bxc
        case 4:
            registers[B] ^= registers[C];
            break;
        // out
        case 5:
            instructionOutput = getComboValue(operand) % 8;
            break;
        // bdv
        case 6:
            registers[B] = (long)(registers[A] / Math.Pow(2, getComboValue(operand)));
            break;
        // cdv
        case 7:
            registers[C] = (long)(registers[A] / Math.Pow(2, getComboValue(operand)));
            break;
    }

    return (instructionOutput, moveIncrement, newPosition);
}