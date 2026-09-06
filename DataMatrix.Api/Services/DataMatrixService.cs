using DataMatrix.Api.DAL;
using DataMatrix.Api.Generators;
using DataMatrix.Api.Models;
using DataMatrix.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DataMatrix.Api.Services;

/// <summary>
/// Сервис для работы с DataMatrix
/// </summary>
public class DataMatrixService(ApplicationDbContext db)
{
    private const string Chars = "1234567890";
    private const int CodeLength = 20;
    private static readonly Random _random = Random.Shared;

    /// <summary>
    /// Создание кода DataMatrix
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task CreateCodeAsync(int userId, CancellationToken ct = default)
    {
        char[] codeChars = new char[CodeLength];
        for (int i = 0; i < CodeLength; i++)
        {
            codeChars[i] = Chars[_random.Next(Chars.Length)];
        }

        Code code = new()
        {
            UserId = userId,
            Value = new string(codeChars)
        };

        await db.Codes.AddAsync(code, ct);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Получение всех кодов
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<CodeDto>> GetAllCodesAsync(CancellationToken ct = default)
        => await db.Codes.Include(c=>c.User).AsNoTracking().Select(code=>new CodeDto(code)).ToListAsync(ct);

    /// <summary>
    /// Получение кода по идентификатору
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="codeId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<byte[]> GetCodeImageByIdAsync(string userName, int codeId, CancellationToken ct = default)
    {
        if (await db.Codes.AsNoTracking().FirstOrDefaultAsync(i => i.Id == codeId, ct) is { } code)
        {
            return DataMatrixGenerator.GenerateRandom(userName, code.Value);
        }
        return [];
    }
}