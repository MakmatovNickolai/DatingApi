namespace Infrastructure.Models.Dto;

public class ConversationsDto
{
    public List<ConversationDto> Conversations { get; set; }
    public int TotalCount { get; set; }
}