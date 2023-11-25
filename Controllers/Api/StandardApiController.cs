using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api;

public class StandardApiControllerAttribute : RouteAttribute
{
    public StandardApiControllerAttribute(string test) : base("Api/" + test)
    {

    }
}