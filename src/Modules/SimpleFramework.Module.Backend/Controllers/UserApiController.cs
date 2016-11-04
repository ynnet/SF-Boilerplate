﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleFramework.Core.Abstraction.Data;
using SimpleFramework.Core.Entitys;
using SimpleFramework.Core.Web.SmartTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleFramework.Module.Backend.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/users")]
    public class UserApiController : Core.Web.Base.Controllers.ControllerBase
    {
        private readonly IRepositoryWithTypedId<UserEntity, long> userRepository;

        public UserApiController(IRepositoryWithTypedId<UserEntity, long> userRepository,
            IServiceCollection service, 
            ILogger<UserApiController> logger) : base(service,logger)
        {
            this.userRepository = userRepository;
        }

        [HttpPost("grid")]
        public ActionResult List([FromBody] SmartTableParam param)
        {
            var query = userRepository.Queryable()
                // .Include(x => x.Roles).ThenInclude(r => r.Role)
                .Where(x => !x.IsDeleted);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;

                if (search.Email != null)
                {
                    string email = search.Email;
                    query = query.Where(x => x.Email.Contains(email));
                }

                if (search.FullName != null)
                {
                    string fullName = search.FullName;
                    query = query.Where(x => x.FullName.Contains(fullName));
                }

                if (search.CreatedOn != null)
                {
                    if (search.CreatedOn.before != null)
                    {
                        DateTimeOffset before = search.CreatedOn.before;
                        before = before.Date.AddDays(1);
                        // query = query.Where(x => x.CreatedDate <= before);
                    }

                    if (search.CreatedOn.after != null)
                    {
                        DateTimeOffset after = search.CreatedOn.after;
                        after = after.Date;
                        // query = query.Where(x => x.CreatedDate >= after);
                    }
                }
            }

            var users = query.ToSmartTableResult(
                param,
                user => new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    // CreatedOn = user.CreatedDate,
                    //   Roles = string.Join(", ", user.Roles.Select(x => x.Role.Name))
                });

            return Json(users);
        }

        [HttpGet("{id}")]
        public ActionResult Get(long id)
        {
            var user = userRepository.Queryable().FirstOrDefault(x => x.Id == id);

            var model = new
            {
                Id = user.Id,
                FullName = user.FullName
            };

            return Json(model);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            var user = userRepository.Queryable().FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            user.IsDeleted = true;
            userRepository.SaveChange();
            return Json(true);
        }
    }
}
