using System;

namespace Biovation.Server.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute { }
}