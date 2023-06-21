using NotificationTemplate.Services;
using NotificationTemplate.Templates.Models;

namespace Infrastructure.Models;

public class EmailMessage
{
    public string Subject { get; set; }
    public string To { get; set; }
    public TemplateType Template { get; set; }
    public VerificationModel TemplateModel { get; set; }
}