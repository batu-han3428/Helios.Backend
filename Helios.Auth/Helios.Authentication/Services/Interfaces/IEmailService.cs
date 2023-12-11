﻿using Helios.Common.DTO;
using Helios.Common.Model;

namespace Helios.Authentication.Services.Interfaces
{
    public interface IEmailService
    {
        Task AddStudyUserMail(StudyUserModel studyUserModel);
        Task SystemAdminUserMail(SystemAdminDTO systemAdminDTO);
        Task UserResetPasswordMail(StudyUserModel studyUserModel);
        Task ForgotPasswordMail(ForgotPasswordDTO forgotPasswordDTO);
    }
}
