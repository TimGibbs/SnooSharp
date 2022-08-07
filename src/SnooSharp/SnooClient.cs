using System.Net.Http.Json;

namespace SnooSharp;
public class SnooClient : ISnooClient
{
    private AccessToken? _accessToken;
    private readonly HttpClient _httpClient;
    private readonly SnooClientOptions _options;

    public static SemaphoreSlim _lock = new(1);
    public SnooClient(HttpClient client, SnooClientOptions options)
    {
        _options = options;
        _httpClient = client;
        _httpClient.BaseAddress = new Uri(Constants.UrlParts.BaseUrl);
    }
    
    private async Task SetAccessToken()
    {
        await _lock.WaitAsync();
        try
        {
            if (_accessToken is null || _accessToken.ValidTill < DateTime.Now)
            {
                _accessToken = await GetToken();
            }
        }
        finally
        {
            _lock.Release();
        }

    }

    private async Task<AccessToken> GetToken()
    {
        var payload = new AuthPayload(_options.Username,_options.Password);
        var request = new HttpRequestMessage(HttpMethod.Post, Constants.UrlParts.LoginEndpoint)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add(Constants.Headers.UserAgent, Constants.HeaderValues.UserAgent);
        
        var response = await _httpClient.SendAsync(request, new CancellationToken());

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
        
        var tokenResponse =  await response.Content.ReadFromJsonAsync<TokenResponse>();
        return new AccessToken(tokenResponse!);
    }

    public async Task<DataResponse> GetData(DateTime date)
    {
        await SetAccessToken();
        var url = Constants.UrlParts.DataEndpoint + $"?startTime={date:MM/dd/yyyy hh:mm:ss}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add(Constants.Headers.UserAgent, Constants.HeaderValues.UserAgent);
        request.Headers.Add(Constants.Headers.Auth, "Bearer "+_accessToken!.Token);
        
        var response = await _httpClient.SendAsync(request, new CancellationToken());
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
        
        return (await response.Content.ReadFromJsonAsync<DataResponse>())!;
    }
    
    public async Task<IEnumerable<DataResponse>> GetDataResponses(DateTime start, DateTime end)
    {
        var days = start.EachDayUntil(end);
        return await Task.WhenAll(days.Select(GetData));
    }
}