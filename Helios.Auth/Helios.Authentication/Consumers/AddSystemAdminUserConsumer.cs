using MassTransit;
using System.Net.Mail;
using Helios.Authentication.EventBusBase;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Helios.Common.DTO;

namespace Helios.Authentication.Consumers
{
    public class AddSystemAdminUserConsumer : MailConsumerBase<SystemAdminDTO>
    {
        public AddSystemAdminUserConsumer(IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient)
           : base(environment, config, smtpClient)
        {
        }

        public override Task Consume(ConsumeContext<SystemAdminDTO> context)
        {
            var model = context.Message;

            string tempPath = "";
            string mailSubject = "";

            if (model.Language == "tr")
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AddSystemAdminUserMail_TR.html");
                mailSubject = (model.isAddUser ?? true) ? "eCRF hesabınız oluşturuldu" : "Şifre sıfırlama";
            }
            else
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AddSystemAdminUserMail.html");
                mailSubject = (model.isAddUser ?? true) ? "your eCRF account is created": "Reset password";
            }


            string mailContent = "";

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.jpg");
            byte[] imageArray = File.ReadAllBytes(imgPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);


            using (StreamReader reader = File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd()
                    .Replace("@FullName", model.Name + " " + model.LastName)
                    .Replace("@Email", model.Email)
                    .Replace("@Password", model.Password)
                    .Replace("@ContactLink", "https://localhost:44458/ContactUs")
                    .Replace("@Link", "https://localhost:44458/Login")
                    .Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@dynamicdomain", "https://localhost:44458/");
            }

            var mailMessage = new System.Net.Mail.MailMessage(userName, model.Email, mailSubject, mailContent)
            { IsBodyHtml = true, Sender = new MailAddress(userName) };

            var isSend = smtpClient.SendMailAsync(mailMessage);
            isSend.Wait();

            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}
