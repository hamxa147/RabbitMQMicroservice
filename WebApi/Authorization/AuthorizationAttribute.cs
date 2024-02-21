﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Authorization
{
    public class AuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string ClaimType { get; set; } = "";

        public AuthorizationAttribute(string _claimType)
        {
            ClaimType = _claimType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check claims
            var hasAccessClaim = context.HttpContext.User.HasClaim(c => c.Type == ClaimType && c.Value == "True");
            if (!hasAccessClaim)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
