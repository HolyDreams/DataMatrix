using DataMatrix.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DataMatrix.Api.Controllers;

/// <summary>
/// Контроллер аутентификации
/// </summary>
[ApiController, Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="roleManager"></param>
    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
            return BadRequest("Введите пароль");
        
        User? user = await _userManager.FindByNameAsync(request.Name);
        if (user == null)
            return Unauthorized("Неверный логин или пароль");
        
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded) return Unauthorized("Неверный логин или пароль");

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok(new { message = "Вход выполнен успешно", userId = user.Id });
    }

    /// <summary>
    /// Выход
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Вы вышли из системы" });
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Все поля обязательны для заполнения");
        }
        
        User user = new (){ UserName = request.Name};

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            foreach (string roleName in request.Roles)
            {
                if(await _roleManager.FindByNameAsync(roleName) is {} role)
                    await _userManager.AddToRoleAsync(user, role.Name);
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new { message = "Пользователь успешно создан", userId = user.Id });
        }
        
        IEnumerable<string> errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { errors });
    }
}