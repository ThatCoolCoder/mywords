using System;
using Microsoft.AspNetCore.Mvc;

namespace Middleware;

public class LandingPageRedirector
{
    public static async Task RedirectIfNeeded(HttpContext context)
    {
        // if logged out - redirect to / which has a controller with a landing page (but obviously don't do anything if already there)
        // if logged in - just serve the spa when not found
        //                  (unless looking for the api, then they actually want a 404 so don't give them any in that case)
        //                  (unless looking for password reset page, since you might want to do a password reset while logged in)

        if (!context.User.Claims.Any() && !context.Request.Path.Equals("/"))
        {
            context.Response.Redirect("/");
        }
        else
        {
            if (context.User.Claims.Any() && !context.Request.Path.StartsWithSegments("/api") &&
                !context.Request.Path.StartsWithSegments("/RequestResetPassword"))
            {
                await Controllers.LandingPageController.ServeSpaHost(context);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}