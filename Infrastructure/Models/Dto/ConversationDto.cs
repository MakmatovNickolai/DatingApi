namespace Infrastructure.Models.Dto;

public class ConversationDto
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public MessageDto? LastMessage { get; set; }
    public ProfileDto? Profile { get; set; }
}