using AutoMapper;
using Common.Exceptions;
using DataAccess.Repositories;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IUserService
{
    Task DeleteAsync(Guid userId);
    Task UpdateUserDetailsAsync(Guid userId, UserDetailUpdateRequest data);
    Task<UserDto> GetDetailsAsync(Guid profileId);
    Task UpdateLocationAsync(Guid profileId, UpdateLocationRequest data);
    Task UpdateSearchSettingsAsync(Guid profileId, UpdateSearchSettingsRequest request);
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

    public async Task UpdateUserDetailsAsync(Guid profileId, UserDetailUpdateRequest data)
    {
        var profile = await _unitOfWork.ProfileRepository.GetAsync(profileId);
        if (profile == null)
        {
            throw new BadRequestException("No such profile");
        }
        await _userValidator.CheckImagesAsync(data.ImageIds);
        await _unitOfWork.ProfileRepository.UpdateAsync(profileId, (u, ctx) =>
        {
            u.Entity.Name = data.Name;
            u.Entity.BirthDate = DateOnly.Parse(data.DateOfBirth);
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

    public async Task<UserDto> GetDetailsAsync(Guid userId)
    {
        var user = await _unitOfWork.UserRepository
            .GetAll()
            .Include(u => u.Profile)
                .ThenInclude(p => p.Gender)
            .Include(u => u.Profile)
                .ThenInclude(p => p.Images)
            .FirstAsync(u => u.Id == userId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateLocationAsync(Guid profileId, UpdateLocationRequest data)
    {
        await _unitOfWork.ProfileRepository.UpdateAsync(profileId, (u, ctx) =>
        {
            u.Entity.Latitude = data.Latitude;
            u.Entity.Longitude = data.Longitude;
        });
    }

    public async Task UpdateSearchSettingsAsync(Guid userId, UpdateSearchSettingsRequest request)
    {
        await _unitOfWork.UserRepository.UpdateAsync(userId, (u, ctx) =>
        {
            u.Entity.FilterAgeMax = request.AgeMax;
            u.Entity.FilterAgeMin = request.AgeMin;
            u.Entity.FilterDistance = request.Distance;
        });
    }
}