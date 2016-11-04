﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omu.ValueInjecter;
using SimpleFramework.Core.Entitys;
using SimpleFramework.Core.Web.Base.Controllers;
using SimpleFramework.Core.Web.Base.DataContractMapper;
using SimpleFramework.Module.Backend.ViewModels;
using SimpleFramework.Web.Control.JqGrid.Core.Json;
using SimpleFramework.Web.Control.JqGrid.Core.Request;
using SimpleFramework.Web.Control.JqGrid.Core.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace SimpleFramework.Module.Backend.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Authorize]
    public class UserCrudController : CrudControllerBase<UserEntity, UserViewModel>
    {
        public UserCrudController(IServiceCollection collection, ILogger<UserCrudController> logger) : base(collection, logger)
        {
            CrudDtoMapper = new UserDtoMapper();
        }
        [Route("Users")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(JqGridRequest request, string queryJson)
        {
            int totalRecords = 0;
            var query = _repository.Query().SelectPage(request.PageIndex, request.RecordsCount, out totalRecords);
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords,
            };
            foreach (UserEntity userEntity in query)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(userEntity.Id), userEntity));
            }

            response.Reader.RepeatItems = false;
            return new JqGridJsonResult(response);
        }
    }
    /// <summary>
    /// 用户据映射
    /// </summary>
    public class UserDtoMapper : CrudDtoMapper<UserEntity, UserViewModel>
    {
        /// <summary>
        /// DTO转换领域的实体映射
        /// </summary>
        /// <param name="dto">DTO实体映射</param>
        /// <param name="entity">实体映射DTO</param>
        /// <returns>The entity</returns>
        protected override UserEntity OnMapDtoToEntity(UserViewModel dto, UserEntity entity)
        {
            var retVal = new UserEntity();
            retVal.InjectFrom(dto);
            return retVal;
        }
        /// <summary>
        /// 领域的实体转换DTO映射
        /// </summary>
        /// <param name="entity">实体映射</param>
        /// <param name="dto">DTO映射实体</param>
        /// <returns>The dto</returns>
        protected override UserViewModel OnMapEntityToDto(UserEntity entity, UserViewModel dto)
        {
            var retVal = new UserViewModel();
            retVal.InjectFrom(entity);
            return retVal;
        }
    }
}
