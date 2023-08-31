using Helios.eCRF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Helios.eCRF.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddTenant(TenantModel model);
        Task<bool> UpdateTenant(TenantModel model);
        Task<TenantModel> GetTenant(Guid id);
        Task<UserDTO> GetUserByEmail(string mail);
        Task<bool> AddUser(UserDTO model);
        Task<bool> PassiveOrActiveUser(UserDTO model);
        Task<bool> UpdateUser(UserDTO model);
        Task<bool> AddRole(UserDTO model);
        Task<List<TenantModel>> GetTenantList();
    }
}
