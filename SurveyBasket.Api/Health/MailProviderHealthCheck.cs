using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Core.Settings;
using MailKit.Net.Smtp;

namespace SurveyBasket.Api.Health;

public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
		try
		{
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, cancellationToken);

            return await Task.FromResult(HealthCheckResult.Healthy());
        }
		catch (Exception exception)
		{
            return await Task.FromResult(HealthCheckResult.Unhealthy(exception : exception));
        }
    }
}
