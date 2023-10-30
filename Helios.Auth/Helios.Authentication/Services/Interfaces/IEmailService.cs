using Helios.Common.Model;

namespace Helios.Authentication.Services.Interfaces
{
    public interface IEmailService
    {
        Task AddStudyUserMail(StudyUserModel studyUserModel);
        Task UserResetPasswordMail(StudyUserModel studyUserModel);
    }
}
