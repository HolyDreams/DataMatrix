using DataMatrix.Web.Controllers.Home.Models;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Metadata.Ecma335;

namespace DataMatrix.Web.Controllers.Home
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IApiHttpClient _apiHttpClient;
        private readonly IFileService _fileService;
        private readonly ILogger<HomeController> _logger;
        private const string CookieName = "AuthToken";
        private const int PageSize = 10;

        public HomeController(IApiHttpClient apiHttpClient, IFileService fileService, ILogger<HomeController> logger)
        {
            _apiHttpClient = apiHttpClient ?? throw new ArgumentNullException(nameof(apiHttpClient));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet]
        [HttpGet("~/")]
        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();
            var cookie = Request.Cookies[CookieName];
            if (!string.IsNullOrWhiteSpace(cookie))
                model.IsAuthorized = true;
            else
                model.IsAuthorized = false;

            return View(model);
        }

        [HttpPost("auth")]
        public async Task<ActionResult<HomeViewModel>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var authResult = await _apiHttpClient.AuthAsync(request.Login, request.Password);
                var expires = GetExpires(authResult);
                Response.Cookies.Append(CookieName, authResult, new CookieOptions() { Expires = expires });
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<HomeViewModel>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var authResult = await _apiHttpClient.RegisterAsync(request.Login, request.Password, [.. request.Roles]);
                var expires = GetExpires(authResult);
                Response.Cookies.Append(CookieName, authResult, new CookieOptions() { Expires = expires });
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var model = new HomeViewModel();
            var cookie = Request.Cookies[CookieName];

            if (string.IsNullOrWhiteSpace(cookie))
                return Ok(model);

            try
            {
                await _apiHttpClient.LogoutAsync(cookie);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Response.Cookies.Delete(CookieName);
                    return Ok(model);
                }
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            Response.Cookies.Delete(CookieName);
            return Ok(model);
        }

        [HttpGet("codes/{page}")]
        public async Task<ActionResult<HomeViewModel>> GetCodes([FromRoute] int page = 1)
        {
            var cookie = Request.Cookies[CookieName];
            var expires = GetExpires(cookie);
            if (expires is null || DateTime.Now > expires)
                return Unauthorized("relogin");

            var model = new HomeViewModel();
            model.IsAuthorized = true;
            try
            {
                var codes = await _apiHttpClient.GetAllAsync(cookie);
                model.IsAuthorized = true;
                model.CurrentPage = page;
                model.PageSize = PageSize;
                model.TotalPages = codes.Count / PageSize + (codes.Count % PageSize > 0 ? 1 : 0);
                model.Codes = [.. codes.Skip((page - 1) * PageSize).Take(PageSize)];
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return StatusCode(403);
                }
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    model.IsAuthorized = false;
                    Response.Cookies.Delete(CookieName);
                }
                else
                {
                    _logger.LogError(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }

            return model;
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download([FromRoute] int id)
        {
            var cookie = Request.Cookies[CookieName];
            var expires = GetExpires(cookie);
            if (expires is null || DateTime.Now > expires)
                return Unauthorized("relogin");

            try
            {
                var file = await _fileService.GetFile(id, cookie!);
                if (file is null)
                    return NotFound();
                return File(file.Content, "image/png", "barcode.png");
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    return StatusCode(403);
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            var cookie = Request.Cookies[CookieName];
            var expires = GetExpires(cookie);
            if (expires is null || DateTime.Now > expires)
                return Unauthorized("relogin");

            try
            {
                await _apiHttpClient.CreateRandomAsync(cookie);
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        private DateTime? GetExpires(string? cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie))
                return null;
            var time = cookie.Split(";").First(s => s.Contains("expires"));
            return DateTime.TryParse(time.Substring(time.IndexOf(",") + 2), out var date) ? date : null;
        }
    }
}
