using System.Runtime.CompilerServices;

namespace UndocumentedProof.Types;

/// <summary>
/// Чтение и запись указателя на таблицу методов. Указатель записан в начале
/// объекта, сразу после заголовка для блока синхронизации: два служебных поля
/// по 8 байт.
///
/// Запись сюда меняет тип живого объекта. После такой замены сборщик мусора
/// прочитает его поля по описанию чужого типа, поэтому исходный указатель
/// возвращается на место сразу же, в том же методе.
/// </summary>
internal static class Innards
{
    /// <summary>Адрес объекта в куче.</summary>
    internal static unsafe nint Address(object value) => *(nint*)Unsafe.AsPointer(ref value);

    /// <summary>Текущий указатель на таблицу методов.</summary>
    internal static unsafe nint MethodTable(object value) => *(nint*)Address(value);

    /// <summary>
    /// Записывает чужой указатель на таблицу методов. Вызывать только парой
    /// с возвратом прежнего значения: между заменой и возвратом нельзя
    /// вызывать сборку мусора и нельзя выходить из метода.
    /// </summary>
    internal static unsafe void SetMethodTable(object value, nint table)
    {
        *(nint*)Address(value) = table;
    }
}
