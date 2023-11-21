using Microsoft.AspNetCore.Mvc;

namespace Middleware;

public class LandingPageRedirector
{
    public static async Task Main(HttpContext context)
    {
        // if logged out - redirect to / which has a controller with a landing page
        // if logged in - just serve the spa


        context.Response.Redirect("/");
        return;

        if (context.User.Claims.Count() == 0 && !context.Request.Path.Equals("/")) context.Response.Redirect("/");
        else if (context.User.Claims.Count() != 0)
        {
            await context.Response.SendFileAsync("wwwroot/host.html");
            await context.Response.CompleteAsync();
        }
    }
}