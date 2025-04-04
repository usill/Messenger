using Microsoft.AspNetCore.Http.HttpResults;

namespace TestSignalR.Middleware
{
    public class Security
    {
        private readonly IConfiguration _configuration;
        public Security(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task UseCSRF(HttpContext context, Func<Task> next)
        {
            string origin = context.Request.Headers["Origin"].ToString();
            string referer = context.Request.Headers["Referer"].ToString();

            string domain = _configuration.GetSection("CurrentDomain").Get<string>()!;

            if(!origin.EndsWith(domain) && !referer.EndsWith(domain))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("CSRF validation error");
                return;
            }

            await next();
        }
    }
}
