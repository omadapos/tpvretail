namespace OmadaPOS.Infrastructure;

/// <summary>
/// DelegatingHandler that intercepts HTTP 401 Unauthorized responses and
/// notifies the UI via <see cref="SessionExpiredNotifier"/> so it can
/// force re-login without requiring every service to handle this case individually.
/// </summary>
public sealed class TokenExpiryHandler : DelegatingHandler
{
    public TokenExpiryHandler() : base(new HttpClientHandler()) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            SessionExpiredNotifier.Raise();

        return response;
    }
}
