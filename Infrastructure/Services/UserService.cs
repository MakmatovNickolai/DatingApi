using AutoMapper;
using Common.Exceptions;
using DataAccess.Repositories;
using Infrastructure.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IUserService
{
    Task DeleteAsync(Guid userId);
    Task UpdateLocationAsync(Guid profileId, UpdateLocationDto data);
    Task UpdateUserDetailsAsync(Guid userId, UserDetailUpdateRequest data);
    Task<ProfileDto> GetProfileDetailsAsync(Guid profileId);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserValidatorService _userValidator;

    public UserService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserValidatorService userValidator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userValidator = userValidator;
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _unitOfWork.UserRepository.DeleteAsync(userId);
    }

    public async Task UpdateLocationAsync(Guid profileId, UpdateLocationDto data)
    {
        await _unitOfWork.ProfileRepository.UpdateAsync(profileId, (u, ctx) =>
        {
            u.Entity.Latitude = data.Latitude;
            u.Entity.Longitude = data.Longitude;
        });
    }

    public async Task UpdateUserDetailsAsync(Guid userId, UserDetailUpdateRequest data)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(userId);
        if (user == null)
        {
            throw new BadRequestException("No such user");
        }
        var profile = await _unitOfWork.ProfileRepository.GetAsync(x => x.UserId == userId);
        if (profile == null)
        {
            throw new BadRequestException("No such profile");
        }
        await _userValidator.CheckImagesAsync(data.ImageIds);
        await _unitOfWork.ProfileRepository.UpdateAsync(userId, (u, ctx) =>
        {
            u.Entity.Name = data.Name;
            u.Entity.BirthDate = DateOnly.FromDateTime(data.BirthDate);
            u.Entity.Description = data.Description;
        });
        foreach (var imageId in data.ImageIds)
        {
            await _unitOfWork.ImageRepository.UpdateAsync(imageId, (u, ctx) =>
            {
                u.Entity.ProfileId = profile.Id;
            });
        }
    }

    public async Task<ProfileDto> GetProfileDetailsAsync(Guid profileId)
    {
        var profile = await _unitOfWork.ProfileRepository
            .GetAll(x => x.Gender, x => x.Images).FirstOrDefaultAsync(x => x.Id == profileId);
        if (profile == null)
        {
            throw new BadRequestException("No such profile");
        }
        return _mapper.Map<ProfileDto>(profile);
    }
}
