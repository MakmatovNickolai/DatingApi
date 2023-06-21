using Hangfire;
using Infrastructure.Config;
using Infrastructure.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationTemplate.Services;

namespace Infrastructure.Services;

public interface INotificationService
{
    void QueueEmailSendingTask(EmailMessage message);
}

internal class StagingNotificationService : INotificationService
{
    private readonly ITemplateService _templateService;
    private readonly NotificationOptions _notificationOptions;
    private readonly ILogger<StagingNotificationService> _logger;

    public StagingNotificationService(ITemplateService templateService,
        IOptions<NotificationOptions> notificationOptions,
        ILogger<StagingNotificationService> logger)
    {
        _logger = logger;
        _templateService = templateService;
        _notificationOptions = notificationOptions.Value;
    }

    public void QueueEmailSendingTask(EmailMessage emailMessage)
    {
        // todo should the delivery status be tracked in some way?
        BackgroundJob.Enqueue(() => SendEmailAsync(emailMessage));
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_notificationOptions.Username);
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;
        _logger.LogInformation($"{message.To}");
        var builder = new BodyBuilder
        {
            HtmlBody = await _templateService.GetMessage(message.Template, message.TemplateModel)
        };
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        _logger.LogInformation($"SMTP Host ${_notificationOptions.Server}. Sending to ${message.To}");
        await smtp.ConnectAsync(_notificationOptions.Server, _notificationOptions.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_notificationOptions.Username, _notificationOptions.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}

internal class ProductionNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public ProductionNotificationService(ILogger logger)
    {
        _logger = logger;
    }

    public void QueueEmailSendingTask(EmailMessage message)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(EmailMessage message)
    {
        throw new NotImplementedException();
    }
}