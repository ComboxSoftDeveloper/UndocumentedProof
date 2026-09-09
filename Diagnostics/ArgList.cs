using System.Runtime.InteropServices;
using UndocumentedProof.Types;

namespace UndocumentedProof.Diagnostics;

/// <summary>
/// Четвёртое скрытое ключевое слово — __arglist. Оно объявляет метод
/// с переменным числом аргументов без params и без массива, а на стороне
/// вызова перечисляет их через ArgIterator.
///
/// Компилятор такой код принимает и собирает без единого предупреждения,
/// а вот выполнится он не везде. В target.h джита стоит проверка:
/// соглашение вызова с переменным числом аргументов поддерживается только
/// на Windows и только не на 32-битном Arm. На остальных платформах джит
/// отказывается компилировать такой вызов.
/// </summary>
internal static class ArgList
{
    /// <summary>Выводит, чем кончается вызов такого метода.</summary>
    internal static int Run()
    {
        Console.WriteLine("Рантайм: " + Environment.Version);
        Console.WriteLine("Система: " + RuntimeInformation.OSDescription.Trim()
                          + ", " + RuntimeInformation.ProcessArchitecture);
        Console.WriteLine();
        Console.WriteLine("Метод объявлен так:");
        Console.WriteLine();
        Console.WriteLine("  private static string Join(__arglist)");
        Console.WriteLine();
        Console.WriteLine("Вызов: Join(__arglist(1, \"два\", 3.5))");
        Console.WriteLine();

        try
        {
            string joined = Vararg.Call();
            Console.WriteLine("  вызов прошёл: " + joined);
        }
        catch (Exception error)
        {
            Console.WriteLine("  вызов не прошёл");
            Console.WriteLine("  тип исключения: " + error.GetType().FullName);
            Console.WriteLine("  сообщение:      " + error.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Сборка при этом прошла без ошибок и без предупреждений.");
        Console.WriteLine();
        Console.WriteLine("Соглашение вызова с переменным числом аргументов джит принимает");
        Console.WriteLine("только на Windows и только не на 32-битном Arm. На остальных");
        Console.WriteLine("платформах та же сборка падает с InvalidProgramException.");

        return 0;
    }
}
