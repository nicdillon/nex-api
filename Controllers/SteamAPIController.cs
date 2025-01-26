using Microsoft.AspNetCore.Mvc;
using NexAPI.Models;
using NexAPI.Services;
using System.Web;

namespace NexAPI.Controllers
{
    [ApiController]
    [Route("api/steam")]
    public class SteamAPIController(ILogger<SteamAPIController> logger, IConfiguration configuration, SteamAPIService steamService) : ControllerBase
    {
        private readonly ILogger<SteamAPIController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly SteamAPIService _steamService = steamService;

        [HttpGet(Name = "GetSteamGames")]
        public async Task<IEnumerable<SteamGame>> GetGamesAsync()
        {
            try
            {
                return await _steamService.GetGamesAsync<IEnumerable<SteamGame>>(10);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error getting Steam games: {ex.Message}";
                _logger.LogError(message: errorMessage);
                return [];
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            string returnUrl = "https://host.docker.internal:8080.com/api/steam/auth/callback"; // Replace with your callback URL
            string steamOpenIdUrl = $"https://steamcommunity.com/openid/login?" +
                                    $"openid.ns=http://specs.openid.net/auth/2.0&" +
                                    $"openid.mode=checkid_setup&" +
                                    $"openid.return_to={HttpUtility.UrlEncode(returnUrl)}&" +
                                    $"openid.realm=https://yourwebsite.com&" +
                                    $"openid.identity=http://specs.openid.net/auth/2.0/identifier_select&" +
                                    $"openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select";

            return Redirect(steamOpenIdUrl);
        }

        [HttpGet("auth/callback")]
        public async Task<IActionResult> AuthCallback([FromQuery] string openid)
        {
            if (!await _steamService.VerifyOpenIdAsync(openid))
            {
                return BadRequest("Invalid Steam OpenID");
            }

            string steamId = _steamService.GetSteamIdFromOpenIdAsync(openid);
            var games = await _steamService.GetOwnedGamesAsync(steamId);

            return Ok(games);
        }
    }
}
