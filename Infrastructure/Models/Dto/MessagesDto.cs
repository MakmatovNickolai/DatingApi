namespace Infrastructure.Models.Dto;

public class MessagesDto
{
    public List<MessageDto> Messages { get; set; }
    public int LastMessageId { get; set; }
    public string LastMessageCreatedOn { get; set; }
}