using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
CaseGenerator.GenerateCase().Solve();

/// <summary>
/// Circular Linked List Node
/// </summary>
class Train(Train next, Train prev) {
    public class Finished(uint answer) : Exception { public uint answer = answer; }

    public bool LightsOn = CaseGenerator.rnd.Next(2) == 1;
    public Train next = next;
    public Train prev = prev;

    /// <summary>
    /// Algorithm itself
    /// </summary>
    static public IEnumerable<string> Steps(Train head) {
        Func<bool, string> name = (bool v) => v switch {
            true => "горит",
            false => "потух"
        };
        yield return "Начало алгоритма";
        bool targetLightsOn = !head.LightsOn;
        yield return "Решено, что все вагоны будут " + (targetLightsOn ? "гореть" : "потушены");
        head.LightsOn = !head.LightsOn;
        yield return "Инвертирование света в текущем вагоне";
        uint counter = 1;
        while (true) {
            head = head.next;
            yield return "Переход в следующий вагон";
            yield return "Текущий вагон " + name(head.LightsOn);
            if (head.LightsOn != targetLightsOn) {
                head.LightsOn = !head.LightsOn;
                yield return "Инвертирование света в текущем вагоне";
                counter++;
                yield return "Увеличено количество вагонов на 1. Текущее: " + counter;
            } else {
                head.LightsOn = !targetLightsOn;
                yield return "Инвертирование света в текущем вагоне";
                for (uint i = 0; i < counter; i++) head = head.prev;
                yield return "Возвращаемся назад на " + counter + " вагонов";
                if (head.LightsOn == targetLightsOn) {
                    yield return "В текущем вагоне свет не изменился";
                    for (uint i = 0; i < counter; i++) head = head.next;
                    yield return "Возвращаемся вперёд на " + counter + " вагонов";
                    head.LightsOn = !head.LightsOn;
                    yield return "Изменяем свет на изначальный";
                    counter++;
                    yield return "Увеличено количество вагонов на 1. Текущее: " + counter;
                } else {
                    yield return "В текущем вагоне свет изменился "
                        + "=> мы сделали полный круг за " + counter + "вагонов";
                    throw new Finished(counter);
                }
            }
        }
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
                .Append(node.LightsOn ? '+' : '-')
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
        Console.Write('\n');
        try {
            foreach (string step_info in Train.Steps(head)) {
                Console.WriteLine("\t" + PrintTrain() + " : " + step_info);
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
        // uint length = (uint) rnd.Next(4, 9);
        uint length = 15;
        Train[] trains = new Train[length]!;

        trains[0] = new(null, null);
        trains[length-1] = new(null, null);
        for (uint i = 1; i < length-1; i++) {
            trains[i] = new(null, null)!;
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
