namespace API.JwtToken.Service
{
    public interface IRefreshHandler
    {
        Task<string> GenerateToken(string username);

    }
}
