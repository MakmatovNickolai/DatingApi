using Auth.Config;
using Auth.Dto;
using Auth.Request;
using Auth.Response;
using Common.Services;
using DataAccess.Entities;
using DataAccess.Entities.Identity;
using DataAccess.Repositories;
using Google.Apis.Auth;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationTemplate.Templates.Models;

namespace Auth.Services;

public interface IRegisterService
{
    Task<RegisterResponse> RegisterEmailAsync(RegisterEmailRequest request);
    Task<RegisterResponse> RegisterGoogleAsync(RegisterGoogleRequest request);
    Task SetPasswordAsync(SetPasswordDto setPasswordData);
    Task RepeatCodeAsync(UserEmailRequest userEmailData);
    Task<bool> ConfirmUserAsync(ConfirmUserRequest data);
    Task<bool> CheckEmailInUseAsync(string email);
    Task<bool> CheckEmailVerificationAsync(string email);
    Task<CheckGoogleUserResponse> CheckGoogleUserAsync(GoogleUserRequest data);
}
public class RegisterService : IRegisterService
{

    private readonly IUserValidatorService _userValidatorService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly LinkOptions _linkOptions;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        IUserValidatorService userValidatorService,
        IVerificationCodeService verificationCodeService,
        IPasswordHasher passwordHasher,
        INotificationService notificationService,
        IOptions<LinkOptions> linkOptions,
        IUnitOfWork unitOfWork,
        ILogger<RegisterService> logger)
    {

        _userValidatorService = userValidatorService;
        _passwordHasher = passwordHasher;
        _verificationCodeService = verificationCodeService;
        _notificationService = notificationService;
        _linkOptions = linkOptions.Value;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterEmailAsync(RegisterEmailRequest request)
    {
        await _userValidatorService.CheckExistingEmailAsync(request.Email);
        await _userValidatorService.CheckImagesAsync(request.ImageIds);
        var genderId = await _userValidatorService.TryGetGenderIdAsync(request.Gender);

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = await _unitOfWork.UserRepository.InsertAsync(new User
        {
            Email = request.Email!,
            PasswordHash = passwordHash,
            IsConfirmed = false,
            IsGoogleUser = false,
            DeviceUniqueId = request.DeviceUniqueId,
            DeviceInfo = request.DeviceInfo,
            DeviceLocale = request.DeviceLocale,
            AppVersion = request.AppVersion,
            Platform = request.Platform,
            IsAppleUser = false,
            IsFacebookUser = false,
            DeviceGmtOffset = request.DeviceGmtOffset,
            IsRootedDevice = request.IsDeviceRooted,
            IsEmulator = request.IsDeviceEmulator
        });

        var profile = await _unitOfWork.ProfileRepository.InsertAsync(new Profile()
        {
            Name = request.Name,
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            Description = request.Description,
            GenderId = genderId,
            UserId = user.Id
        });

        foreach (var imageId in request.ImageIds)
        {
            await _unitOfWork.ImageRepository.UpdateAsync(imageId, (i, ctx) =>
            {
                i.Entity.ProfileId = profile.Id;
            });
        }

        var registerCode =
            await _verificationCodeService.AddVerificationCode(user.Id, VerificationCodeType.EmailConfirm);

        _logger.LogInformation($"Data: {_linkOptions.GetConfirmUrl(user.Id, registerCode)}");
        _notificationService.QueueEmailSendingTask(new EmailMessage
        {
            Subject = "Email verification",
            Template = NotificationTemplate.Services.TemplateType.EmailConfirmation,
            TemplateModel = new VerificationModel
            {
                BaseUrl = _linkOptions.FrontHost,
                Token = registerCode,
                UserId = user.Id.ToString()
            },
            To = request.Email!
        });
        return new RegisterResponse
        {
            Success = true,
            Message = "User registered successfully",
            UserId = user.Id
        };
    }

    public async Task<RegisterResponse> RegisterGoogleAsync(RegisterGoogleRequest request)
    {
        await _userValidatorService.CheckExistingEmailAsync(request.Email);
        await _userValidatorService.CheckImagesAsync(request.ImageIds);
        var genderId = await _userValidatorService.TryGetGenderIdAsync(request.Gender);

        var payload = await GoogleJsonWebSignature
            .ValidateAsync(request.GoogleToken, new GoogleJsonWebSignature.ValidationSettings());

        var user = await _unitOfWork.UserRepository.InsertAsync(new User
        {
            Email = payload.Email!,
            IsConfirmed = false,
            IsGoogleUser = true,
            DeviceUniqueId = request.DeviceUniqueId,
            DeviceInfo = request.DeviceInfo,
            DeviceLocale = request.DeviceLocale,
            AppVersion = request.AppVersion,
            Platform = request.Platform,
            IsAppleUser = false,
            IsFacebookUser = false,
            DeviceGmtOffset = request.DeviceGmtOffset,
            IsRootedDevice = request.IsDeviceRooted,
            IsEmulator = request.IsDeviceEmulator
        });

        var profile = await _unitOfWork.ProfileRepository.InsertAsync(new Profile()
        {
            Name = request.Name,
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            Description = request.Description,
            GenderId = genderId,
            UserId = user.Id
        });

        foreach (var imageId in request.ImageIds)
        {
            await _unitOfWork.ImageRepository.UpdateAsync(imageId, (i, ctx) =>
            {
                i.Entity.ProfileId = profile.Id;
            });
        }
        return new RegisterResponse
        {
            Success = true,
            Message = "User registered successfully",
            UserId = user.Id
        };
    }

    public async Task SetPasswordAsync(SetPasswordDto setPasswordData)
    {
        var passwordHash = _passwordHasher.HashPassword(setPasswordData.Password);

        await _unitOfWork.UserRepository.UpdateAsync(setPasswordData.UserId, (u, ctx) =>
        {
            u.Entity.PasswordHash = passwordHash;
        });
    }

    public async Task<bool> ConfirmUserAsync(ConfirmUserRequest data)
    {
        var isConfirmed = await _verificationCodeService
            .VerifyCode(data.UserId, data.Code, VerificationCodeType.EmailConfirm);

        if (!isConfirmed)
        {
            return false;
        }

        await _unitOfWork.UserRepository.UpdateAsync(data.UserId, (x, ctx) =>
        {
            x.Entity.IsConfirmed = true;
        });

        return true;
    }

    public async Task<bool> CheckEmailInUseAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        return await _unitOfWork.UserRepository.CountAsync(x => x.Email == email) > 0;
    }

    public async Task<bool> CheckEmailVerificationAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        return await _unitOfWork.UserRepository.CountAsync(x => x.Email == email && x.IsConfirmed) > 0;
    }

    public async Task<CheckGoogleUserResponse> CheckGoogleUserAsync(GoogleUserRequest data)
    {
        var googleIdIsInUse = await _unitOfWork.UserRepository
            .CountAsync(x => x.GoogleId == data.GoogleId) > 0;
        var googleEmailIsInUse = await _unitOfWork.UserRepository
            .CountAsync(x => x.Email == data.GoogleEmail) > 0;
        return new CheckGoogleUserResponse
        {
            EmailExists = googleEmailIsInUse,
            IdExists = googleIdIsInUse,
            Success = true
        };
    }

    public async Task RepeatCodeAsync(UserEmailRequest userEmailData)
    {
        var user = await _unitOfWork.UserRepository
            .GetAsync(x => x.Email.ToLower() == userEmailData.Email.ToLower());
        if (user == null)
        {
            return;
        }

        var registerCode = await _verificationCodeService
            .AddVerificationCode(user.Id, VerificationCodeType.EmailConfirm);

        _notificationService.QueueEmailSendingTask(new EmailMessage
        {
            Subject = "Email verification",
            Template = NotificationTemplate.Services.TemplateType.EmailConfirmation,
            TemplateModel = new VerificationModel
            {
                BaseUrl = _linkOptions.FrontHost,
                Token = registerCode,
                UserId = user.Id.ToString()
            },
            To = user.Email
        });
    }
}
