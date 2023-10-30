using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.Enums;
using Helios.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreModuleController : Controller
    {
        private CoreContext _context;

        public CoreModuleController(CoreContext context)
        {
            _context = context;
        }

        #region Module
        [HttpPost]
        public async Task<bool> AddModule(ModuleModel model)
        {
            _context.Modules.Add(new Domains.Entities.Module
            {
                AddedById = model.UserId,
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now,
                IsActive = true
            });

            var result = await _context.SaveCoreContextAsync(new Guid(), DateTimeOffset.Now) > 0;

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
            var result = await _context.SaveCoreContextAsync(new Guid(), DateTimeOffset.Now) > 0;

            return result;
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

        [HttpGet]
        public async Task<List<ElementModel>> GetModuleElements(Guid moduleId)
        {
            var result = await _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.ElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id.ToString(),
                    Title = x.Title,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                }).OrderBy(x => x.Order).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<ElementModel> GetElementData(Guid id)
        {
            var result = await _context.Elements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
                .Include(x => x.ElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id.ToString(),
                    Title = x.Title,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Description = x.Description,
                    IsRequired = x.IsRequired,
                    IsHidden = x.IsHidden,
                    CanMissing = x.CanMissing,
                    Unit = x.ElementDetail.Unit,
                    LowerLimit = x.ElementDetail.LowerLimit,
                    UpperLimit = x.ElementDetail.UpperLimit
                }).FirstOrDefaultAsync();

            return result;
        }

        [HttpPost]
        public async Task<bool> SaveModuleContent(ElementModel model)
        {
            var id = new Guid();
            var userId = model.UserId != "" ? Guid.Parse(model.UserId) : new Guid();
            var moduleId = Guid.Parse(model.ModuleId);

            if (!string.IsNullOrEmpty(model.Id))
                id = Guid.Parse(model.Id);

            var element = await _context.Elements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element == null)
            {
                var moduleElementMaxOrder = _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted).Select(x => x.Order).Max();

                var elm = new Element()
                {
                    Title = model.Title,
                    ElementName = model.ElementName,
                    Description = model.Description,
                    CanMissing = model.CanMissing,
                    ElementType = model.ElementType,
                    IsDependent = model.IsDependent,
                    IsHidden = model.IsHidden,
                    IsReadonly = model.IsReadonly,
                    IsTitleHidden = model.IsTitleHidden,
                    Width = model.Width,
                    IsRequired = model.IsRequired,
                    ModuleId = moduleId,
                    Order = moduleElementMaxOrder + 1,
                    CreatedAt = DateTimeOffset.Now,
                    AddedById = userId,
                };

                _context.Elements.Add(elm);
                var result = await _context.SaveCoreContextAsync(userId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    var elementDetail = new ElementDetail()
                    {
                        ElementId = elm.Id,
                        Unit = model.Unit,
                        LowerLimit = model.LowerLimit,
                        UpperLimit = model.UpperLimit,
                        CreatedAt = DateTimeOffset.Now,
                        AddedById = userId,
                        //ButtonText = model.buttonText
                    };

                    _context.ElementDetails.Add(elementDetail);
                    result = await _context.SaveCoreContextAsync(userId, DateTimeOffset.Now) > 0;
                }
            }
            else
            {
                element.Title = model.Title;
                element.ElementName = model.ElementName;
                element.Description = model.Description;
                element.CanMissing = model.CanMissing;
                element.ElementType = model.ElementType;
                element.IsDependent = model.IsDependent;
                element.IsHidden = model.IsHidden;
                element.IsReadonly = model.IsReadonly;
                element.IsTitleHidden = model.IsTitleHidden;
                element.Width = model.Width;
                element.IsRequired = model.IsRequired;
                element.ModuleId = Guid.Parse(model.ModuleId);
                element.UpdatedAt = DateTimeOffset.Now;
                element.UpdatedById = userId;

                _context.Update(element);

                var elementDetail = await _context.ElementDetails.FirstOrDefaultAsync(x => x.ElementId == element.Id && x.IsActive && !x.IsDeleted);

                elementDetail.Unit = model.Unit;
                elementDetail.LowerLimit = model.LowerLimit;
                elementDetail.UpperLimit = model.UpperLimit;
                element.UpdatedAt = DateTimeOffset.Now;
                element.UpdatedById = userId;

                _context.Update(elementDetail);
                var result = await _context.SaveCoreContextAsync(userId, DateTimeOffset.Now) > 0;
            }

            //module.Name = model.Name;
            //module.UpdatedAt = DateTimeOffset.Now;
            //module.UpdatedById = model.UserId;

            //_context.Modules.Update(module);

            return true;
        }
                
    }
}
