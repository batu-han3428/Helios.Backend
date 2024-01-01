using Helios.Common.DTO;
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
                TenantId = model.TenantId,
                AddedById = model.UserId,
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now,
                IsActive = true
            });

            var result = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

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
            var result = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

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
            var result = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

            return result;
        }

        [HttpGet]
        public async Task<ModuleModel> GetModule(Int64 id)
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
        public async Task<List<ElementModel>> GetModuleElements(Int64 moduleId)
        {
            var result = await _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.ElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    ElementOptions = x.ElementDetail.ElementOptions,
                    Width = x.Width
                }).OrderBy(x => x.Order).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<ElementModel> GetElementData(Int64 id)
        {
            var result = new ElementModel();

            result = await _context.Elements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
               .Include(x => x.ElementDetail)
               .Select(x => new ElementModel()
               {
                   Id = x.Id,
                   Title = x.Title,
                   ElementName = x.ElementName,
                   ElementType = x.ElementType,
                   Description = x.Description,
                   IsRequired = x.IsRequired,
                   IsHidden = x.IsHidden,
                   CanMissing = x.CanMissing,
                   Width = x.Width,
                   Unit = x.ElementDetail.Unit,
                   Mask = x.ElementDetail.Mask,
                   LowerLimit = x.ElementDetail.LowerLimit,
                   UpperLimit = x.ElementDetail.UpperLimit,
                   Layout = x.ElementDetail.Layout,
                   IsDependent = x.IsDependent,
                   IsRelated = x.IsRelated,
                   ElementOptions = x.ElementDetail.ElementOptions,
                   DefaultValue = x.ElementDetail.DefaultValue,
                   AddTodayDate = x.ElementDetail.AddTodayDate
               }).FirstOrDefaultAsync();

            if (result.IsDependent)
            {
                var dep = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == id && x.IsActive && !x.IsDeleted);

                if (dep != null)
                {
                    result.DependentSourceFieldId = dep.SourceElementId;
                    result.DependentTargetFieldId = dep.TargetElementId;
                    result.DependentCondition = (int)dep.ValueCondition;
                    result.DependentAction = (int)dep.ActionType;
                    result.DependentFieldValue = dep.ActionValue;
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SaveModuleContent(ElementModel model)
        {
            var result = new ApiResponse<dynamic>();

            var element = await _context.Elements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element == null)
            {
                var moduleElements = _context.Elements.Where(x => x.ModuleId == model.ModuleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

                if (!checkElementName(model.ModuleId, model.ElementName, moduleElements).Result)
                {
                    result.IsSuccess = false;
                    result.Message = "Duplicate element name";

                    return result;
                }

                var moduleElementMaxOrder = moduleElements.Count() > 0 ? moduleElements.Select(x => x.Order).Max() : 1;

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
                    ModuleId = model.ModuleId,
                    TenantId = model.TenantId,
                    Order = moduleElementMaxOrder + 1,
                    //CreatedAt = DateTimeOffset.Now,
                    //AddedById = userId,
                };

                _context.Elements.Add(elm);
                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (result.IsSuccess)
                {
                    var elementDetail = new ElementDetail()
                    {
                        ElementId = elm.Id,
                        TenantId = model.TenantId,
                        Unit = model.Unit,
                        Mask = model.Mask,
                        LowerLimit = model.LowerLimit,
                        UpperLimit = model.UpperLimit,
                        Layout = model.Layout,
                        ElementOptions = model.ElementOptions,
                        MetaDataTags = model.ElementName,
                        DefaultValue = model.DefaultValue,
                        AddTodayDate = model.AddTodayDate,
                        //CreatedAt = DateTimeOffset.Now,
                        //AddedById = userId,
                        //ButtonText = model.buttonText
                    };

                    _context.ElementDetails.Add(elementDetail);
                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    elm.ElementDetailId = elementDetail.Id;
                    _context.Elements.Update(elm);

                    if (model.IsDependent)
                    {
                        var elementEvent = new ModuleElementEvent()
                        {
                            SourceElementId = model.DependentTargetFieldId,
                            TargetElementId = elm.Id,
                            TenantId = model.TenantId,
                            ValueCondition = (ActionCondition)model.DependentCondition,
                            ActionType = (ActionType)model.DependentAction,
                            ActionValue = model.DependentFieldValue,
                        };

                        _context.ModuleElementEvents.Add(elementEvent);
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    //control for both of element and elementDetail saved
                    var elmDtl = await _context.ElementDetails.FirstOrDefaultAsync(x => x.ElementId == elm.Id && x.IsActive && !x.IsDeleted);
                    if (elmDtl == null)
                    {
                        elm.IsActive = false;
                        elm.IsDeleted = true;
                        _context.Elements.Update(elm);
                        var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        result.IsSuccess = false;
                        result.Message = "Operation failed. Please try again.";
                    }
                    else
                        result.Message = result.IsSuccess ? "Successful" : "Error";
                }
                else
                {
                    result.Message = "Error";
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
                element.ModuleId = model.ModuleId;
                element.UpdatedAt = DateTimeOffset.Now;
                element.UpdatedById = model.UserId;

                _context.Update(element);

                var elementDetail = await _context.ElementDetails.FirstOrDefaultAsync(x => x.ElementId == element.Id && x.IsActive && !x.IsDeleted);

                elementDetail.Unit = model.Unit;
                elementDetail.Mask = model.Mask;
                elementDetail.LowerLimit = model.LowerLimit;
                elementDetail.UpperLimit = model.UpperLimit;
                elementDetail.Layout = model.Layout;
                elementDetail.ElementOptions = model.ElementOptions;
                elementDetail.DefaultValue = model.DefaultValue;
                elementDetail.AddTodayDate = model.AddTodayDate;
                element.UpdatedAt = DateTimeOffset.Now;
                element.UpdatedById = model.UserId;

                _context.Update(elementDetail);

                if (model.IsDependent)
                {
                    var dep = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted);

                    if (dep == null)
                    {
                        var elementEvent = new ModuleElementEvent()
                        {
                            SourceElementId = model.DependentSourceFieldId,
                            TargetElementId = element.Id,
                            ValueCondition = (ActionCondition)model.DependentCondition,
                            ActionType = (ActionType)model.DependentAction,
                            ActionValue = model.DependentFieldValue,
                            ModuleId = element.ModuleId
                        };

                        _context.ModuleElementEvents.Add(elementEvent);
                    }
                    else
                    {
                        dep.SourceElementId = model.DependentSourceFieldId;
                        dep.TargetElementId = element.Id;
                        dep.ValueCondition = (ActionCondition)model.DependentCondition;
                        dep.ActionType = (ActionType)model.DependentAction;
                        dep.ActionValue = model.DependentFieldValue;
                        dep.ModuleId = element.ModuleId;

                        _context.ModuleElementEvents.Update(dep);
                    }
                }

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> CopyElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();

            var element = await _context.Elements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element != null)
            {
                var name = element.ElementName + "_1";

                for (; ; )
                {
                    if (checkElementName(element.ModuleId, name).Result)
                        break;
                    else
                        name = name + "_1";
                }

                element.Id = 0;
                element.ElementName = name;
                element.Order = (element.Order + 1);

                var elementDetail = await _context.ElementDetails.Where(x => x.ElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                elementDetail.Id = 0;

                _context.Add(element);
                _context.Add(elementDetail);

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                elementDetail.ElementId = element.Id;
                element.ElementDetailId = elementDetail.Id;

                _context.Update(element);
                _context.Update(elementDetail);

                var moduleElements = await _context.Elements.Where(x => x.ModuleId == element.ModuleId && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in moduleElements)
                {
                    if (item.Order >= element.Order && item.Id != element.Id)
                    {
                        item.Order = (item.Order + 1);
                        _context.Update(item);
                    }
                }

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                result.Message = result.IsSuccess ? "Successful" : "Error";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Error";
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();
            var element = await _context.Elements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
            var elementDetail = await _context.ElementDetails.Where(x => x.ElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element != null)
            {
                element.IsDeleted = true;
                element.IsActive = false;
                elementDetail.IsDeleted = true;
                elementDetail.IsActive = false;

                _context.Update(element);
                _context.Update(elementDetail);

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                result.Message = result.IsSuccess ? "Successful" : "Error";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Error";
            }

            return result;
        }

        [HttpGet]
        public async Task<List<TagModel>> GetMultipleTagList(Int64 id)
        {
            var result = await _context.MultipleChoiceTag.Where(x => x.IsActive && !x.IsDeleted).Select(x => new TagModel()
            {
                Id = x.Id,
                TagKey = x.Key,
                TagName = x.Name,
                TagValue = x.Value,
            }).ToListAsync();

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddNewTag(List<TagModel> tags)
        {
            var result = new ApiResponse<dynamic>();
            int userId = 0;
            var dbTags = await _context.MultipleChoiceTag.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();

            foreach (var item in tags)
            {
                var tg = dbTags.FirstOrDefault(x => x.Key == item.TagKey);

                if (tg == null)
                {
                    var tag = new MultipleChoiceTag()
                    {
                        Key = item.TagKey,
                        Name = item.TagName,
                        Value = item.TagValue
                    };

                    _context.MultipleChoiceTag.Add(tag);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "duplicate name";
                }
            }

            result.IsSuccess = await _context.SaveCoreContextAsync(userId, DateTimeOffset.Now) > 0;
            result.Message = result.IsSuccess ? "Successful" : "Error";
            return result;
        }

        private async Task<bool> checkElementName(Int64 moduleId, string elementName, List<Element> moduleElements = null)
        {
            if (moduleElements == null)
                moduleElements = _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

            if (moduleElements.FirstOrDefault(x => x.ElementName == elementName) != null)
                return false;
            else
                return true;
        }

    }
}
