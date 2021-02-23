using System;

namespace ChatModule
{
    internal class Utils
    {
        public static T IsNotNull<T>(T arg, string argName)
            where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, $"{argName} cannot be null");
            }
            return arg;
        }

        public static T IsNotNullT<T>(T arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, $"{argName} cannot be null");
            }
            return arg;
        }
        public static bool IsFailure(Azure.Response response)
            => !IsSuccess(response);

        public static bool IsSuccess(Azure.Response response)
        {
            return response.Status >= 200 && response.Status <= 299;
        }
    }
}
