﻿using Helios.Common.DTO;
using Helios.Common.Model;
using RestSharp;

namespace Helios.Core.Services.Interfaces
{
    public interface IStudyService
    {
        Task<ApiResponse<dynamic>> SetSubjectDetailMenu(Int64 studyId, List<SubjectDetailMenuModel> detailMenuModels);
        Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 studyId);
    }
}
