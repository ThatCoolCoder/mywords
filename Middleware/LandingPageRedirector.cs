using Microsoft.AspNetCore.Mvc;

namespace Middleware;

public class LandingPageRedirector
{
    public static async Task Main(HttpContext context)
    {
        // if logged out - redirect to / which has a controller with a landing page
        // if logged in - just serve the spa when not found
        //                  (unless looking for the api, then they actually want a 404 so don't give them anything in that case)

        if (context.User.Claims.Count() == 0 && !context.Request.Path.Equals("/")) context.Response.Redirect("/");
        else if (context.User.Claims.Count() != 0 && !context.Request.Path.StartsWithSegments("/Api"))
        {
            await Controllers.LandingPageController.ServeSpaHost(context);
        }
    }
}