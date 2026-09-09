using System.Runtime.CompilerServices;
using UndocumentedProof.Types;

using System.Runtime.InteropServices;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Замена типа у живого объекта. У объекта в куче перед данными идут два
/// служебных поля по 8 байт: заголовок для блока синхронизации и указатель
/// на таблицу методов. Если записать в это место указатель на таблицу
/// другого типа, рантайм начнёт считать объект этим типом.
///
/// Оставлять объект в таком виде нельзя: сборщик мусора прочитает его поля
/// по описанию чужого типа и уронит процесс. Поэтому прежний указатель возвращается на место
/// в том же методе, а после возврата отчёт вызывает сборку мусора и проверяет,
/// что куча цела.
/// </summary>
internal static class MethodTableSwap
{
    /// <summary>Выводит, кем объект считает себя до и после замены.</summary>
    internal static int Run()
    {
        Console.WriteLine("Рантайм: " + Environment.Version);
        Console.WriteLine("Система: " + RuntimeInformation.OSDescription.Trim()
                          + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine();

        int[] numbers = [111, 222, 333];
        const string sample = "Шпион";

        Console.WriteLine("До замены");
        Console.WriteLine("  массив отвечает: " + numbers.GetType());
        Console.WriteLine("  строка отвечает: " + sample.GetType());
        Console.WriteLine();

        string typeAfter = Swap(numbers, sample, out string content);

        Console.WriteLine("После замены");
        Console.WriteLine("  массив отвечает: " + typeAfter);
        Console.WriteLine("  содержимое как строка: " + Describe(content));
        Console.WriteLine();

        Console.WriteLine("После возврата");
        Console.WriteLine("  массив отвечает: " + numbers.GetType());
        Console.WriteLine("  первый элемент:  " + numbers[0]);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine("  сборка мусора прошла, куча цела");
        Console.WriteLine();
        Console.WriteLine("Сразу за указателем на таблицу методов у строки записана");
        Console.WriteLine("длина в четырёх байтах, а у массива под длину отведено восемь.");
        Console.WriteLine("Строка прочитала первые четыре байта и получила длину 3.");
        Console.WriteLine();
        Console.WriteLine("Символы строка читает сразу за своими четырьмя байтами длины.");
        Console.WriteLine("Там у массива ещё не данные, а оставшиеся четыре байта его");
        Console.WriteLine("длины, и в них нули: отсюда два первых символа, заменённые");
        Console.WriteLine("точками. Третий символ приходится уже на первое число массива:");
        Console.WriteLine("111, а код 111 в Юникоде — латинская o.");

        return 0;
    }

    /// <summary>
    /// Замена и возврат в одном методе. Между ними нет ни выхода, ни вызова
    /// сборки мусора: объект в чужом типе живёт ровно две строки.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string Swap(int[] numbers, string sample, out string content)
    {
        nint original = Innards.MethodTable(numbers);
        nint borrowed = Innards.MethodTable(sample);

        Innards.SetMethodTable(numbers, borrowed);

        string typeName = numbers.GetType().ToString();
        content = Unsafe.As<int[], string>(ref numbers);

        Innards.SetMethodTable(numbers, original);

        return typeName;
    }

    /// <summary>
    /// Вывод содержимого с заменой непечатаемых символов: там необработанная
    /// память, и показать её как есть нельзя.
    /// </summary>
    private static string Describe(string value)
    {
        if (value.Length == 0)
        {
            return "пусто";
        }

        System.Text.StringBuilder builder = new();
        int limit = Math.Min(value.Length, 16);

        for (int i = 0; i < limit; i++)
        {
            char symbol = value[i];
            builder.Append(char.IsControl(symbol) || symbol > 0x7E ? '.' : symbol);
        }

        return builder + "   (длина " + value.Length + ")";
    }
}
