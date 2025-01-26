using Microsoft.AspNetCore.Mvc;
using oversight_steam_webservice.Models;
using oversight_steam_webservice.Services;
using System.Web;

namespace oversight_steam_webservice.Controllers
{
    [ApiController]
    [Route("api/steam")]
    public class SteamAPIController : ControllerBase
    {
        private readonly ILogger<SteamAPIController> _logger;
        private readonly IConfiguration _configuration;
        private SteamAPIService _steamService;


        public SteamAPIController(ILogger<SteamAPIController> logger, IConfiguration configuration, SteamAPIService steamService)
        {
            _logger = logger;
            _configuration = configuration;
            _steamService = steamService;
        }

        [HttpGet(Name = "GetSteamGames")]
        public async Task<IEnumerable<SteamGame>> GetGamesAsync()
        {
            try
            {
                return await _steamService.GetGamesAsync<IEnumerable<SteamGame>>(10);
            }
            catch (Exception ex)
            {
                string message = $"Error getting Steam games: {ex.Message}";
                _logger.LogError(message: message);
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

            string steamId = await _steamService.GetSteamIdFromOpenIdAsync(openid);
            var games = await _steamService.GetOwnedGamesAsync(steamId);

            return Ok(games);
        }
    }
}
