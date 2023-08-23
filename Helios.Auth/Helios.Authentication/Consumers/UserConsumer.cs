using MassTransit;
using System.Net.Mail;
using System;
using Helios.Authentication.Models;
using Helios.Authentication.EventBusBase;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Reflection;

namespace Helios.Authentication.Consumers
{
    public class UserConsumer : MailConsumerBase<UserDTO>
    {
        public UserConsumer(IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient)
           : base(environment, config, smtpClient)
        {
        }


        public override System.Threading.Tasks.Task Consume(ConsumeContext<UserDTO> context)
        {
            var model = context.Message;


            string htmlMessage = "heeyy";

            //var imgPath = environment.WebRootFileProvider.GetFileInfo("img/helios_222_70.png")?.PhysicalPath;
            //var imgPath = System.IO.Path.GetDirectoryName("MailPhotos/helios_222_70.png");
            //byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
            //string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            //using (StringReader streamReader = new StringReader(template.TemplateBody))
            //{
            //    htmlMessage = streamReader.ReadToEnd()
            //        .Replace("@To", user.Name)
            //        .Replace("@FileName", model.FileName)
            //        .Replace("@PlaceholderName", model.FileName)
            //        .Replace("@RequestedBy", model.RequestedBy)
            //        .Replace("@RequestedFrom", model.RequestedFrom != null ? string.Join(", ", model.RequestedFrom) : "")
            //        .Replace("@RequestDate", model.RequestDate.ToString())
            //        .Replace("@FileState", model.FileState)
            //        .Replace("@WaitingFileTable", table);
            //}

            string tempPath = "";
            //var lang = UnitOfWork.ResearchContext.Researches.Where(p => p.Id == model.ResearchId).Select(s => s.Language).FirstOrDefault();
            //if (lang != null && lang == "tr")
            //    tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ConsumerMailLayoutTr.html");
            //else
                tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ConsumerMailLayout.html");
            string mailContent = "";

            Attachment attachment = new Attachment(Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.png"));

            string contentID = "test001@host";
            attachment.ContentId = contentID;

            using (StreamReader reader = File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd().Replace("$Body$", htmlMessage)
                    //.Replace("@imgbase64", base64ImageRepresentation)
                    .Replace("@img", "<img src=\"cid:" + contentID + "\">")
                    .Replace("@dynamicdomain", "dasda");
            }

            var mailMessage = new System.Net.Mail.MailMessage(userName, "batu_6407@hotmail.com.tr", "konu", mailContent)
            { IsBodyHtml = true, Sender = new MailAddress(userName) };

            mailMessage.Attachments.Add(attachment);

            var isSend = smtpClient.SendMailAsync(mailMessage);
            isSend.Wait();

            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}
