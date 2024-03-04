using AutoMapper;
using Helios.Common.Domains.Core.Entities;
using Helios.Common.DTO;

namespace Helios.Common.Helpers.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ModuleDTO, StudyVisitPageModule>();
            CreateMap<ElementDTO, StudyVisitPageModuleElement>();
            CreateMap<CalculatationElementDetailDTO, StudyVisitPageModuleCalculationElementDetails>();
            CreateMap<ModuleElementEventDTO, StudyVisitPageModuleElementEvent>();
            CreateMap<ElementDetailDTO, StudyVisitPageModuleElementDetail>();
            CreateMap<StudyRoleModulePermissionDTO, StudyRoleModulePermission>();
        }
    }
}
