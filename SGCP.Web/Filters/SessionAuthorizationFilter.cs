using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SGCP.Web.Filters
{
    public class SessionAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();

            if (controllerName?.Equals("AuthController_MVC", StringComparison.OrdinalIgnoreCase) == true)
            {
                return; 
            }

            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Login", "AuthController_MVC", null);
            }
        }
    }
}