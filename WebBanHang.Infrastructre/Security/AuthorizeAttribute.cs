using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;
using WebBanHang.Infrastructre.Models;

namespace WebBanHang.Infrastructre.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        private readonly IList<UserRole> roles;

        public AuthorizeAttribute(params UserRole[] _role)
        {
            roles = _role?? Array.Empty<UserRole>();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            bool annoymousUser = filterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (annoymousUser) return;

            // authorization
            var user = (Users)filterContext.HttpContext.Items["User"];
            var userRole = (UserRole)user.RoleID;
            if (user == null || (roles.Any() && !roles.Contains(userRole)))
            {
                // not logged in or role not authorized
                filterContext.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
