// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly record struct ValueContainer<T>(T Value) : IValueContainer where T : notnull {
    public bool TryGetAsValue<T1>([NotNullWhen(true)] out T1? output) where T1 : notnull {
        if (Value is T1 castedValue) {
            output = castedValue;
            return true;
        }

        output = default;
        return false;
    }
    
    public Type GetTypeOfValue() => typeof(T);
}
