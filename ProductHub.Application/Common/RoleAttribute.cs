//using Apps_ADM.Application.Interfaces.Collaborators;
//using Apps_ADM.Domain.Constants;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.DependencyInjection;
//using System.Security.Claims;

//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
//public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
//{
//    private readonly string[] _roles;

//    public RoleAuthorizeAttribute(string roles)
//    {
//        _roles = roles.Split(',')
//                      .Select(role => role.Trim())
//                      .ToArray();
//    }

//    public void OnAuthorization(AuthorizationFilterContext context)
//    {
//        var user = context.HttpContext.User;

//        if (user?.Identity == null || !user.Identity.IsAuthenticated)
//        {
//            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
//            return;
//        }

//        var collaboratorService = context.HttpContext.RequestServices.GetService<ICollaboratorService>();
//        var userOid = user.FindFirst(TokenClaimsConstants.UserOid)?.Value;

//        if (string.IsNullOrEmpty(userOid) || collaboratorService == null)
//        {
//            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
//            return;
//        }

//        var collaboratorRolesString = collaboratorService.GetRoleForUser(userOid, CancellationToken.None).Result;

//        var collaboratorRoles = (collaboratorRolesString ?? "")
//            .Split(',', StringSplitOptions.RemoveEmptyEntries)
//            .Select(role => role.Trim())
//            .ToList();

//        var userClaimsRoles = user.FindAll(ClaimTypes.Role)
//            .Select(claim => claim.Value.Trim())
//            .ToList();

//        var allRoles = collaboratorRoles.Concat(userClaimsRoles).Distinct();
//        var cleanedRoles = _roles.Select(r => r.Trim());

//        if (!cleanedRoles.Any(role => allRoles.Contains(role)))
//        {
//            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
//        }
//    }
//}
