using Microsoft.AspNetCore.Mvc;

namespace Controllers;

public class StandardApiControllerAttribute : RouteAttribute
{
    public StandardApiControllerAttribute(string test) : base("Api/" + test)
    {

    }
}