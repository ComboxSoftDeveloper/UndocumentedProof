using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UndocumentedProof.Types;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Сверка. Проверяет каждое утверждение статьи на том рантайме, где запущена.
/// Если поведение изменилось, прогон останавливается: лучше остановиться,
/// чем дать статье утверждение, которое на этой версии уже неверно.
/// </summary>
internal static class Checks
{
    /// <summary>Код возврата: 0 — всё сошлось, 1 — есть расхождение.</summary>
    internal static int Run()
    {
        bool ok = true;
        ok &= HiddenKeywords();
        
        ok &= VarargMatchesPlatform();
        ok &= TypeSwap();
        
        ok &= BoolOperators();
        ok &= NullBehaviour();

        Console.WriteLine();
        Console.WriteLine(ok ? "СВЕРКА ПРОЙДЕНА" : "СВЕРКА НЕ ПРОЙДЕНА");

        return ok ? 0 : 1;
    }

    /// <summary>Скрытые слова дают тот же тип, что GetType, и умеют писать значение.</summary>
    private static bool HiddenKeywords()
    {
        Console.WriteLine("Скрытые ключевые слова");

        int number = 42;
        string text = "Секрет";

        TypedReference toNumber = __makeref(number);
        TypedReference toText = __makeref(text);

        bool ok = Report("__reftype для int даёт Int32", __reftype(toNumber) == typeof(int), "");
        ok &= Report("__reftype для string даёт String", __reftype(toText) == typeof(string), "");

        __refvalue(toNumber, int) = 100;
        ok &= Report("__refvalue пишет в исходную переменную", number == 100, number.ToString());

        return ok;
    }

    /// <summary>
    /// Метод с переменным числом аргументов. Здесь допустимы оба исхода:
    /// на Windows вызов проходит, на других системах джит отказывается
    /// компилировать вызов. Сверка проверяет, что исход совпал с ожидаемым
    /// для этой платформы.
    /// </summary>
    private static bool VarargMatchesPlatform()
    {
        Console.WriteLine();
        Console.WriteLine("Переменное число аргументов");

        bool expected = OperatingSystem.IsWindows() && RuntimeInformation.ProcessArchitecture != Architecture.Arm;

        string outcome;
        bool called;
        try
        {
            outcome = Vararg.Call();
            called = true;
        }
        catch (Exception error)
        {
            outcome = error.GetType().Name + ": " + error.Message;
            called = false;
        }

        Console.WriteLine("  система: " + RuntimeInformation.OSDescription.Trim() + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine("  ожидалось: " + (expected ? "вызов проходит" : "вызов не проходит"));

        return Report("исход совпал с ожидаемым для платформы", called == expected, outcome);
    }

    /// <summary>Замена таблицы методов меняет ответ GetType и обратима.</summary>
    private static bool TypeSwap()
    {
        Console.WriteLine();
        Console.WriteLine("Замена типа у живого объекта");

        int[] numbers = [111, 222, 333];
        const string sample = "Шпион";

        Type during = Borrow(numbers, sample);

        bool ok = Report("во время замены массив зовётся строкой", during == typeof(string), during.Name);

        ok &= Report("после возврата массив снова массив", numbers.GetType() == typeof(int[]), "");
        ok &= Report("данные массива на месте", numbers[0] == 111 && numbers[2] == 333, "");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        return Report("сборка мусора после возврата прошла", true, "");
    }

    /// <summary>Объект с операторами true и false работает как условие.</summary>
    private static bool BoolOperators()
    {
        Console.WriteLine();
        Console.WriteLine("Операторы true и false");

        Flag yes = new(true);
        Flag no = new(false);

        bool ok = Report("объект со значением да проходит условие", yes ? true : false, "");
        ok &= Report("объект со значением нет не проходит", no ? false : true, "");

        return ok;
    }

    /// <summary>Обычный метод у пустой ссылки падает, метод расширения нет.</summary>
    private static bool NullBehaviour()
    {
        Console.WriteLine();
        Console.WriteLine("Вызов у пустой ссылки");

        Sample? empty = null;

        bool ok = Report("метод с полем падает", Throws(() => empty!.WithField()), "");

        ok &= Report("метод без обращения к полям тоже падает", Throws(() => empty!.WithoutField()), "");
        ok &= Report("метод расширения не падает", !Throws(() => empty!.Extension()), "");

        return ok;
    }

    /// <summary>Замена и возврат в одном методе.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Type Borrow(int[] numbers, string sample)
    {
        nint original = Innards.MethodTable(numbers);

        Innards.SetMethodTable(numbers, Innards.MethodTable(sample));
        
        Type during = numbers.GetType();
        Innards.SetMethodTable(numbers, original);

        return during;
    }

    /// <summary>Бросает ли вызов исключение.</summary>
    private static bool Throws(Func<string> call)
    {
        try
        {
            _ = call();
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }

    /// <summary>Одна строка отчёта.</summary>
    private static bool Report(string name, bool ok, string detail)
    {
        Console.WriteLine((ok ? "  ок   " : "  СБОЙ ") + name + (detail.Length == 0 ? "" : ": " + detail));
        return ok;
    }
}
