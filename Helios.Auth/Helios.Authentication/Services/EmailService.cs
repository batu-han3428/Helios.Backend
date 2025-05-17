using Helios.Authentication.Contexts;
using System.Net.Mail;
using Helios.Authentication.Services.Interfaces;
using System.ComponentModel;
using MassTransit;
using Helios.Authentication.Consumers.ConsumerModels;
using Helios.Common.Model;
using Helios.Common.DTO;

namespace Helios.Authentication.Services
{
    public class EmailService: IEmailService
    {
        private AuthenticationContext _context;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _config;
        private readonly IBus _backgorundWorker;
        public EmailService(AuthenticationContext context, SmtpClient smtpClient, IConfiguration config, IBus _bus)
        {
            _context = context;
            _smtpClient = smtpClient;
            _config = config;
            _backgorundWorker = _bus;
        }

        public async Task SendMail(string mail, string subject, string content)
        {
            var mailMessage = new MailMessage(_config["EmailSender:UserName"], mail, subject, content)
            { IsBodyHtml = true, Sender = new MailAddress(_config["EmailSender:UserName"]) };

            var isSend = _smtpClient.SendMailAsync(mailMessage);
            isSend.Wait();
        }

        public async Task AddStudyUserMail(StudyUserModel studyUserModel)
        {
            await _backgorundWorker.Publish(new AddStudyUserModel
            {
                Name = studyUserModel.Name,
                LastName = studyUserModel.LastName,
                Email = studyUserModel.Email,
                Password = studyUserModel.Password,
                ResearchName = studyUserModel.ResearchName,
                ResearchLanguage = studyUserModel.ResearchLanguage,
                FirstAddition = studyUserModel.FirstAddition
            });
        }

        public async Task SystemAdminUserMail(SystemAdminDTO systemAdminDTO)
        {
            await _backgorundWorker.Publish(systemAdminDTO);
        }
        public async Task CreateRoleUserMail(CreateRoleUserDTO createRoleUserDTO)
        {
            await _backgorundWorker.Publish(new CreateRoleUserDTO
            {
                Name = createRoleUserDTO.Name,
                LastName = createRoleUserDTO.LastName,
                Email = createRoleUserDTO.Email,
                Language = createRoleUserDTO.Language
            });
        }
        public async Task UserResetPasswordMail(StudyUserModel studyUserModel)
        {
            await _backgorundWorker.Publish(new UserResetPasswordModel
            {
                Name = studyUserModel.Name,
                LastName = studyUserModel.LastName,
                Email = studyUserModel.Email,
                Password = studyUserModel.Password,
                ResearchName = studyUserModel.ResearchName,
                ResearchLanguage = studyUserModel.ResearchLanguage
            });
        }

        public async Task ForgotPasswordMail(ForgotPasswordDTO forgotPasswordDTO)
        {
            await _backgorundWorker.Publish(forgotPasswordDTO);
        }
    }
}
