﻿using Helios.Common.DTO;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Common.Enums;
using Helios.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using Helios.Common.Model;

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
        public async Task<List<ModuleModel>> GetModuleList(Int64 tenantId)
        {
            var result = await _context.Modules.Where(x => x.TenantId == tenantId && x.IsActive && !x.IsDeleted).Select(x => new ModuleModel()
            {
                Id = x.Id,
                Name = x.Name
            }).AsNoTracking().ToListAsync();

            return result;
        }
        #endregion

        [HttpGet]
        public async Task<List<ElementModel>> GetModuleElements(Int64 moduleId)
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
                    CalculationSourceInputs = x.ElementDetail.CalculationSourceInputs,
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
                    DatagridProperties = x.ElementDetail.DatagridProperties,
                    ColumnIndex = x.ElementDetail.ColunmIndex,
                    RowIndex = x.ElementDetail.RowIndex,
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            foreach (var item in result)
            {
                if (item.ParentId == 0)
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
                RelationSourceInputs = x.ElementDetail.RelationSourceInputs,
                RelationMainJs = x.ElementDetail.RelationMainJs,
                ElementOptions = x.ElementDetail.ElementOptions,
                DefaultValue = x.ElementDetail.DefaultValue,
                AddTodayDate = x.ElementDetail.AddTodayDate,
                CalculationSourceInputs = x.ElementDetail.CalculationSourceInputs,
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
                DatagridProperties = x.ElementDetail.DatagridProperties,
                ColumnIndex = x.ElementDetail.ColunmIndex,
                RowIndex = x.ElementDetail.RowIndex,
            }).AsNoTracking().FirstOrDefaultAsync();

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

            if (element == null)
            {
                var moduleElements = _context.Elements.Where(x => x.ModuleId == model.ModuleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

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
                        ElementName = model.ElementName,
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
                            CalculationSourceInputs = model.CalculationSourceInputs,
                            RelationSourceInputs = model.RelationSourceInputs,
                            MainJs = model.MainJs,
                            RelationMainJs = model.RelationMainJs,
                            StartDay = model.StartDay,
                            EndDay = model.EndDay,
                            StartMonth = model.StartMonth,
                            EndMonth = model.EndMonth,
                            StartYear = model.StartYear,
                            EndYear = model.EndYear,
                            IsInCalculation = model.ElementType == ElementType.Calculated,
                            LeftText = model.LeftText,
                            RightText = model.RightText,
                            RowCount = model.RowCount,
                            ColumnCount = model.ColumnCount,
                            DatagridProperties = model.DatagridProperties,
                            RowIndex = model.ElementType == ElementType.DataGrid ? 1 : 0,
                            ColunmIndex = model.ColumnIndex
                            //CreatedAt = DateTimeOffset.Now,
                            //AddedById = userId,
                            //ButtonText = model.buttonText
                        };

                        _context.ElementDetails.Add(elementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        elm.ElementDetailId = elementDetail.Id;
                        _context.Elements.Update(elm);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (model.IsDependent)
                        {
                            var elementEvent = new ModuleElementEvent()
                            {
                                SourceElementId = model.DependentTargetFieldId,
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
                                    TargetElementId = item.elementFieldSelectedGroup.value,
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
                            var relElmIds = relList.Select(x => x.relationFieldsSelectedGroup.value).ToList();

                            foreach (var item in relElmIds)
                            {
                                var elementEvent = new ModuleElementEvent()
                                {
                                    ModuleId = model.ModuleId,
                                    SourceElementId = item,
                                    TargetElementId = elm.Id,
                                    TenantId = model.TenantId,
                                    EventType = EventType.Relation,
                                };

                                _context.ModuleElementEvents.Add(elementEvent);
                            }

                            var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            if (!isSuccess)
                            {
                                element.IsRelated = false;
                                elementDetail.RelationSourceInputs = "";
                                elementDetail.RelationMainJs = "";

                                _context.Elements.Update(element);
                                _context.ElementDetails.Update(elementDetail);
                                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                            }
                        }

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
                        else if (!result.IsSuccess)//if dependent or calculation didn't save
                        {
                            elm.IsActive = false;
                            elm.IsDeleted = true;
                            _context.Elements.Update(elm);

                            elmDtl.IsActive = false;
                            elmDtl.IsDeleted = true;
                            _context.ElementDetails.Update(elmDtl);

                            var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            result.IsSuccess = false;
                            result.Message = "Operation failed. Please try again.";
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
                element.Title = model.Title;
                element.ElementName = model.ElementName;
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
                elementDetail.DatagridProperties = model.DatagridProperties;
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

                if (model.ElementType == ElementType.Calculated)
                {
                    var dbCalcList = JsonSerializer.Deserialize<List<CalculationModel>>(elementDetail.CalculationSourceInputs);
                    var existCalDtil = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == element.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();
                    var elementInExistCalList = await _context.ElementDetails.Where(x => existCalElmIds.Contains(x.ElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                    var calcElmIds = calcList.Select(x => x.elementFieldSelectedGroup.value).ToList();

                    //update updated variable list
                    foreach (var item in dbCalcList)
                    {
                        var c = calcList.FirstOrDefault(x => x.variableName == item.variableName);

                        if (c != null && c.elementFieldSelectedGroup.value != item.elementFieldSelectedGroup.value)
                        {
                            var exc = existCalDtil.FirstOrDefault(x => x.TargetElementId == item.elementFieldSelectedGroup.value && x.IsActive && !x.IsDeleted);

                            exc.TargetElementId = c.elementFieldSelectedGroup.value;
                            _context.CalculatationElementDetails.Update(exc);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    //change elementDetail first
                    foreach (var item in elementInExistCalList)
                    {
                        item.IsInCalculation = false;
                        _context.ElementDetails.Update(item);
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

                    elementDetail.CalculationSourceInputs = model.CalculationSourceInputs;
                    _context.ElementDetails.Update(elementDetail);

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (model.IsRelated)
                {
                    var dbRelList = JsonSerializer.Deserialize<List<RelationModel>>(elementDetail.RelationSourceInputs);
                    var rels = await _context.ModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                    var relList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);
                    var relElmIds = relList.Select(x => x.relationFieldsSelectedGroup.value).ToList();

                    foreach (var item in dbRelList)
                    {
                        var r = relList.FirstOrDefault(x => x.variableName == item.variableName);

                        if (r != null && r.relationFieldsSelectedGroup.value != item.relationFieldsSelectedGroup.value)
                        {
                            var exr = rels.FirstOrDefault(x => x.TargetElementId == item.relationFieldsSelectedGroup.value && x.IsActive && !x.IsDeleted);

                            exr.TargetElementId = r.relationFieldsSelectedGroup.value;
                            _context.ModuleElementEvents.Update(exr);
                        }
                    }

                    var relIds = rels.Select(x => x.SourceElementId).ToList();

                    //first add unadded rows to evet
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
                            };

                            _context.ModuleElementEvents.Add(elementEvent);
                        }
                    }

                    //then remove deleted rows
                    foreach (var item in rels)
                    {
                        var delRel = relList.FirstOrDefault(x => x.relationFieldsSelectedGroup.value == item.SourceElementId);

                        if (delRel == null)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.ModuleElementEvents.Add(item);
                        }
                    }

                    elementDetail.RelationSourceInputs = model.RelationSourceInputs;
                    _context.ElementDetails.Update(elementDetail);

                    var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    if (!isSuccess)
                    {
                        element.IsRelated = false;
                        elementDetail.RelationSourceInputs = "";
                        elementDetail.RelationMainJs = "";

                        _context.Elements.Update(element);
                        _context.ElementDetails.Update(elementDetail);
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
                if (elementDetail.IsInCalculation)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used in a calculation element formul. Please remove it first from calculation element.";

                    return result;
                }

                var moduleEvent = _context.ModuleElementEvents.FirstOrDefault(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted);

                if (moduleEvent != null)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used as relation or dependent in another element. Please remove it first and try again.";

                    return result;
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
