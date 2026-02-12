using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Server.Common
{
    public static class Guard
    {
        public static void AgainstNull(object value, string name = null)
        {
            if (value == null)
            {
                name ??= "Value";
                throw new ArgumentException($"{name} cannot be null");
            }
        }
    }
}
