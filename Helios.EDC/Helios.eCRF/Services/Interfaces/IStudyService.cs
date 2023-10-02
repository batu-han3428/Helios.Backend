using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Helios.Common.DTO;
using Helios.Common.Model;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IStudyService
    {
        Task<bool> AddModule(ModuleModel model);
        Task<bool> UpdateModule(ModuleModel model);
        Task<bool> DeleteModule(ModuleModel model);
        Task<ModuleModel> GetModule(Guid id);
        Task<List<ModuleModel>> GetModuleList();
        Task<List<StudyDTO>> GetStudyList();
        Task<StudyDTO> GetStudy(Guid studyId);
        Task<ApiResponse<dynamic>> StudySave(StudyModel studyModel);
    }
}
