using System.Collections.Concurrent;

namespace IgPanelTelegramBot.Models;
internal class UserSession
{
    internal static ConcurrentDictionary<long, UserSession> _userSessions = new();

    internal static void UpdateOrCreate(long userId, UserSession userSession)
    {
        _userSessions.AddOrUpdate(
            userId,
            userSession,
            (key, existingValue) => userSession
        );
    }

    internal static UserSession? Get(long userId)
    {
        _userSessions.TryGetValue(userId, out var userSession);
        return userSession;
    }

    internal static bool Remove(long userId)
    {
        return _userSessions.TryRemove(userId, out _);
    }

    internal static bool Exists(long userId)
    {
        return _userSessions.ContainsKey(userId);
    }

    internal static IEnumerable<KeyValuePair<long, UserSession>> GetAll()
    {
        return _userSessions;
    }

    public string ServiceId {  get; private set; }

    internal UserSession(string serviceId)
    {
        ServiceId = serviceId;
    }

    public string Link { get; set; } = string.Empty;
}