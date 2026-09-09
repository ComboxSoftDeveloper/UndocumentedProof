using System.Text;
using UndocumentedProof.Diagnostics;

namespace UndocumentedProof;

/// <summary>
/// Точка входа. Замеров в этом проекте нет: всё, что он проверяет, либо
/// работает, либо нет, и числа тут ничего не добавят. Каждый режим выводит
/// свой отчёт, а батник перенаправляет вывод в файл.
/// </summary>
internal static class Program
{
    /// <summary>Разбор аргументов и запуск.</summary>
    private static int Main(string[] args)
    {
        // Консоль на разных машинах пишет по-разному, а отчёты уезжают
        // в репозиторий одним набором.
        Console.OutputEncoding = Encoding.UTF8;

        string mode = args.Length > 0 ? args[0] : "checks";

        return mode switch
        {
            "checks" => Checks.Run(),
            "keywords" => Keywords.Run(),
            "arglist" => ArgList.Run(),
            "swap" => MethodTableSwap.Run(),
            "boolops" => BoolOps.Run(),
            "nullcall" => NullCall.Run(),
            _ => Unknown(mode),
        };
    }

    /// <summary>Неизвестный режим: список того, что есть.</summary>
    private static int Unknown(string mode)
    {
        Console.WriteLine("Неизвестный режим: " + mode);
        Console.WriteLine("Есть: checks, keywords, arglist, swap, boolops, nullcall");

        return 1;
    }
}
