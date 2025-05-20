namespace SIG_PSPEP.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"Enviar email para: {email}");
        Console.WriteLine($"Assunto: {subject}");
        Console.WriteLine($"Mensagem: {htmlMessage}");
        return Task.CompletedTask;
    }
}
