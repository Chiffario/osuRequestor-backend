using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using osuRequestor.Apis.OsuApi.Interfaces;
using osuRequestor.Apis.OsuApi.Models;
using osuRequestor.Configuration;
using osuRequestor.Models;

namespace osuRequestor.Apis.OsuApi;

/// <summary>
/// Stolen entirely off of Relaxation Vault's wrapper until a real one exists
/// </summary>
public class OsuApiProvider : IOsuApiProvider
{
    private const string osu_base = "https://osu.ppy.sh/";
    private const string api_beatmap_scores_link = "api/v2/beatmaps/{0}/scores?mods[]=RX&mods[]={1}&mode=osu";
    private const string api_beatmap_link = "api/v2/beatmaps/{0}";
    private const string api_score_link = "api/v2/scores/{0}";
    private const string api_scores_link = "api/v2/scores?cursor_string={0}";
    private const string api_user_link = "api/v2/users/{0}";
    private const string api_token_link = "oauth/token";
    private const string map_download_link = "osu/{0}";
    private const string api_me_link = "api/v2/me/";
    private const string api_friends_link = "api/v2/friends/";
    private readonly HttpClient _httpClient;
    private readonly OsuApiConfig _config;
    private readonly ILogger<OsuApiProvider> _logger;

    private TokenResponse? _userlessToken;
    private DateTime? _userlessTokenExpiration;

    public OsuApiProvider(IOptions<OsuApiConfig> config, HttpClient httpClient, ILogger<OsuApiProvider> logger)
    {
        _config = config.Value;
        _httpClient = httpClient;
        _logger = logger;

        RefreshUserlessToken().Wait();
    }

    public async Task<BeatmapScores?> GetBeatmapScores(int id, string[] mods)
    {
        await RefreshUserlessToken();

        var modsString = string.Join("&mods[]=", mods);

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_beatmap_scores_link, id, modsString)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken)}
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);

        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BeatmapScores>();
    }

    public async Task<Beatmap?> GetBeatmap(int id)
    {
        await RefreshUserlessToken();

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_beatmap_link, id)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Beatmap>();
    }

    public async Task<BeatmapModel?> GetBeatmapModel(int id)
    {
        await RefreshUserlessToken();

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_beatmap_link, id)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BeatmapModel>();
    }

    public async Task<Score?> GetScore(long id)
    {
        await RefreshUserlessToken();

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_score_link, id)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Score>();
    }

    public async Task<ScoresResponse?> GetScores(long? cursor)
    {
        await RefreshUserlessToken();

        var cursorString = cursor == null ? "" : Convert.ToBase64String(Encoding.Default.GetBytes($"{{\"id\": {cursor}}}"));

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_scores_link, cursorString)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ScoresResponse>();
    }

    public async Task<User?> GetUser(int id)
    {
        await RefreshUserlessToken();

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_user_link, id)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<UserModel?> GetUserModel(int id)
    {
        await RefreshUserlessToken();

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(api_user_link, id)),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _userlessToken!.AccessToken) }
        };
        requestMessage.Headers.Add("x-api-version", "99999999");

        var response = await _httpClient.SendAsync(requestMessage);
        if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound })
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserModel>();
    }

    public async Task<User?> GetSelfUser(string token)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + api_me_link),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };

        var response = await _httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User[]?> GetFriends(string token)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + api_friends_link),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };

        var response = await _httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<User[]>();
    }

    public async Task<bool> DownloadMap(int id, string path)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(osu_base + string.Format(map_download_link, id))
        };

        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        await File.WriteAllBytesAsync(path, await response.Content.ReadAsByteArrayAsync());

        return true;
    }

    public async Task<TokenResponse?> RefreshToken(string refreshToken, string accessToken)
    {
        var config = new RefreshTokenRequest
        {
            ClientId = _config.ClientId,
            ClientSecret = _config.ClientSecret,
            RefreshToken = refreshToken,
            AccessToken = accessToken
        };

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(osu_base + api_token_link),
            Content = new StringContent(JsonSerializer.Serialize(config), null, "application/json"),
            Headers = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") }}
        };

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<TokenResponse>();
    }

    private async Task RefreshUserlessToken()
    {
        if (_userlessTokenExpiration > DateTime.UtcNow)
        {
            return;
        }

        var requestModel = new GetUserlessTokenRequest
        {
            ClientId = _config.ClientId,
            ClientSecret = _config.ClientSecret,
        };

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(osu_base + api_token_link),
            Content = new StringContent(JsonSerializer.Serialize(requestModel), null, "application/json"),
            Headers = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
        };

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Log(LogLevel.Error, "Couldn't update userless token! Status code: {Code}", response.StatusCode);
            return;
        }

        _userlessToken = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (_userlessToken == null)
        {
            _logger.Log(LogLevel.Error, "Couldn't parse userless token! {Json}", await response.Content.ReadAsStringAsync());
            return;
        }

        _userlessTokenExpiration = DateTime.UtcNow.AddSeconds(_userlessToken.ExpiresIn);
    }
}