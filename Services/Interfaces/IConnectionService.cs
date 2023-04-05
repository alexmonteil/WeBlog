namespace WeBlog.Services.Interfaces
{
    public interface IConnectionService
    {
        string GetConnectionString(IConfiguration configuration);
        string BuildConnectionString(string databaseUrl);
    }
}
