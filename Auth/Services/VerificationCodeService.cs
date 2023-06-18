using Auth.Config;
using Common.Services;
using DataAccess.Entities.Identity;
using DataAccess.Repositories;
using Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace Auth.Services;

public interface IVerificationCodeService
{
    Task<string> AddVerificationCode(Guid userId, VerificationCodeType codeType);
    Task<bool> VerifyCode(Guid userId, string attemptedCode, VerificationCodeType codeType);
}
public class VerificationCodeService : IVerificationCodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserValidation _userValidation;
    private readonly EnvOptions _envOptions;
    public VerificationCodeService(
        ICodeGenerator codeGenerator,
        IPasswordHasher passwordHasher,
        IOptions<UserValidation> userValidation,
        IOptions<EnvOptions> envOptions,
        IUnitOfWork unitOfWork)
    {
        _codeGenerator = codeGenerator;
        _passwordHasher = passwordHasher;
        _userValidation = userValidation.Value;
        _envOptions = envOptions.Value;
        _unitOfWork = unitOfWork;
    }

    private async Task InvalidateCode(Guid userId, VerificationCodeType codeType)
    {
        await _unitOfWork.VerificationCodeRepository.DeleteAsync(x => x.UserId == userId && x.Type == codeType);
    }

    public async Task<string> AddVerificationCode(Guid userId, VerificationCodeType codeType)
    {
        await InvalidateCode(userId, codeType);

        var newCode = GetCode();

        await _unitOfWork.VerificationCodeRepository.InsertAsync(new VerificationCode
        {
            ExpiryDate = DateTime.UtcNow.AddSeconds(_userValidation.CodeLifetime),
            Token = _passwordHasher.HashPassword(newCode),
            Type = codeType,
            UserId = userId,
        });

        return newCode;
    }

    public async Task<bool> VerifyCode(Guid userId, string attemptedCode, VerificationCodeType codeType)
    {
        var codeItem = await _unitOfWork.VerificationCodeRepository.GetAsync(x => x.UserId == userId && x.Type == codeType);
        if (codeItem == null) return false;
        if (codeItem.ExpiryDate <= DateTime.UtcNow)
        {
            await InvalidateCode(userId, codeType);
            return false;
        }

        return _passwordHasher.IsPasswordValid(attemptedCode, codeItem.Token);
    }

    private string GetCode()
    {
        return _codeGenerator.Generate(useDigits: true, useLowerCase: true, useUppercase: true, useNonAlphanumeric: false, _userValidation.VerificationCodeLength);
    }
}
