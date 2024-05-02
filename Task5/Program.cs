using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
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
    /// Algorithm itself
    /// </summary>
    static public IEnumerable<string> Steps(Train head) {
        head.LightsOn = !head.LightsOn;
        bool TargetConvert = head.LightsOn;
        uint amount_looped = 0;
        uint amount_converted = 1;
        Func<string> prefix = () => "looped="+amount_looped + ", converted="+amount_converted + ' ';
        yield return prefix() + "Инвертирование текущей позиции";
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
        for (uint i = 1; i < length; i++) { Console.Write("   " + (i+1)); }
        Console.WriteLine("\n\t" + PrintTrain() + " : Initialize");
        try {
            uint counter = 0;
            foreach (string step_info in Train.Steps(head)) {
                Console.WriteLine("\t" + PrintTrain() + " : " + step_info);
                counter++;
                if (counter > 99) {
                    Console.WriteLine("Finished because reached maximum steps: " + counter);
                    break;
                }
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
