﻿using MassTransit;
using System.Net.Mail;
using System;
using Helios.Authentication.Models;
using Helios.Authentication.EventBusBase;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Reflection;
using Helios.Authentication.Consumers.ConsumerModels;

namespace Helios.Authentication.Consumers
{
    public class AddStudyUserConsumer : MailConsumerBase<AddStudyUserModel>
    {
        public AddStudyUserConsumer(IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient)
           : base(environment, config, smtpClient)
        {
        }

        public override Task Consume(ConsumeContext<AddStudyUserModel> context)
        {
            var model = context.Message;

            string tempPath = "";
            string mailSubject = "";

            if (model.FirstAddition)
            {
                if (model.ResearchLanguage == 2)
                {
                    tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ConfirmationMail_TR.html");
                    mailSubject = model.ResearchName + " çalışması için  eCRF hesabınız oluşturuldu";
                }
                else
                {
                    tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/ConfirmationMail.html");
                    mailSubject = "Your " + model.ResearchName + "eCRF account is created";
                }
            }
            else
            {
                if (model.ResearchLanguage == 2)
                {
                    tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AddStudyUserMail_TR.html");
                    mailSubject = model.ResearchName + " çalışması için  eCRF hesabınız oluşturuldu";
                }
                else
                {
                    tempPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailTemplates/AddStudyUserMail.html");
                    mailSubject = "Your " + model.ResearchName + "eCRF account is created";
                }
            }

            
            string mailContent = "";

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"MailPhotos/helios_222_70.jpg");
            byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);


            using (StreamReader reader = System.IO.File.OpenText(tempPath))
            {
                mailContent = reader.ReadToEnd()
                    .Replace("@FullName", model.Name + " " + model.LastName)
                    .Replace("@UserName", model.Email)
                    .Replace("@Password", model.Password)
                    .Replace("@ResearchName", model.ResearchName)
                    .Replace("@ContactLink", "https://localhost:44458/ContactUs")
                    .Replace("@StudyWebLink", "https://localhost:44458/Login")
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
