using Auth.Config;
using Auth.Dto;
using Common.Exceptions;
using Common.Services;
using DataAccess.Entities.Identity;
using DataAccess.Repositories;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using NotificationTemplate.Templates.Models;

namespace Auth.Services;

public interface IRestorePasswordService
{
    Task ForgotPasswordAsync(UserEmailDto data);
    Task SetNewPasswordAsync(SetNewPasswordDto data);
}

internal class RestorePasswordService : IRestorePasswordService
{
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IUserValidatorService _userValidatorService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly LinkOptions _linkOptions;

    public RestorePasswordService(
        IVerificationCodeService verificationCodeService,
        INotificationService notificationService,
        IUserValidatorService userValidatorService,
        IPasswordHasher passwordHasher,
        IOptions<LinkOptions> linkOptions,
        IUnitOfWork unitOfWork)
    {
        _verificationCodeService = verificationCodeService;
        _notificationService = notificationService;
        _userValidatorService = userValidatorService;
        _passwordHasher = passwordHasher;
        _linkOptions = linkOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task ForgotPasswordAsync(UserEmailDto data)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Email.ToLower() == data.Email.ToLower());
        if (user == null)
        {
            // do not throw an error in order to exclude brute force check
            return;
        }

        var code = await _verificationCodeService.AddVerificationCode(user.Id, VerificationCodeType.RestorePassword);

        _notificationService.QueueEmailSendingTask(new EmailMessage
        {
            Subject = "Restore password",
            Template = NotificationTemplate.Services.TemplateType.RestorePassword,
            TemplateModel = new VerificationModel
            {
                BaseUrl = _linkOptions.FrontHost,
                Token = code,
                UserId = user.Id.ToString()
            },
            To = user.Email
        });
    }

    public async Task SetNewPasswordAsync(SetNewPasswordDto data)
    {
        // _userValidatorService.CheckPassword(data.NewPassword);

        var isValid = await _verificationCodeService.VerifyCode(data.UserId, data.Code, VerificationCodeType.RestorePassword);
        if (!isValid)
        {
            throw new BadRequestException("Code is not valid");
        }

        var passwordHash = _passwordHasher.HashPassword(data.NewPassword);
        await _unitOfWork.UserRepository.UpdateAsync(data.UserId, (u, ctx) =>
        {
            u.Entity.PasswordHash = passwordHash;
        });
    }
}
