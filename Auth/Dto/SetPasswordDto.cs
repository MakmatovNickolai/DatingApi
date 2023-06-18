namespace Auth.Dto;

public record SetPasswordDto(
    Guid UserId, 
    string Password
);

