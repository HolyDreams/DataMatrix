using DataMatrix.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DataMatrix.Api.Models;

namespace DataMatrix.Api.Controllers;

/// <summary>
/// Контроллер для работы с формированием и просмотром Data Matrix
/// </summary>
[ApiController, Route("[controller]/[action]")]
public class DataMatrixController(DataMatrixService dataMatrixService, UserManager<User> userManager) : ControllerBase
{
    /// <summary>
    /// Создание и сохранение 20 значного кода
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Creator")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GenerateRandomAsync(CancellationToken ct = default)
    {
        if (int.TryParse(userManager.GetUserId(User), out int userId))
        {
            await dataMatrixService.CreateCodeAsync(userId, ct);
        }
        else
        {
            return BadRequest("Ошибка получения идентификатора пользователя");
        }

        return Ok();
    }

    /// <summary>
    /// Просмотр генерированных кодов
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Viewer")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllCodesAsync(CancellationToken ct = default)
        => Ok(await dataMatrixService.GetAllCodesAsync(ct));
    
    /// <summary>
    /// Получение кода в Data Matrix
    /// </summary>
    /// <param name="id">Идентификатор кода</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Viewer")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PrintCodeImageByIdAsync([FromRoute(Name = "id")] int id, CancellationToken ct = default)
    {
        if (await dataMatrixService.GetCodeImageByIdAsync(userManager.GetUserName(User) ?? "Без имени", id, ct) is { Length: > 0 } bytes)
        {
            return File(bytes, "image/png", "barcode.png");
        }

        return BadRequest("Указанный код не найден");
    }
}