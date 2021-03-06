using System.Diagnostics.CodeAnalysis;

namespace Server
{
    public struct Option<T> where T : notnull
    {
        public static Option<T> None => default;
        public static Option<T> Some(T value) => new Option<T>(value);

        readonly bool isSome;
        readonly T value;

        Option(T value)
        {
            this.value = value;
            isSome = this.value is { };
        }

        public bool IsSome([MaybeNullWhen(false)]out T value)
        {
            value = this.value;
            return isSome;
        }
    }
}