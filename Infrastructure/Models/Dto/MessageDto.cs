namespace Infrastructure.Models.Dto;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid SenderId { get; set; }
    public DateTime CreatedOn { get; set; }
}