public interface ITokenRepository
{
    Task SaveTokenAsync(string userId, string token);
    Task<List<string>> GetTokensForUserAsync(string userId);
}

public class InMemoryTokenRepository : ITokenRepository
{
    private readonly Dictionary<string, List<string>> _tokens = new();

    public async Task SaveTokenAsync(string userId, string token)
    {
        if (!_tokens.ContainsKey(userId))
            _tokens[userId] = new List<string>();
        if (!_tokens[userId].Contains(token))
            _tokens[userId].Add(token);
        await Task.CompletedTask;
    }

    public async Task<List<string>> GetTokensForUserAsync(string userId)
    {
        return await Task.FromResult(_tokens.ContainsKey(userId) ? _tokens[userId] : new List<string>());
    }
}