using System.Runtime.CompilerServices;

namespace UndocumentedProof.Types;

/// <summary>
/// Метод с переменным числом аргументов через скрытое ключевое слово
/// __arglist. Вынесен в отдельный тип, чтобы вызов не попадал в тела отчётов:
/// соглашение вызова проверяется при компиляции того метода, где стоит вызов,
/// и отчёт упал бы целиком, не напечатав ни строки.
/// </summary>
internal static class Vararg
{
    /// <summary>Перечисляет аргументы через ArgIterator.</summary>
    private static string Join(__arglist)
    {
        ArgIterator iterator = new(__arglist);
        string result = string.Empty;

        while (iterator.GetRemainingCount() > 0)
        {
            TypedReference argument = iterator.GetNextArg();
            result += __reftype(argument).Name + " = " + TypedReference.ToObject(argument) + "; ";
        }

        return result;
    }

    /// <summary>Вызов вынесен сюда и не встраивается.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static string Call() => Join(__arglist(1, "два", 3.5));
}
