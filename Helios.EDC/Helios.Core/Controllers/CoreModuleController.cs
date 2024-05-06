using Helios.Common.DTO;
using Helios.Core.Contexts;
using Helios.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Helios.Common.Model;
using Helios.Core.helpers;
using Helios.Core.Domains.Entities;
using Helios.Common.Helpers.Api;

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
            _context.Modules.Add(new Module
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
        public async Task<ApiResponse<dynamic>> DeleteModule(ModuleModel model)
        {
            var result = new ApiResponse<dynamic>();
            var module = await _context.Modules.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted)
                .Include(x => x.Elements)
                .ThenInclude(x => x.ElementDetail)
                .Include(x => x.Elements)
                .ThenInclude(x => x.ModuleElementEvents)
                .Include(x => x.Elements)
                .ThenInclude(x => x.CalculatationElementDetails)
                .FirstOrDefaultAsync();

            if (module == null)
            {
                result.IsSuccess = false;
                result.Message = "Unsuccessful";
            }

            module.UpdatedAt = DateTimeOffset.Now;
            module.UpdatedById = model.UserId;
            module.IsActive = false;
            module.IsDeleted = true;

            foreach (var item in module.Elements)
            {
                item.UpdatedAt = DateTimeOffset.Now;
                item.UpdatedById = model.UserId;
                item.IsActive = false;
                item.IsDeleted = true;

                item.ElementDetail.UpdatedAt = DateTimeOffset.Now;
                item.ElementDetail.UpdatedById = model.UserId;
                item.ElementDetail.IsActive = false;
                item.ElementDetail.IsDeleted = true;

                foreach (var evnt in item.ModuleElementEvents)
                {
                    evnt.UpdatedAt = DateTimeOffset.Now;
                    evnt.UpdatedById = model.UserId;
                    evnt.IsActive = false;
                    evnt.IsDeleted = true;
                }

                foreach (var cal in item.CalculatationElementDetails)
                {
                    cal.UpdatedAt = DateTimeOffset.Now;
                    cal.UpdatedById = model.UserId;
                    cal.IsActive = false;
                    cal.IsDeleted = true;
                }
            }

            _context.Modules.Update(module);
            var res = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

            if (res)
            {
                result.IsSuccess = true;
                result.Message = "Successful";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Unsuccessful";
            }

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
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var result = await _context.Modules.Where(x => x.TenantId == baseDTO.TenantId && x.IsActive && !x.IsDeleted).Select(x => new ModuleModel()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking().ToListAsync();

            return result;
        }

        #endregion

        [HttpGet]
        public async Task<List<ElementModel>> GetModuleAllElements(Int64 moduleId)
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
                    IsRequired = x.IsRequired,
                    ElementOptions = x.ElementDetail.ElementOptions,
                    Width = x.Width,
                    Unit = x.ElementDetail.Unit,
                    Mask = x.ElementDetail.Mask,
                    LowerLimit = x.ElementDetail.LowerLimit,
                    UpperLimit = x.ElementDetail.UpperLimit,
                    Layout = x.ElementDetail.Layout,
                    DefaultValue = x.ElementDetail.DefaultValue,
                    AddTodayDate = x.ElementDetail.AddTodayDate,
                    MainJs = x.ElementDetail.MainJs,
                    StartDay = x.ElementDetail.StartDay,
                    EndDay = x.ElementDetail.EndDay,
                    StartMonth = x.ElementDetail.StartMonth,
                    EndMonth = x.ElementDetail.EndMonth,
                    StartYear = x.ElementDetail.StartYear,
                    EndYear = x.ElementDetail.EndYear,
                    LeftText = x.ElementDetail.LeftText,
                    RightText = x.ElementDetail.RightText
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<List<ElementModel>> GetModuleElementsWithChildren(Int64 moduleId)
        {
            var finalList = new List<ElementModel>();

            var result = await _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.ElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    ParentId = x.ElementDetail.ParentId,
                    Title = x.Title,
                    Description = x.Description,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    IsRequired = x.IsRequired,
                    ElementOptions = x.ElementDetail.ElementOptions,
                    Width = x.Width,
                    Unit = x.ElementDetail.Unit,
                    Mask = x.ElementDetail.Mask,
                    LowerLimit = x.ElementDetail.LowerLimit,
                    UpperLimit = x.ElementDetail.UpperLimit,
                    Layout = x.ElementDetail.Layout,
                    DefaultValue = x.ElementDetail.DefaultValue,
                    AddTodayDate = x.ElementDetail.AddTodayDate,
                    MainJs = x.ElementDetail.MainJs,
                    StartDay = x.ElementDetail.StartDay,
                    EndDay = x.ElementDetail.EndDay,
                    StartMonth = x.ElementDetail.StartMonth,
                    EndMonth = x.ElementDetail.EndMonth,
                    StartYear = x.ElementDetail.StartYear,
                    EndYear = x.ElementDetail.EndYear,
                    LeftText = x.ElementDetail.LeftText,
                    RightText = x.ElementDetail.RightText,
                    ColumnCount = x.ElementDetail.ColumnCount,
                    RowCount = x.ElementDetail.RowCount,
                    DatagridAndTableProperties = x.ElementDetail.DatagridAndTableProperties,
                    ColumnIndex = x.ElementDetail.ColunmIndex,
                    RowIndex = x.ElementDetail.RowIndex,
                    AdverseEventType = x.ElementDetail.AdverseEventType
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            foreach (var item in result)
            {
                if (item.ParentId == 0 || item.ParentId == null)
                    finalList.Add(item);
                else
                {
                    var parent = result.FirstOrDefault(x => x.Id == item.ParentId);

                    parent.ChildElements.Add(item);
                }
            }

            return finalList;
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
                ParentId = x.ElementDetail.ParentId,
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
                RelationMainJs = x.ElementDetail.RelationMainJs,
                ElementOptions = x.ElementDetail.ElementOptions,
                DefaultValue = x.ElementDetail.DefaultValue,
                AddTodayDate = x.ElementDetail.AddTodayDate,
                MainJs = x.ElementDetail.MainJs,
                StartDay = x.ElementDetail.StartDay,
                EndDay = x.ElementDetail.EndDay,
                StartMonth = x.ElementDetail.StartMonth,
                EndMonth = x.ElementDetail.EndMonth,
                StartYear = x.ElementDetail.StartYear,
                EndYear = x.ElementDetail.EndYear,
                LeftText = x.ElementDetail.LeftText,
                RightText = x.ElementDetail.RightText,
                ColumnCount = x.ElementDetail.ColumnCount,
                RowCount = x.ElementDetail.RowCount,
                DatagridAndTableProperties = x.ElementDetail.DatagridAndTableProperties,
                ColumnIndex = x.ElementDetail.ColunmIndex,
                RowIndex = x.ElementDetail.RowIndex,
                AdverseEventType = x.ElementDetail.AdverseEventType
            }).AsNoTracking().FirstOrDefaultAsync();

            var events = new List<ModuleElementEvent>();

            if (result.IsRelated || result.IsDependent)
            {
                events = await _context.ModuleElementEvents.Where(x => x.TargetElementId == id && x.IsActive && !x.IsDeleted).ToListAsync();
            }

            if (result.IsDependent)
            {
                var dep = events.FirstOrDefault(x => x.EventType == EventType.Dependency && x.IsActive && !x.IsDeleted);

                if (dep != null)
                {
                    result.DependentSourceFieldId = dep.SourceElementId;
                    result.DependentTargetFieldId = dep.TargetElementId;
                    result.DependentCondition = (int)dep.ValueCondition;
                    result.DependentAction = (int)dep.ActionType;
                    result.DependentFieldValue = dep.ActionValue;
                }
            }

            if (result.IsRelated)
            {
                var rels = events.Where(x => x.EventType == EventType.Relation && x.IsActive && !x.IsDeleted).ToList();
                var relSrcIds = rels.Select(x => x.SourceElementId).ToList();
                var srcs = await _context.Elements.Where(x => relSrcIds.Contains(x.Id)).ToArrayAsync();
                var relStr = "[";

                foreach (var item in rels)
                {
                    var src = srcs.FirstOrDefault(x => x.Id == item.SourceElementId);

                    relStr += "{\"relationFieldsSelectedGroup\":{\"label\":\"" + src.ElementName + " - " + StringExtensionsHelper.GetEnumDescription(src.ElementType) + "\",\"value\":" + item.SourceElementId + "},\"variableName\":\"" + item.VariableName + "\"}";

                    if (item == rels.LastOrDefault())
                        relStr += "]";
                    else
                        relStr += ",";
                }

                result.RelationSourceInputs = relStr;
            }

            if (result.ElementType == ElementType.Calculated)
            {
                var cals = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == result.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                var calSrcIds = cals.Select(x => x.TargetElementId).ToList();
                var srcs = await _context.Elements.Where(x => calSrcIds.Contains(x.Id)).ToArrayAsync();

                var calStr = "[";

                foreach (var item in cals)
                {
                    var src = srcs.FirstOrDefault(x => x.Id == item.TargetElementId);

                    calStr += "{\"elementFieldSelectedGroup\":{\"label\":\"" + src.ElementName + " - " + StringExtensionsHelper.GetEnumDescription(src.ElementType) + "\",\"value\":" + item.TargetElementId + "},\"variableName\":\"" + item.VariableName + "\"}";

                    if (item == cals.LastOrDefault())
                        calStr += "]";
                    else
                        calStr += ",";
                }

                result.CalculationSourceInputs = calStr;
            }

            var validations = await _context.ElementValidationDetails.Where(x => x.ElementId == result.Id && x.IsActive && !x.IsDeleted).ToListAsync();

            if (validations != null)
            {
                result.ValidationList = JsonSerializer.Serialize(validations);
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SaveModuleContent(ElementModel model)
        {
            var result = new ApiResponse<dynamic>() { Message = "Operation is successfully." };
            var calcList = new List<CalculationModel>();

            var element = await _context.Elements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (model.ElementType == ElementType.Calculated)
            {
                if (model.CalculationSourceInputs == null || model.CalculationSourceInputs == "[]" || string.IsNullOrEmpty(model.CalculationSourceInputs))
                {
                    result.IsSuccess = false;
                    result.Message = "Error in calculation elements selection";

                    return result;
                }

                try
                {
                    calcList = JsonSerializer.Deserialize<List<CalculationModel>>(model.CalculationSourceInputs);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "Error in calculation elements selection";

                    return result;
                }
            }

            var moduleElements = _context.Elements.Where(x => x.ModuleId == model.ModuleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

            if (element == null)
            {
                if (!checkElementName(model.ModuleId, model.ElementName, moduleElements).Result)
                {
                    result.IsSuccess = false;
                    result.Message = "Duplicate element name";

                    return result;
                }

                var elm = new Element();

                try
                {
                    var moduleElementMaxOrder = moduleElements.Count() > 0 ? moduleElements.Select(x => x.Order).Max() : 1;

                    elm = new Element()
                    {
                        Title = model.Title,
                        ElementName = model.ElementName.TrimStart().TrimEnd(),
                        Description = model.Description,
                        CanMissing = model.CanMissing,
                        ElementType = model.ElementType,
                        IsDependent = model.IsDependent,
                        IsHidden = model.IsHidden,
                        IsReadonly = model.IsReadonly,
                        IsRequired = model.IsRequired,
                        IsRelated = model.IsRelated,
                        IsTitleHidden = model.IsTitleHidden,
                        Width = model.Width,
                        ModuleId = model.ModuleId,
                        TenantId = model.TenantId,
                        Order = model.ParentId == 0 ? moduleElementMaxOrder + 1 : 0,
                    };

                    _context.Elements.Add(elm);
                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    if (result.IsSuccess)
                    {
                        var elementDetail = new ElementDetail()
                        {
                            ElementId = elm.Id,
                            TenantId = model.TenantId,
                            ParentId = model.ParentId,
                            Unit = model.Unit,
                            Mask = model.Mask,
                            LowerLimit = model.LowerLimit,
                            UpperLimit = model.UpperLimit,
                            Layout = model.Layout,
                            ElementOptions = model.ElementOptions,
                            MetaDataTags = model.ElementName,
                            DefaultValue = model.DefaultValue,
                            AddTodayDate = model.AddTodayDate,
                            MainJs = model.MainJs,
                            RelationMainJs = model.RelationMainJs,
                            StartDay = model.ElementType != ElementType.DateOption ? 0 : model.StartDay,
                            EndDay = model.ElementType != ElementType.DateOption ? 0 : model.EndDay,
                            StartMonth = model.ElementType != ElementType.DateOption ? 0 : model.StartMonth,
                            EndMonth = model.ElementType != ElementType.DateOption ? 0 : model.EndMonth,
                            StartYear = model.ElementType != ElementType.DateOption ? 0 : model.StartYear,
                            EndYear = model.ElementType != ElementType.DateOption ? 0 : model.EndYear,
                            IsInCalculation = model.ElementType == ElementType.Calculated,
                            LeftText = model.LeftText,
                            RightText = model.RightText,
                            RowCount = model.RowCount,
                            ColumnCount = model.ColumnCount,
                            DatagridAndTableProperties = model.DatagridAndTableProperties,
                            RowIndex = model.ParentId == 0 ? 0 : model.RowIndex,
                            ColunmIndex = model.ColumnIndex,
                            AdverseEventType = model.AdverseEventType,
                            //CreatedAt = DateTimeOffset.Now,
                            //AddedById = userId,
                            //ButtonText = model.buttonText
                        };

                        _context.ElementDetails.Add(elementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (!result.IsSuccess)
                        {
                            elm.IsActive = false;
                            elm.IsDeleted = true;
                            _context.Elements.Update(elm);
                            var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            result.IsSuccess = false;
                            result.Message = "Operation failed. Please try again.";

                            return result;
                        }

                        if (model.IsDependent)
                        {
                            var elementEvent = new ModuleElementEvent()
                            {
                                SourceElementId = model.DependentSourceFieldId,
                                TargetElementId = elm.Id,
                                ModuleId = model.ModuleId,
                                TenantId = model.TenantId,
                                EventType = EventType.Dependency,
                                ValueCondition = (ActionCondition)model.DependentCondition,
                                ActionType = (ActionType)model.DependentAction,
                                ActionValue = model.DependentFieldValue,
                            };

                            _context.ModuleElementEvents.Add(elementEvent);
                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }

                        if (model.ElementType == ElementType.Calculated)
                        {
                            var calcElmIds = calcList.Select(x => x.elementFieldSelectedGroup.value).ToList();
                            var elementInCalList = await _context.ElementDetails.Where(x => calcElmIds.Contains(x.ElementId)).ToListAsync();

                            foreach (var item in calcList)
                            {
                                var calcDtil = new CalculatationElementDetail()
                                {
                                    TenantId = model.TenantId,
                                    ModuleId = model.ModuleId,
                                    CalculationElementId = elm.Id,
                                    TargetElementId = item.elementFieldSelectedGroup.value != 0 ? item.elementFieldSelectedGroup.value : elm.Id,//own element is in calculation list
                                    VariableName = item.variableName
                                };

                                _context.CalculatationElementDetails.Add(calcDtil);
                            }

                            foreach (var item in elementInCalList)
                            {
                                item.IsInCalculation = true;

                                _context.ElementDetails.Update(item);
                            }

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }

                        if (model.IsRelated)
                        {
                            var relList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);

                            foreach (var item in relList)
                            {
                                var elementEvent = new ModuleElementEvent()
                                {
                                    ModuleId = model.ModuleId,
                                    SourceElementId = item.relationFieldsSelectedGroup.value != 0 ? item.relationFieldsSelectedGroup.value : elm.Id,//element related to own
                                    TargetElementId = elm.Id,
                                    TenantId = model.TenantId,
                                    EventType = EventType.Relation,
                                    VariableName = item.variableName
                                };

                                _context.ModuleElementEvents.Add(elementEvent);
                            }

                            var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            if (!isSuccess)
                            {
                                element.IsRelated = false;
                                elementDetail.RelationMainJs = "";

                                _context.Elements.Update(element);
                                _context.ElementDetails.Update(elementDetail);
                                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                            }
                        }

                        if (model.HasValidation)
                        {
                            var validations = JsonSerializer.Deserialize<List<ElementValidationModel>>(model.ValidationList);

                            foreach (var item in validations)
                            {
                                var validation = new ElementValidationDetail
                                {
                                    ModuleId = model.ModuleId,
                                    ElementId = elm.Id,
                                    ActionType = item.ValidationActionType,
                                    ValueCondition = item.ValidationCondition,
                                    Value = item.ValidationValue,
                                    Message = item.ValidationMessage,
                                };

                                _context.ElementValidationDetails.Add(validation);
                            }

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }

                        if (!result.IsSuccess)//if dependent or calculation didn't save
                        {
                            elm.IsActive = false;
                            elm.IsDeleted = true;
                            _context.Elements.Update(elm);

                            elementDetail.IsActive = false;
                            elementDetail.IsDeleted = true;
                            _context.ElementDetails.Update(elementDetail);

                            var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            result.IsSuccess = false;
                            result.Message = "Operation failed. Please try again to save dependents or calculation part.";
                        }
                    }
                    else
                    {
                        result.Message = "Error";
                    }
                }
                catch (Exception ex)
                {
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
                }
            }
            else
            {
                if (element.ElementName.TrimStart().TrimEnd() != model.ElementName.TrimStart().TrimEnd())
                {
                    if (!checkElementName(model.ModuleId, model.ElementName, moduleElements).Result)
                    {
                        result.IsSuccess = false;
                        result.Message = "Duplicate element name";

                        return result;
                    }
                }

                element.Title = model.Title;
                element.ElementName = model.ElementName.TrimStart().TrimEnd();
                element.Description = model.Description;
                element.CanMissing = model.CanMissing;
                element.ElementType = model.ElementType;
                element.IsDependent = model.IsDependent;
                element.IsHidden = model.IsHidden;
                element.IsReadonly = model.IsReadonly;
                element.IsRequired = model.IsRequired;
                element.IsRelated = model.IsRelated;
                element.IsTitleHidden = model.IsTitleHidden;
                element.Width = model.Width;
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
                elementDetail.MainJs = model.MainJs;
                elementDetail.RelationMainJs = model.RelationMainJs;
                elementDetail.StartDay = model.StartDay;
                elementDetail.EndDay = model.EndDay;
                elementDetail.StartMonth = model.StartMonth;
                elementDetail.EndMonth = model.EndMonth;
                elementDetail.StartYear = model.StartYear;
                elementDetail.EndYear = model.EndYear;
                elementDetail.LeftText = model.LeftText;
                elementDetail.RightText = model.RightText;
                elementDetail.RowCount = model.RowCount;
                elementDetail.ColumnCount = model.ColumnCount;
                elementDetail.AdverseEventType = model.AdverseEventType;
                elementDetail.DatagridAndTableProperties = model.DatagridAndTableProperties;
                element.UpdatedAt = DateTimeOffset.Now;
                element.UpdatedById = model.UserId;

                _context.Update(elementDetail);
                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (model.IsDependent)
                {
                    var dep = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted);

                    if (dep == null)
                    {
                        var elementEvent = new ModuleElementEvent()
                        {
                            SourceElementId = model.DependentSourceFieldId,
                            TargetElementId = element.Id,
                            EventType = EventType.Dependency,
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

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }
                else
                {
                    var dep = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == model.Id && x.EventType == EventType.Dependency && x.IsActive && !x.IsDeleted);

                    if (dep != null)
                    {
                        dep.IsActive = false;
                        dep.IsDeleted = true;

                        _context.ModuleElementEvents.Update(dep);
                    }
                }

                if (model.ElementType == ElementType.Calculated)
                {
                    var existCalDtil = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == element.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();
                    var elementInExistCalList = await _context.ElementDetails.Where(x => existCalElmIds.Contains(x.ElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                    var calcElmIds = calcList.Select(x => x.elementFieldSelectedGroup.value).ToList();

                    var elementInExistCalListIds = elementInExistCalList.Select(x => x.ElementId).ToList();
                    var allCalDtil = await _context.CalculatationElementDetails.Where(x => elementInExistCalListIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                    //update updated variable list
                    foreach (var item in existCalDtil)
                    {
                        var c = calcList.FirstOrDefault(x => x.variableName == item.VariableName);

                        if (c != null && c.elementFieldSelectedGroup.value != item.TargetElementId)
                        {
                            item.TargetElementId = c.elementFieldSelectedGroup.value;
                            _context.CalculatationElementDetails.Update(item);
                        }

                        var cc = calcList.FirstOrDefault(x => x.elementFieldSelectedGroup.value == item.TargetElementId);

                        if (cc != null && cc.variableName != item.VariableName)
                        {
                            item.VariableName = cc.variableName;
                            _context.CalculatationElementDetails.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    //change elementDetail first
                    foreach (var item in elementInExistCalList)
                    {
                        var a = allCalDtil.Count(x => x.TargetElementId == item.ElementId);

                        if (a == 1)//if element used in another calculation element too, IsInCalculation must remain true.
                        {
                            item.IsInCalculation = false;
                            _context.ElementDetails.Update(item);
                        }
                    }

                    var elementInCalList = await _context.ElementDetails.Where(x => calcElmIds.Contains(x.ElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                    //then change new elementDetail flags
                    foreach (var item in elementInCalList)
                    {
                        item.IsInCalculation = true;
                        _context.ElementDetails.Update(item);
                    }

                    existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();//ids updated

                    //add new calcElementDetails
                    foreach (var item in calcList)
                    {
                        if (!existCalElmIds.Contains(item.elementFieldSelectedGroup.value))
                        {
                            var calcDtil = new CalculatationElementDetail()
                            {
                                TenantId = model.TenantId,
                                ModuleId = model.ModuleId,
                                CalculationElementId = element.Id,
                                TargetElementId = item.elementFieldSelectedGroup.value,
                                VariableName = item.variableName
                            };

                            _context.CalculatationElementDetails.Add(calcDtil);
                        }
                    }

                    //remove deleted items from calc
                    foreach (var item in existCalDtil)
                    {
                        if (!calcElmIds.Contains(item.TargetElementId))
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.Update(item);
                        }
                    }

                    _context.ElementDetails.Update(elementDetail);

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (model.IsRelated)
                {
                    try
                    {
                        var rels = await _context.ModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                        var orgRelList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);
                        var relList = orgRelList;
                        var relElmIds = relList.Select(x => x.relationFieldsSelectedGroup.value).ToList();

                        foreach (var item in rels)
                        {
                            var r = relList.FirstOrDefault(x => x.variableName == item.VariableName);

                            if (r != null && r.relationFieldsSelectedGroup.value != item.SourceElementId)
                            {
                                item.SourceElementId = r.relationFieldsSelectedGroup.value;
                                _context.ModuleElementEvents.Update(item);
                            }
                        }

                        var relIds = rels.Select(x => x.SourceElementId).ToList();

                        //add unadded rows to evet
                        foreach (var item in relList)
                        {
                            if (!relIds.Contains(item.relationFieldsSelectedGroup.value))
                            {
                                var elementEvent = new ModuleElementEvent()
                                {
                                    ModuleId = model.ModuleId,
                                    SourceElementId = item.relationFieldsSelectedGroup.value,
                                    TargetElementId = model.Id,
                                    TenantId = model.TenantId,
                                    EventType = EventType.Relation,
                                    VariableName = item.variableName
                                };

                                _context.ModuleElementEvents.Add(elementEvent);
                            }
                        }

                        //remove deleted rows
                        foreach (var item in rels)
                        {
                            var delRel = orgRelList.FirstOrDefault(x => x.relationFieldsSelectedGroup.value == item.SourceElementId);

                            if (delRel == null)
                            {
                                item.IsActive = false;
                                item.IsDeleted = true;

                                _context.ModuleElementEvents.Update(item);
                            }
                        }

                        _context.ElementDetails.Update(elementDetail);

                        var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (!isSuccess)
                        {
                            element.IsRelated = false;
                            elementDetail.RelationMainJs = "";

                            _context.Elements.Update(element);
                            _context.ElementDetails.Update(elementDetail);
                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        element.IsRelated = false;
                        elementDetail.RelationMainJs = "";

                        _context.Elements.Update(element);
                        _context.ElementDetails.Update(elementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }
                else
                {
                    var rels = await _context.ModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.EventType == EventType.Relation && x.IsActive && !x.IsDeleted).ToListAsync();

                    if (rels.Count > 0)
                    {
                        foreach (var item in rels)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.ModuleElementEvents.Update(item);
                        }
                    }
                }

                if (model.HasValidation)
                {
                    var dbValidations = await _context.ElementValidationDetails.Where(x => x.ElementId == model.Id).ToListAsync();
                    var validations = JsonSerializer.Deserialize<List<ElementValidationModel>>(model.ValidationList);

                    //add or update
                    foreach (var item in validations)
                    {
                        var existVal = dbValidations.FirstOrDefault(x => x.Id == item.Id && x.IsActive && !x.IsDeleted);

                        if (existVal != null)
                        {
                            existVal.Value = item.ValidationValue;
                            existVal.ValueCondition = item.ValidationCondition;
                            existVal.Message = item.ValidationMessage;
                            existVal.ActionType = item.ValidationActionType;

                            _context.ElementValidationDetails.Update(existVal);
                        }
                        else
                        {
                            var validation = new ElementValidationDetail
                            {
                                ModuleId = model.ModuleId,
                                ElementId = element.Id,
                                ActionType = item.ValidationActionType,
                                ValueCondition = (ActionCondition)item.ValidationCondition,
                                Value = item.ValidationValue,
                                Message = item.ValidationMessage,
                            };

                            _context.ElementValidationDetails.Add(validation);
                        }

                    }

                    //delete
                    foreach (var item in dbValidations)
                    {
                        var existVal = validations.FirstOrDefault(x => x.Id == item.Id);

                        if (existVal == null)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.ElementValidationDetails.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }
                else
                {
                    var dbValidations = await _context.ElementValidationDetails.Where(x => x.ElementId == model.Id).ToListAsync();

                    if (dbValidations.Count > 0)
                    {
                        foreach (var item in dbValidations)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.ElementValidationDetails.Update(item);
                        }

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }
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
                element.Order = element.Order + 1;

                var elementDetail = await _context.ElementDetails.Where(x => x.ElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                elementDetail.Id = 0;

                _context.Add(element);
                _context.Add(elementDetail);

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                elementDetail.ElementId = element.Id;
                _context.Update(elementDetail);

                if (element.ElementType == ElementType.Calculated)
                {
                    var calcdtls = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == model.Id).ToListAsync();

                    foreach (var cal in calcdtls)
                    {
                        cal.CalculationElementId = element.Id;
                        _context.Add(cal);
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (element.ElementType == ElementType.DataGrid || element.ElementType == ElementType.Table)
                {
                    var childrenDtils = await _context.ElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                    var chldrnIds = childrenDtils.Select(x => x.ElementId).ToList();
                    var children = await _context.Elements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                    foreach (var child in children)
                    {
                        var nm = child.ElementName + "_1";

                        for (; ; )
                        {
                            if (checkElementName(child.ModuleId, nm).Result)
                                break;
                            else
                                nm = nm + "_1";
                        }

                        var chDtl = childrenDtils.FirstOrDefault(x => x.ElementId == child.Id);
                        chDtl.Id = 0;

                        child.Id = 0;
                        child.ElementName = nm;
                        child.Order = child.Order + 1;

                        _context.Add(child);
                        _context.Add(chDtl);

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        chDtl.Id = child.Id;
                        chDtl.ParentId = element.Id;

                        _context.Update(chDtl);

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }

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
                if (elementDetail.IsInCalculation && element.ElementType != ElementType.Calculated)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used in a calculation element formul. Please remove it first from calculation element.";

                    return result;
                }

                var moduleEvent = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted);

                if (moduleEvent != null)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used as relation or dependent in another element. Please remove it first and try again.";

                    return result;
                }

                if (element.ElementType == ElementType.DataGrid || element.ElementType == ElementType.Table)
                {
                    var childrenDtils = await _context.ElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                    var chldrnIds = childrenDtils.Select(x => x.ElementId).ToList();

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.ElementDetails.Update(item);
                    }

                    var children = await _context.Elements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                    foreach (var item in children)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.Elements.Update(item);
                    }
                }

                //remove events
                var moduleEvents = await _context.ModuleElementEvents.Where(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in moduleEvents)
                {
                    item.IsActive = false;
                    item.IsDeleted = true;

                    _context.ModuleElementEvents.Update(item);
                }

                if (element.ElementType == ElementType.Calculated)
                {
                    var childrenDtils = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var targetElmIds = childrenDtils.Select(x => x.TargetElementId).ToList();

                    var anotherCalcDtils = await _context.CalculatationElementDetails.Where(x => targetElmIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).GroupBy(x => x.TargetElementId).ToListAsync();

                    var chngIds = new List<Int64>();

                    foreach (var item in anotherCalcDtils)
                    {
                        if (item.ToList().Count == 1)
                            chngIds.Add(item.FirstOrDefault().TargetElementId);
                    }

                    var elmDtils = _context.ElementDetails.Where(x => targetElmIds.Contains(x.ElementId) && x.IsActive && !x.IsDeleted).ToList();

                    foreach (var item in elmDtils)
                    {
                        var elmCnt = elmDtils.Where(x => x.Id == item.Id).Count();

                        if (chngIds.Contains(item.ElementId) && elmCnt == 1)
                        {
                            item.IsInCalculation = false;

                            _context.ElementDetails.Update(item);
                        }
                    }

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.CalculatationElementDetails.Update(item);
                    }
                }

                //remove validations
                var validations = await _context.ElementValidationDetails.Where(x => x.ElementId == element.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                if (validations != null && validations.Count > 0)
                {
                    foreach (var validation in validations)
                    {
                        validation.IsActive = false;
                        validation.IsDeleted = true;

                        _context.ElementValidationDetails.Update(validation);
                    }
                }

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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> RemoveMultipleTagList([FromBody] JsonElement data)
        {
            try
            {
                if (!data.TryGetProperty("id", out var idElement) || idElement.ValueKind != JsonValueKind.Number)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }

                long id = idElement.GetInt64();

                var tag = await _context.MultipleChoiceTag.FirstOrDefaultAsync(x => x.Id == id);

                if (tag != null)
                {
                    BaseDTO baseDTO = Request.Headers.GetBaseInformation();
                    _context.MultipleChoiceTag.Remove(tag);

                    var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > -1;

                    if (result)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
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
                        Name = item.TagName.TrimStart().TrimEnd(),
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
            result.Message = result.IsSuccess ? "Successful" : "There is a list of tags registered with this name. Please try again with a different name.";
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AutoSaveElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();



            return result;
        }

        private async Task<bool> checkElementName(Int64 moduleId, string elementName, List<Element> moduleElements = null)
        {
            if (moduleElements == null)
                moduleElements = _context.Elements.Where(x => x.ModuleId == moduleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

            if (moduleElements.FirstOrDefault(x => x.ElementName == elementName.TrimStart().TrimEnd()) != null)
                return false;
            else
                return true;
        }

    }
}
