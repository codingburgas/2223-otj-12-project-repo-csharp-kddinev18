using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using System.Text;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly SessionOptions _sessionOptions;

        public BaseController(IOptions<SessionOptions> sessionOptions)
        {
            _sessionOptions = sessionOptions.Value;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = HttpContext.Session;
            var lastActivityTime = session.GetString("LastActivityTime");

            if (lastActivityTime != null)
            {
                TimeSpan timeSinceLastActivity = DateTime.Now - DateTime.Parse(lastActivityTime);

                if (timeSinceLastActivity >= _sessionOptions.IdleTimeout)
                {
                    context.Result = new RedirectToRouteResult(new { controller = "Home", action = "Index" });
                    return;
                }
                session.SetString("LastActivityTime", DateTime.Now.ToString());
            }
            else
            {
                context.Result = new RedirectToRouteResult(new { controller = "Home", action = "Index" });
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}

