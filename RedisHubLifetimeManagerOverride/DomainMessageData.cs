namespace RedisHubLifetimeManagerOverride;

public enum DataCategory {
    Alpha,
    Bravo,
    Charlie
}

public sealed record DomainMessageData(string Id, DataCategory Category, int Level, string SensitiveData);