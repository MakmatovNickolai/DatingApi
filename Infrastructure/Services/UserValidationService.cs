using Common.Exceptions;
using Common.Services;
using DataAccess.Entities.Identity;
using DataAccess.Repositories;

namespace Infrastructure.Services;

public interface IUserValidatorService
{
    Task<User?> ValidateUser(string email, string password);
    void CheckName(string? name);
    Task<Guid> TryGetGenderIdAsync(string gender);
    Task CheckImagesAsync(Guid[] imageIds);
    Task CheckExistingEmailAsync(string email);
}

public class UserValidatorService : IUserValidatorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    public UserValidatorService(
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public void CheckName(string? name)
    {
        if (string.IsNullOrEmpty(name)) throw new BadRequestException("Name must not be null");
        // todo move to appsettings
        if (name.Length < 2 || name.Length > 8)
            throw new BadRequestException($"Name length must be between {2} and {8} characters");
    }

    public async Task<User?> ValidateUser(string email, string password)
    {
        var searchEmail = email.ToLower();
        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Email == searchEmail);

        if (user != null && _passwordHasher.IsPasswordValid(password, user.PasswordHash!))
        {
            return user;
        }

        return null;
    }

    public async Task<Guid> TryGetGenderIdAsync(string gender)
    {
        var existingGender = await _unitOfWork.GenderRepository.GetAsync(x => x.Name == gender);
        if (existingGender is null) throw new BadRequestException("Wrong gender");
        return existingGender.Id;
    }

    public async Task CheckImagesAsync(Guid[] imageIds)
    {
        var imagesExist = await _unitOfWork.ImageRepository
            .CountAsync(x => imageIds.Contains(x.Id)) == imageIds.Length; // todo what if 0 images?
        if (!imagesExist) throw new BadRequestException("Wrong image ids");
    }

    public async Task CheckExistingEmailAsync(string email)
    {
        var searchEmail = email.ToLower();
        await _unitOfWork.UserRepository.FailIfAnyAsync(x => x.Email == searchEmail);
    }
}
