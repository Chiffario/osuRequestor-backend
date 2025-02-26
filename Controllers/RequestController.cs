using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using osuRequestor.Apis.OsuApi.Interfaces;
using osuRequestor.Data;
using osuRequestor.Models;

namespace osuRequestor.Controllers;

[Route("api/requests")]
public class RequestController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOsuApiProvider _osuApiProvider;
    private readonly ILogger<RequestController> _logger;

    public RequestController(DatabaseContext databaseContext, IOsuApiProvider osuApiProvider, ILogger<RequestController> logger)
    {
        _databaseContext = databaseContext;
        _osuApiProvider = osuApiProvider;
        _logger = logger;
    }
    /// <summary>
    /// Returns a list of beatmaps requested to a player
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRequests(int? playerId)
    {
        if (playerId == null) return BadRequest();

        var requests = await _databaseContext
            .Requests
            // Tracking introduces unnecessary overhead for read-only ops
            .AsNoTracking()
            .Include(requestModel => requestModel.Beatmap)
            .Include(requestModel => requestModel.RequestedTo)
            .OrderByDescending(i => i.Id)
            .Take(10).ToListAsync();
        if (requests.Count == 0) return NotFound();
        _logger.LogInformation($"Getting requests for {playerId}");
        return Ok(requests);
    }
    /// <summary>
    /// Creates a request to a player
    /// </summary>
    /// <param name="playerId">ID of a player the request is addressed to</param>
    /// <param name="beatmapId">ID of the beatmap requested</param>
    /// <returns>HTTP 200 on success, HTTP 400 on missing parameters</returns>
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostRequest(int? playerId, int? beatmapId)
    {

        if (playerId == null || beatmapId == null) return BadRequest();

        var player = _databaseContext.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == playerId);
        if (player is null)
        {
            _logger.LogDebug($"Player {playerId} not found, fetching from osu!api");
            var user = await _osuApiProvider.GetUser(playerId ?? 0);
            if (user is null) return BadRequest();
            _logger.LogDebug($"Fetched player {playerId} ({user.Username})");
            var userModel = new UserModel {
                Id = user.Id,
                Username = user.Username
            };
            _databaseContext.Users.Add(userModel);
        }
        else
        {
            _logger.LogDebug($"Found player {playerId}");
        };
        var beatmap = _databaseContext.Beatmaps
            .AsNoTracking()
            .FirstOrDefault(b => b.BeatmapId == beatmapId);
        if (beatmap is null)
        {
            _logger.LogDebug($"Beatmap {beatmapId} not found, fetching from osu!api");
            var apiBeatmap = await _osuApiProvider.GetBeatmap(beatmapId ?? 0);
            if (apiBeatmap is null) return BadRequest();
            _logger.LogDebug($"Fetched beatmap {beatmapId} ({apiBeatmap.BeatmapSet.Artist} - {apiBeatmap.BeatmapSet.Title})");
            var beatmapModel = new BeatmapModel {
                BeatmapId = apiBeatmap.Id,
                BeatmapSetId = apiBeatmap.BeatmapSet.Id,
                Artist = apiBeatmap.BeatmapSet.Artist,
                Title = apiBeatmap.BeatmapSet.Title,
                DifficultyRating = apiBeatmap.StarRating

            };
            _databaseContext.Beatmaps.Add(beatmapModel);
        }
        else
        {
            _logger.LogDebug($"Found beatmap {beatmapId} ");
        }
        await _databaseContext.SaveChangesAsync();
        Debug.Assert(beatmap != null, nameof(beatmap) + " != null");
        Debug.Assert(player != null, nameof(player) + " != null");
        var req = new RequestModel {
            Beatmap = beatmap,
            RequestedFrom = player,
            RequestedTo = player
        };
        _databaseContext.Requests.Add(req);
        await _databaseContext.SaveChangesAsync();

        return Ok(req);
    } 
}