using System.Security.Claims;
using API.Requests;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class HasAccessFilter(IDocumentAccessService accessService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Guid documentId;
        
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (context.ActionArguments.TryGetValue("documentId", out var paramDocId) && paramDocId is Guid docId)
        {
            documentId = docId;
        }
        else if (!context.ActionArguments.TryGetValue("request", out var requestObj) || requestObj is not CustomRequest request)
        {
            context.Result = new BadRequestObjectResult("Invalid or missing request data.");
            return;
        }
        else
        {
            documentId = request.DocumentId;
        }

        var hasAccessResult = await accessService.ValidateAccess(userId, documentId);
        if (hasAccessResult.IsFailure)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}