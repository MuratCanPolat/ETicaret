using System.Text.Json;

namespace E_Ticaret.Extensions //TODO Projenin bazı kısımlarında namespace isimleri E_Ticaret yerine ETicaretWeb olarak geçiyor. Kafa karışılığını önlemek adına web demiştim ama tutarlılık açısından düzeltilmeli.
{
    public static class SessionExtensions
    {
        // Nesneyi JSON'a çevirip Session'a kaydeder.
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Session'daki JSON verisini okuyup tekrar nesneye çevirir.
        public static T? GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null ? default(T) : JsonSerializer.Deserialize<T>(sessionData);
        }
    }
}