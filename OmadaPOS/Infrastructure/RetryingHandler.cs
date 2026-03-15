namespace OmadaPOS.Infrastructure;

/// <summary>
/// DelegatingHandler that automatically retries transient HTTP failures.
///
/// Retryable conditions:
///   - Network-level exceptions (<see cref="HttpRequestException"/>, <see cref="TaskCanceledException"/> for timeout)
///   - 5xx server errors (500, 502, 503, 504)
///
/// Non-retryable (returned immediately):
///   - 4xx client errors (including 401 — handled by <see cref="TokenExpiryHandler"/>)
///   - Any other 2xx / 3xx success or redirect
///
/// Backoff: 500 ms → 1 000 ms → give up (2 retries, 3 total attempts).
/// </summary>
public sealed class RetryingHandler : DelegatingHandler
{
    private const int    MaxRetries   = 2;
    private static readonly int[] BackoffMs = [500, 1_000];

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                // Retry only on 5xx server errors; all 4xx are returned as-is
                if (IsServerError(response.StatusCode) && attempt < MaxRetries)
                {
                    await Delay(attempt, cancellationToken).ConfigureAwait(false);
                    attempt++;

                    // Must clone the request because HttpRequestMessage is not reusable
                    request = await CloneAsync(request).ConfigureAwait(false);
                    continue;
                }

                return response;
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                if (attempt >= MaxRetries)
                {
                    // All retries exhausted — notify the UI of connectivity loss.
                    OfflineNotifier.Raise();
                    throw;
                }

                await Delay(attempt, cancellationToken).ConfigureAwait(false);
                attempt++;

                request = await CloneAsync(request).ConfigureAwait(false);
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsServerError(System.Net.HttpStatusCode code)
    {
        int c = (int)code;
        return c is 500 or 502 or 503 or 504;
    }

    private static bool IsTransient(Exception ex) =>
        ex is HttpRequestException ||
        ex is TaskCanceledException tce && !tce.CancellationToken.IsCancellationRequested;

    private static Task Delay(int attempt, CancellationToken ct) =>
        Task.Delay(BackoffMs[Math.Clamp(attempt, 0, BackoffMs.Length - 1)], ct);

    /// <summary>
    /// Clones an <see cref="HttpRequestMessage"/> so it can be re-sent.
    /// Content is buffered in memory the first time; subsequent retries read the buffer.
    /// </summary>
    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content != null)
        {
            var ms   = new MemoryStream();
            await original.Content.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var (k, v) in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(k, v);
        }

        foreach (var (k, v) in original.Headers)
            clone.Headers.TryAddWithoutValidation(k, v);

        return clone;
    }
}
