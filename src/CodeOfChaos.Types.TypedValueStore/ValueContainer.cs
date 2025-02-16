// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly record struct ValueContainer<T>(T Value) : IValueContainer where T : notnull {
    private readonly Lazy<Type> _lazyUnderlyingType = new(() => typeof(T));
    public Type UnderlyingType => _lazyUnderlyingType.Value;

    public bool TryGetAsValue<T1>([NotNullWhen(true)] out T1? output) where T1 : notnull {
        switch (Value) {
            case T1 castedValue:
                output = castedValue;
                return true;
            default:
                output = default;
                return false;
        }
    }

    public T1 GetAsValue<T1>() where T1 : notnull {
        return Value switch {
            T1 castedValue => castedValue,
            _ => throw new InvalidCastException($"Value of type {UnderlyingType} cannot be casted to {typeof(T1)}")
        };
    }

    public object? GetBoxedValue() => Value;
}
