using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedMessages.Response;

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
                var response = new ApiResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized",
                    Result = ""
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                return;
            }

            // Check claims
            var hasAccessClaim = context.HttpContext.User.HasClaim(c => c.Type == ClaimType && c.Value == "True");
            if (!hasAccessClaim)
            {
                var response = new ApiResponse
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Forbidden: You do not have access to this resource.",
                    Result = ""
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
        }
    }
}
