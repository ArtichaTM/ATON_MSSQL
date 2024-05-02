using System.Diagnostics;
using System.Text;

CaseGenerator.GenerateCase().Solve();

/// <summary>
/// Circular Linked List Node
/// </summary>
class Train(Train? next, Train? prev) {
    public class Finished(uint answer) : Exception { public uint answer = answer; }

    public Train() : this(null, null) {}
    public bool LightsOn = CaseGenerator.rnd.Next(2) == 1;
    public Train? next = next;
    public Train? prev = prev;

    /// <summary>
    /// Algorithm steps
    /// </summary>
    static public IEnumerable<string> Steps(Train head) {
        yield return "{Step info}";
        throw new Finished(99u);
    }
}

/// <summary>
/// Specific Trains case
/// </summary>
class Case(Train head, uint length)
{
    public Train head = head;
    public uint length = length;

    public string PrintTrain() {
        StringBuilder builder = new();
        Train node = head;
        for (uint i = 0; i < length; i++) {
            Debug.Assert(node != null);
            Debug.Assert(node.next != null);
            Debug.Assert(node.prev != null);

            builder
                .Append('[')
                .Append(node.LightsOn ? '-' : '+')
                .Append(']')
                .Append(' ')
                ;
            node = node.next;
        }
        builder.Length -= 1;
        return builder.ToString();
    }

    public void Solve() {
        Console.WriteLine("> Selecting length " + length);
        Console.Write("\t 1");
        for (uint i = 1; i < length; i++) {
            Console.Write("   " + (i+1));
        }
        Console.WriteLine();
        // Console.WriteLine("\t 1   2   3   4   5   6   7   8   9");
        Console.WriteLine('\t' + PrintTrain() + ": Initialize");
        try {
            foreach (string step_info in Train.Steps(head)) {
                Console.WriteLine('\t' + PrintTrain() + ": " + step_info);
            }
        } catch (Train.Finished e) {
            Console.WriteLine("> Finished. Answer: " + e.answer + '\n');
        }
    }
}

/// <summary>
/// Generates Trains cases
/// </summary>
static class CaseGenerator {
    public static Random rnd = new Random();

    public static Case GenerateCase() {
        uint length = (uint) rnd.Next(4, 9);
        Train[] trains = new Train[length];

        trains[0] = new(null, null);
        trains[length-1] = new(null, null);
        for (uint i = 1; i < length-1; i++) {
            trains[i] = new();
        }
        for (uint i = 1; i < length-1; i++) {
            trains[i].prev = trains[i-1];
            trains[i].next = trains[i+1];
        }
        trains[0       ].next = trains[1       ];
        trains[0       ].prev = trains[length-1];
        trains[length-1].next = trains[0       ];
        trains[length-1].prev = trains[length-2];

        return new Case(trains[0], length);
    }

}
