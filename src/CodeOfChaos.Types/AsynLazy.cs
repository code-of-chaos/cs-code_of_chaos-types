// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace System;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class AsyncLazy<T>(Func<CancellationToken, Task<T>> valueFactory) : IAsyncDisposable {
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private Task<T>? _value;
    private bool _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask<T> GetValueAsync(CancellationToken ct = default) {
        ct.ThrowIfCancellationRequested();
        
        
        if (_value is {} value ) return await value.ConfigureAwait(false);
        
        await _semaphore.WaitAsync(ct).ConfigureAwait(false);
        
        try {
            // Check once more within the semaphore lock
            _value ??= valueFactory(ct);
        }
        catch (Exception ex) {
            _value = Task.FromException<T>(ex);
            throw;
        }
        finally {
            _semaphore.Release();
        }

        return await _value.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) return;
        _disposed = true;
        
        // First do all the regular stuff
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
        
        // Value might have not been loaded yet, so dispose accordingly
        if (_value == null) return;
        T data = await _value.ConfigureAwait(false);
        switch (data) {
            case IAsyncDisposable asyncDisposable: await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable: disposable.Dispose();
                break;
        }
        
        _value = null;
    }
}
