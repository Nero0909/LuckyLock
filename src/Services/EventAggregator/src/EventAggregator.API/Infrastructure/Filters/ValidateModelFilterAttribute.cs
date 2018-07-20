using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventAggregator.API.Infrastructure.Filters
{
    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
            }
            else if (filterContext.ActionArguments.FirstOrDefault().Value == null)
            {
                filterContext.ModelState.AddModelError("Model Null", "Model is empty");
                filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
