using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Veritabanı_proje.Models;

namespace Veritabanı_proje.Attributes
{
    public class AuthorizeRolesAttribute : ActionFilterAttribute
    {
        private readonly Role[] _roles;

        public AuthorizeRolesAttribute(params Role[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(userRole) || 
                !int.TryParse(userRole, out int roleValue) ||
                !_roles.Contains((Role)roleValue))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
