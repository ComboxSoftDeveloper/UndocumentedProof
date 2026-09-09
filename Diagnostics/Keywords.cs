using System.Runtime.InteropServices;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Скрытые ключевые слова C#. Их нет в справке по языку, среда разработки
/// их не подсвечивает, но компилятор Roslyn их принимает.
///
/// __makeref создаёт TypedReference — пару из указателя на данные и указателя
/// на тип. __reftype достаёт из неё тип, __refvalue — само значение, причём
/// значение можно и записать обратно в исходную переменную.
/// </summary>
internal static class Keywords
{
    /// <summary>Выводит, что получается через скрытые слова.</summary>
    internal static int Run()
    {
        Console.WriteLine("Рантайм: " + Environment.Version);
        Console.WriteLine("Система: " + RuntimeInformation.OSDescription.Trim()
                          + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine();

        int number = 42;
        string text = "Секрет";

        DateTime moment = new(2026, 1, 1);

        TypedReference toNumber = __makeref(number);
        TypedReference toText = __makeref(text);
        TypedReference toMoment = __makeref(moment);

        Console.WriteLine("Тип переменной без вызова GetType");
        Console.WriteLine();
        Console.WriteLine("  int       __reftype даёт " + __reftype(toNumber).Name);
        Console.WriteLine("  string    __reftype даёт " + __reftype(toText).Name);
        Console.WriteLine("  DateTime  __reftype даёт " + __reftype(toMoment).Name);

        Console.WriteLine();
        Console.WriteLine("Разница с GetType");
        Console.WriteLine();
        Console.WriteLine("  число до записи: " + number);

        // Запись идёт прямо в исходную переменную: TypedReference хранит
        // прямой указатель на неё, а не копию значения.
        __refvalue(toNumber, int) = 100;

        Console.WriteLine("  после __refvalue: " + number);

        object boxed = TypedReference.ToObject(toText);
        Console.WriteLine("  TypedReference.ToObject: " + boxed);

        Console.WriteLine();
        Console.WriteLine("GetType работает с объектом и упаковывает значимый тип.");
        Console.WriteLine("__reftype работает с переменной и упаковки не делает.");

        return 0;
    }
}
