using MassTransit;
using System.Net.Mail;
using Helios.Authentication.EventBusBase;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Text.Encodings.Web;
using Helios.Common.DTO;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Helios.Authentication.Domains.Entities;

namespace Helios.Authentication.Consumers
{
    public class ForgotPasswordConsumer : MailConsumerBase<ForgotPasswordDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ForgotPasswordConsumer(IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient, UserManager<ApplicationUser> userManager)
           : base(environment, config, smtpClient)
        {
            _userManager = userManager;
        }

        public override async Task Consume(ConsumeContext<ForgotPasswordDTO> context)
        {
            var model = context.Message;

            string tempPath = "";
            string mailSubject = "";

            if (model.Language == "tr")
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AdminForgotPasswordMail_TR.html");
                mailSubject = "e-CRF şifre sıfırlama";
            }
            else
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AdminForgotPasswordMail.html");
                mailSubject = "e-CRF reset password";
            }

            string mailContent = "";

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.jpg");
            byte[] imageArray = File.ReadAllBytes(imgPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            string callbackUrl = $"https://localhost:44458/reset-password/code={HttpUtility.UrlEncode(model.Code)}/username={model.Mail}";

            using (StreamReader reader = File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd()
                    .Replace("@FullName", model.Name + " " + model.LastName)
                    .Replace("@ContactLink", "https://localhost:44458/ContactUs")
                    .Replace("@PasswordLink", HtmlEncoder.Default.Encode(callbackUrl))
                    .Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@dynamicdomain", "https://localhost:44458/");
            }

            var mailMessage = new MailMessage(userName, model.Mail, mailSubject, mailContent)
            { IsBodyHtml = true, Sender = new MailAddress(userName) };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                //isSend.Wait();

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.Mail);

                if (user != null)
                {
                    user.IsResetPasswordMailSent = true;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {

            }
            
        }
    }
}