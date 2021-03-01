using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatModule
{
    internal class Utils
    {
        public static void IsNotNull(string arg, string argName)
        {
            if (arg == null || string.IsNullOrWhiteSpace(arg))
            {
                throw new ArgumentNullException(argName, $"{argName} cannot be null");
            }
        }
        public static void IsNotNull<T>(T arg, string argName)
            where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, $"{argName} cannot be null");
            }
        }

        public static void IsNotNullModel<T>(T arg, string argName)
            where T : Models.IModel
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, $"Model: {argName} cannot be null");
            }
        }

        public static bool IsNotNullOrEmpty<T>(IEnumerable<T> arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, $"{argName} cannot be null");
            }
            foreach (var _ in arg)
            {
                return true;
            }
            throw new ArgumentNullException(argName, $"{argName} cannot be empty");
        }
        public static bool IsFailure<T>(Azure.Response<T> response)
            => IsFailure(response.GetRawResponse());
        public static bool IsFailure(Azure.Response response)
            => !IsSuccess(response);
        public static bool IsSuccess<T>(Azure.Response<T> response)
            => IsSuccess(response.GetRawResponse());
        public static bool IsSuccess(Azure.Response response)
            => response.Status >= 200 && response.Status <= 299;

        public async static Task<IEnumerable<T>> AsyncToList<T>(IAsyncEnumerable<T> e)
            => await AsyncToList(e.GetAsyncEnumerator());

        public async static Task<IEnumerable<T>> AsyncToList<T>(IAsyncEnumerator<T> e)
        {
            IList<T> result = new List<T>();
            try
            {
                while (await e.MoveNextAsync())
                {
                    result.Add(e.Current);
                }
            }
            finally
            {
                if (e != null)
                {
                    await e.DisposeAsync();
                }
            }
            return result;
        }
    }
}
