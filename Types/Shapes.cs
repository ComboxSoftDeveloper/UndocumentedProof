// Поля этих типов нужны только затем, чтобы к ним обращались проверки.
// Компилятор об этом не знает и предупреждает, что значения не меняются.
#pragma warning disable CS0649

namespace UndocumentedProof.Types;

/// <summary>
/// Класс с перегруженными операторами true и false. После такой перегрузки
/// объект можно поставить прямо в условие, хотя он не bool.
/// </summary>
internal sealed class Flag
{
    private readonly bool _value;

    internal Flag(bool value) => _value = value;

    /// <summary>Условие считается истинным, когда поле истинно.</summary>
    public static bool operator true(Flag flag) => flag._value;

    /// <summary>Условие считается ложным, когда поле ложно.</summary>
    public static bool operator false(Flag flag) => !flag._value;

    /// <summary>
    /// Оператор И. Вместе с операторами true и false он даёт короткий &&:
    /// компилятор сначала спрашивает у левого операнда, ложный ли он, и только
    /// потом вычисляет правый. Ради этого пара true и false и существует.
    /// </summary>
    public static Flag operator &(Flag left, Flag right) => new(left._value && right._value);

    /// <summary>Оператор ИЛИ, он же даёт короткий ||.</summary>
    public static Flag operator |(Flag left, Flag right) => new(left._value || right._value);

    /// <summary>Значение поля. Читается отчётами.</summary>
    internal bool Value => _value;
}

/// <summary>Обычный класс: на нём проверяется вызов метода у пустой ссылки.</summary>
internal sealed class Sample
{
    internal string Name = "Иван";

    /// <summary>Метод обращается к полю, поэтому без объекта работать не может.</summary>
    internal string WithField() => Name;

    /// <summary>Метод к полям не обращается. Проверка в том, падает ли он.</summary>
    internal string WithoutField() => "выполнился";
}

/// <summary>Метод расширения для того же класса.</summary>
internal static class SampleExtensions
{
    /// <summary>Для компилятора это статический вызов с аргументом.</summary>
    internal static string Extension(this Sample sample) => sample is null ? "выполнился, аргумент пустой" : "выполнился";
}
