// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IValueContainer {
    Type UnderlyingType { get; }

    bool TryGetAsValue<T>([NotNullWhen(true)] out T? output) where T : notnull;
    T GetAsValue<T>() where T : notnull;

    object? GetBoxedValue();
}
