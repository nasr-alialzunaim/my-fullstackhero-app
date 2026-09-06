using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FSH.Modules.ScientificAnalysis.Engine;

public sealed class GenisScientificEngineClient(
    HttpClient httpClient,
    IOptions<GenisScientificEngineOptions> options)
{
    private readonly GenisScientificEngineOptions _options = options.Value;

    public bool Enabled => _options.Enabled;

    public async Task<GenisEngineVersion> GetValidatedVersionAsync(
        CancellationToken cancellationToken)
    {
        EnsureEnabled();

        using var response = await httpClient.GetAsync(new Uri("/version", UriKind.Relative), cancellationToken)
            .ConfigureAwait(false);
        string body = await response.Content.ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"GENis /version returned HTTP {(int)response.StatusCode}.");
        }

        using JsonDocument document = JsonDocument.Parse(body);
        JsonElement root = document.RootElement;

        string service = root.GetProperty("service").GetString() ?? string.Empty;
        string version = root.GetProperty("version").GetString() ?? string.Empty;
        string commit = root.GetProperty("genisUpstreamCommit").GetString() ?? string.Empty;

        if (!string.Equals(service, _options.ExpectedService, StringComparison.Ordinal) ||
            !string.Equals(version, _options.ExpectedVersion, StringComparison.Ordinal) ||
            !string.Equals(commit, _options.ExpectedUpstreamCommit, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Connected GENis engine provenance does not match the configured validated build.");
        }

        return new GenisEngineVersion(service, version, commit);
    }

    public async Task<GenisEngineResponse> GetAsync(
        string path,
        CancellationToken cancellationToken)
    {
        EnsureEnabled();

        using var response = await httpClient.GetAsync(new Uri(path, UriKind.Relative), cancellationToken)
            .ConfigureAwait(false);
        string body = await response.Content.ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        return new GenisEngineResponse(
            (int)response.StatusCode,
            response.Content.Headers.ContentType?.ToString() ?? "application/json",
            body);
    }

    public async Task<GenisEngineResponse> PostJsonAsync(
        string path,
        string requestJson,
        CancellationToken cancellationToken)
    {
        EnsureEnabled();

        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Content = new StringContent(requestJson, Encoding.UTF8);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await httpClient.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
        string body = await response.Content.ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        return new GenisEngineResponse(
            (int)response.StatusCode,
            response.Content.Headers.ContentType?.ToString() ?? "application/json",
            body);
    }

    private void EnsureEnabled()
    {
        if (!_options.Enabled)
        {
            throw new InvalidOperationException(
                "GENis scientific engine integration is disabled. " +
                "Set GenisScientificEngine:Enabled=true for the validated local engine.");
        }
    }
}