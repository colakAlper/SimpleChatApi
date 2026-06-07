using CB.Application.Abstractions;

namespace CB.IntegrationTests.Fakes;

/// <summary>
/// Fake Ollama client that also counts calls.
/// </summary>
public sealed class CountingOllamaClient : IOllamaClient
{
    private int _callCount;

    /// <summary>
    /// Number of times AskAsync was called.
    /// </summary>
    public int CallCount => _callCount;

    /// <summary>
    /// The test model is always ready.
    /// </summary>
    public Task<bool> IsModelReadyAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(true);
    }

    /// <summary>
    /// Returns a predictable answer for tests.
    /// </summary>
    public async Task<string> AskAsync(string message, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _callCount);

        // A tiny delay makes cache behavior easier to observe in tests.
        await Task.Delay(20, cancellationToken);

        return $"Test cevabı: {message}";
    }
}
