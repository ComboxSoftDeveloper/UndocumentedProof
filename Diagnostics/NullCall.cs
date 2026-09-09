using UndocumentedProof.Types;

using System.Runtime.InteropServices;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Вызов метода у пустой ссылки. Ходит утверждение, что обычный метод класса
/// вызовется без ошибки, если внутри он не обращается к полям, — потому что
/// компилятору якобы нечего разыменовывать.
///
/// Отчёт проверяет это на трёх случаях: метод с обращением к полю, метод без
/// обращения и метод расширения.
/// </summary>
internal static class NullCall
{
    /// <summary>Выводит, что происходит в каждом случае.</summary>
    internal static int Run()
    {
        Console.WriteLine("Рантайм: " + Environment.Version);
        Console.WriteLine("Система: " + RuntimeInformation.OSDescription.Trim()
                          + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine();

        Sample? empty = null;

        Console.WriteLine("  ссылка пустая: " + (empty is null));
        Console.WriteLine();

        Report("метод обращается к полю", () => empty!.WithField());
        Report("метод к полям не обращается", () => empty!.WithoutField());
        Report("метод расширения", () => empty!.Extension());

        Console.WriteLine();
        Console.WriteLine("Компилятор C# на вызов метода у ссылочного типа выдаёт");
        Console.WriteLine("инструкцию callvirt даже для невиртуального метода — ради");
        Console.WriteLine("проверки на пустую ссылку. Обращение к полям тут ни при чём.");
        Console.WriteLine();
        Console.WriteLine("Метод расширения работает, потому что это не вызов на объекте:");
        Console.WriteLine("компилятор превращает его в статический вызов с аргументом.");

        return 0;
    }

    /// <summary>Одна строка отчёта: результат или тип исключения.</summary>
    private static void Report(string name, Func<string> call)
    {
        string outcome;
        try
        {
            outcome = call();
        }
        catch (Exception error)
        {
            outcome = error.GetType().Name;
        }

        Console.WriteLine("  " + name.PadRight(30) + outcome);
    }
}
