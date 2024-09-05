using MassTransit;
using System.Net.Mail;
using Helios.Authentication.EventBusBase;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Helios.Common.DTO;

namespace Helios.Authentication.Consumers
{
    public class CreateRoleUserConsumer : MailConsumerBase<CreateRoleUserDTO>
    {
        public CreateRoleUserConsumer(IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient)
           : base(environment, config, smtpClient)
        {
        }

        public override Task Consume(ConsumeContext<CreateRoleUserDTO> context)
        {
            var model = context.Message;

            string tempPath = "";
            string mailSubject = "";

            if (model.Language == "tr")
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/CreateRoleUserMail_TR.html");
                mailSubject = "eCRF hesabınız oluşturuldu";
            }
            else
            {
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/CreateRoleUserMail.html");
                mailSubject = "your eCRF account is created";
            }

            string mailContent = "";

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.jpg");
            byte[] imageArray = File.ReadAllBytes(imgPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);


            using (StreamReader reader = File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd()
                    .Replace("@FullName", model.Name + " " + model.LastName)
                    .Replace("@StudyWebLink", "https://localhost:44458/Login")
                    .Replace("@ContactLink", "https://localhost:44458/ContactUs")
                    .Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@dynamicdomain", "https://localhost:44458/");
            }

            var mailMessage = new MailMessage(userName, model.Email, mailSubject, mailContent)
            { IsBodyHtml = true, Sender = new MailAddress(userName) };

            var isSend = smtpClient.SendMailAsync(mailMessage);
            isSend.Wait();

            return Task.FromResult(0);
        }
    }
}
