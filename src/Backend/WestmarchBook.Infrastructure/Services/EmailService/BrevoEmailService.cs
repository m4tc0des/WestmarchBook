using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using WestmarchBook.Domain.Services.EmailService;

namespace WestmarchBook.Infrastructure.Services.EmailService;

internal class BrevoEmailService : ISendEmailService
{
    private readonly IConfiguration _configuration;

    public BrevoEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordRecoveryCode(string toEmail, string userName, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["EmailSettings:FromName"],
            _configuration["EmailSettings:FromEmail"]!));
        message.To.Add(new MailboxAddress(userName, toEmail));
        message.Subject = "Filarmonica 30 de Junho - Código de Recuperação de Senha";

        message.Body = new TextPart("html")
        {
            Text = $"""
               <h2>Recuperação de Senha</h2>
               <p>Olá, <strong>{userName}</strong>.</p>
               <p>Use o código abaixo para redefinir sua senha:</p>
               <h1 style="letter-spacing: 4px;">{code}</h1>
               <p>Este código é válido por <strong>10 minutos</strong>. Se você não solicitou essa alteração, ignore este e-mail.</p>
               """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["EmailSettings:Host"]!,
            int.Parse(_configuration["EmailSettings:Port"]!),
            SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"]!,
            _configuration["EmailSettings:Password"]!);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}

