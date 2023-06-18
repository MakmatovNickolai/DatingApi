using NotificationTemplate.Templates.Models;
using Razor.Templating.Core;

namespace NotificationTemplate.Services;

public interface ITemplateService
{
    Task<string> GetMessage(TemplateType template, VerificationModel model);
}

public class TemplateService : ITemplateService
{
    public async Task<string> GetMessage(TemplateType template, VerificationModel model)
    {
        string viewName = string.Empty;
        switch (template)
        {
            case TemplateType.EmailConfirmation:
                viewName = "~/Templates/Views/EmailConfirmation.cshtml";
                break;
            case TemplateType.RestorePassword:
                viewName = "~/Templates/Views/PasswordRestore.cshtml";
                break;
        }

        return await RazorTemplateEngine.RenderAsync(viewName, model);
    }
}

public enum TemplateType
{
    EmailConfirmation,
    RestorePassword
}
