using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreStudyController : Controller
    {
        private CoreContext _context;

        public CoreStudyController(CoreContext context)
        {
            _context = context;
        }

        #region Module
        [HttpPost]
        public async Task<bool> AddModule(ModuleModel model)
        {
            _context.Modules.Add(new Module
            {
                AddedById = model.UserId,
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now,
                IsActive = true
            });

            var result = await _context.SaveAuthenticationContextAsync(new Guid(), DateTimeOffset.Now) > 0;

            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateModule(ModuleModel model)
        {
            var module = await _context.Modules.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (module == null)
            {
                return false;
            }

            module.Name = model.Name;
            module.UpdatedAt = DateTimeOffset.Now;
            module.UpdatedById = model.UserId;

            _context.Modules.Update(module);

            return true;
        }
        
        [HttpPost]
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            var module = await _context.Modules.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (module == null)
            {
                return false;
            }

            module.UpdatedAt = DateTimeOffset.Now;
            module.UpdatedById = model.UserId;
            module.IsActive = false;
            module.IsDeleted = true;

            _context.Modules.Update(module);

            return true;
        }

        [HttpGet]
        public async Task<ModuleModel> GetModule(Guid id)
        {
            var model = new ModuleModel();

            var module = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

            if (module != null)
            {
                model.Id = module.Id;
                model.Name = module.Name;
            }

            return model;
        }

        [HttpGet]
        public async Task<List<ModuleModel>> GetModuleList()
        {
            var result = await _context.Modules.Where(x => x.IsActive && !x.IsDeleted).Select(x => new ModuleModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return result;
        }

        #endregion
    }
}
