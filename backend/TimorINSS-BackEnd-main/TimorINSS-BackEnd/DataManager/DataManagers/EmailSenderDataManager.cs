using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class EmailSenderDataManager : IEmailSenderDataManager
    {
        public EmailSenderDataManager(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public EmailSettings _emailSettings { get; }

        public Task SendEmailAsync(string email, string subject, string token, string? username = null, bool? isInternal = false)
        {
            try
            {
                Execute(email, subject, token, username, isInternal ?? false).Wait();
                return Task.FromResult(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task SendEmailNotify(string email, string subject, string content)
        {
            try
            {
                ExecuteSendNotify(email, subject, content).Wait();
                return Task.FromResult(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Execute(string email, string subject, string token, string? username = null, bool? isInternal = false)
        {
            var builder = new BodyBuilder();
            string templatePath;
            //Vai buscar o caminho do logotipo de timor-leste
            var logo = AppDomain.CurrentDomain.BaseDirectory + @"EmailTemplates/inss_logo.png";

            string url;
            if (string.IsNullOrEmpty(username))
            {
                //Neste caso é um primeiro acesso, por isso vai buscar o template de primeiro acesso
                templatePath = AppDomain.CurrentDomain.BaseDirectory + @"EmailTemplates/FirstAcess.html";
                url = isInternal == false ? _emailSettings.FirstAcessUrl + token : _emailSettings.InternalFirstAcessUrl + token;
            }
            else
            {
                //Neste caso como contém nome de utilizador vai buscar o template de recuperação de password
                templatePath = AppDomain.CurrentDomain.BaseDirectory + @"EmailTemplates/RecoverPassword.html";
                url = isInternal == false ? _emailSettings.RecoverUrl + token + "/" + username : _emailSettings.InternalRecoverUrl + token + "/" + username;
            }

            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;

                //Inicializa o corpo do email e insere o nome como quem enviou, neste caso "TimorINSS Email Service"
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "TimorINSS Email Service")
                };

                mail.To.Add(new MailAddress(toEmail));
                //Caso haja nas configurações de email no appsettings um email em cc, ele insere e envia também para o cc
                if (!string.IsNullOrEmpty(_emailSettings.CcEmail))
                    mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                using (StreamReader SourceReader = File.OpenText(templatePath))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }
                //{0} : user
                //{1} : recoverPasswordURL
                //{2} : logoURL
                //{3} : expirationTime

                var message = string.Format(builder.HtmlBody,
                        username,
                        url,
                        logo,
                        15
                        );

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //outras opções
                //mail.Attachments.Add(new Attachment(arquivo));

                //Configurações de smtp de email
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Port = _emailSettings.PrimaryPort;
                    smtp.Host = _emailSettings.PrimaryDomain;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ExecuteSendNotify(string email, string subject, string content)
        {
            var builder = new BodyBuilder();
            string templatePath;
            //Vai buscar o caminho do logotipo de timor-leste
            var logo = AppDomain.CurrentDomain.BaseDirectory + @"EmailTemplates/inss_logo.png";

            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;

                //Inicializa o corpo do email e insere o nome como quem enviou, neste caso "TimorINSS Email Service"
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "TimorINSS Email Service")
                };

                mail.To.Add(new MailAddress(toEmail));
                //Caso haja nas configurações de email no appsettings um email em cc, ele insere e envia também para o cc
                if (!string.IsNullOrEmpty(_emailSettings.CcEmail))
                    mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

           

                var message = string.Format(builder.HtmlBody,
                        content,
                        logo,
                        15
                        );

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //outras opções
                //mail.Attachments.Add(new Attachment(arquivo));

                //Configurações de smtp de email
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Port = _emailSettings.PrimaryPort;
                    smtp.Host = _emailSettings.PrimaryDomain;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}