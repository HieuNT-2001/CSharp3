using Newtonsoft.Json;

namespace WebAppMVC.Data
{
    public static class SessionExtensions
    {
        public static T? GetComplexData<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetComplexData<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
