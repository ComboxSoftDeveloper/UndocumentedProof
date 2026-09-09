using UndocumentedProof.Types;

using System.Runtime.InteropServices;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Операторы true и false. Их перегрузка разрешена для любого типа, и после
/// неё объект можно поставить в условие вместо bool.
///
/// Отчёт проверяет, где это работает: в if, в while, в тернарном операторе
/// и в цепочках && и ||.
/// </summary>
internal static class BoolOps
{
    /// <summary>Выводит, в каких местах объект принимается за условие.</summary>
    internal static int Run()
    {
        Console.WriteLine("Рантайм: " + Environment.Version);
        Console.WriteLine("Система: " + RuntimeInformation.OSDescription.Trim()
                          + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine();
        Console.WriteLine("Класс Flag не наследует bool и не приводится к нему.");
        Console.WriteLine("Перегружены только операторы true и false.");
        Console.WriteLine();

        Flag yes = new(true);
        Flag no = new(false);

        Console.WriteLine("  if (yes)                " + (yes ? "сработал" : "не сработал"));
        Console.WriteLine("  if (no)                 " + (no ? "сработал" : "не сработал"));

        int steps = 0;
        while (yes)
        {
            steps++;
            if (steps == 3)
            {
                break;
            }
        }

        Console.WriteLine("  while с объектом        оборотов " + steps);
        Console.WriteLine("  тернарный оператор      " + (yes ? "ветка да" : "ветка нет"));

        Console.WriteLine();
        Console.WriteLine("Ради чего эта пара операторов и нужна");
        Console.WriteLine();

        Flag both = yes && no;
        Flag either = yes || no;

        Console.WriteLine("  yes && no               " + (both.Value ? "да" : "нет"));
        Console.WriteLine("  yes || no               " + (either.Value ? "да" : "нет"));

        Console.WriteLine();
        Console.WriteLine("Короткий && для своего типа собирается из трёх операторов:");
        Console.WriteLine("& считает результат, а true и false решают, нужно ли вообще");
        Console.WriteLine("вычислять правый операнд. Без пары true и false компилятор");
        Console.WriteLine("короткий && не разрешит.");
        Console.WriteLine();
        Console.WriteLine("Приведения к bool у типа при этом нет:");
        Console.WriteLine("  bool copy = yes;        не компилируется");
        Console.WriteLine("  yes == true             не компилируется");

        return 0;
    }
}
