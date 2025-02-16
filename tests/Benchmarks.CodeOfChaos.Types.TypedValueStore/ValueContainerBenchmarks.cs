// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using BenchmarkDotNet.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Benchmarks.CodeOfChaos.Types.TypedValueStore;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MemoryDiagnoser]
public class ValueContainerBenchmarks {
    private NewValueContainer<int> _newContainer;
    private OldValueContainer<int> _oldContainer;

    [Params(42)]// Parametrize different values for testing
    public int Value { get; set; }

    [GlobalSetup]
    public void Setup() {
        _oldContainer = new OldValueContainer<int>(Value);
        _newContainer = new NewValueContainer<int>(Value);
    }

    [Benchmark]
    public bool OldValueContainer_TryGetAsValue_Int() => _oldContainer.TryGetAsValue(out int _);// Benchmarks OldValueContainer TryGetAsValue

    [Benchmark]
    public bool NewValueContainer_TryGetAsValue_Int() => _newContainer.TryGetAsValue(out int _);// Benchmarks NewValueContainer TryGetAsValue

    [Benchmark]
    public int OldValueContainer_TryGetAsValue_Int_Value() {
        _oldContainer.TryGetAsValue(out int output);// Benchmarks NewValueContainer TryGetAsValue
        return output;
    }

    [Benchmark]
    public int NewValueContainer_TryGetAsValue_Int_Value() {
        _newContainer.TryGetAsValue(out int output);// Benchmarks NewValueContainer TryGetAsValue
        return output;
    }

    [Benchmark]
    public Type OldValueContainer_GetTypeOfValue() => _oldContainer.GetTypeOfValue();// Benchmarks type retrieval for OldValueContainer

    [Benchmark]
    public Type NewValueContainer_GetUnderlyingType() => _newContainer.UnderlyingType;// Benchmarks type retrieval for NewValueContainer
}

public readonly record struct NewValueContainer<T>(T Value) {
    private readonly Lazy<Type> _lazyUnderlyingType = new(() => typeof(T));
    public Type UnderlyingType => _lazyUnderlyingType.Value;

    public bool TryGetAsValue<T1>([NotNullWhen(true)] out T1? output) where T1 : notnull {
        switch (Value) {
            case T1 castedValue: return (output = castedValue) is not null;
            default: return (output = default) is not null;
        }
    }
}

public readonly record struct OldValueContainer<T>(T Value) {
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
