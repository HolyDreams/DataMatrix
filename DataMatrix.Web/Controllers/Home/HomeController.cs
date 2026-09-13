using DataMatrix.Web.Controllers.Home.Models;
using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace DataMatrix.Web.Controllers.Home
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IApiHttpClient _apiHttpClient;
        private readonly IFileService _fileService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<HomeController> _logger;
        private const int PageSize = 10;

        public HomeController(IApiHttpClient apiHttpClient, IFileService fileService, ISecurityService securityService, ILogger<HomeController> logger)
        {
            _apiHttpClient = apiHttpClient ?? throw new ArgumentNullException(nameof(apiHttpClient));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpGet("~/")]
        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();
            var user = User;
            if (user.Identities.First().IsAuthenticated)
            {
                model.IsAuthorized = true;
                model.UserRoles = [.. User.Claims
                                          .Where(c => c.Type == ClaimTypes.Role)
                                          .Select(c => Roles.ToRole(c.Value).ToString())];
            }
            else
                model.IsAuthorized = false;

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<ActionResult<HomeViewModel>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var authResult = await _securityService.Auth(Request.HttpContext, request.Login, request.Password);
                if (authResult.IsSuccess)
                {
                    var model = new HomeViewModel()
                    {
                        UserRoles = [.. authResult.Roles.Select(role => role.ToString())]
                    };

                    return Ok(model);
                }

                if (authResult.ResponseCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();
                return BadRequest(authResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<HomeViewModel>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var registerResult = await _securityService.Register(Request.HttpContext, request.Login, request.Password, request.Roles);
                if (registerResult.IsSuccess)
                {
                    var model = new HomeViewModel()
                    {
                        UserRoles = [.. registerResult.Roles.Select(role => role.ToString())]
                    };

                    return Ok(model);
                }

                if (registerResult.ResponseCode == System.Net.HttpStatusCode.Unauthorized)
                    return Unauthorized();

                return BadRequest(registerResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _securityService.Logout(Request.HttpContext);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.Viewer)]
        [HttpGet("codes/{page}")]
        public async Task<ActionResult<HomeViewModel>> GetCodes([FromRoute] int page = 1)
        {
            var model = new HomeViewModel()
            {
                IsAuthorized = true
            };

            try
            {
                var codes = await _apiHttpClient.GetAllAsync();
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

        [Authorize(Roles = Roles.Viewer)]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download([FromRoute] int id)
        {
            try
            {
                var file = await _fileService.GetFile(id);
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

        [Authorize(Roles = Roles.Creator)]
        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await _apiHttpClient.CreateRandomAsync();
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
    }
}
