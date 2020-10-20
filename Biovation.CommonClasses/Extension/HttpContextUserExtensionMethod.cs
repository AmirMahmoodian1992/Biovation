using Biovation.Domain;
using Microsoft.AspNetCore.Http;

namespace Biovation.CommonClasses.Extension
{
    public static class HttpContextUserExtensionMethod
    {
        public static User GetUser(this HttpContext context)
        {
            try
            {
                return (User)context.Items["User"];
            }
            catch (System.Exception)
            {
                return new User();
            }
        }
    }
}
