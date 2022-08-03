namespace WeBlog.Services.Interfaces
{
    public interface IConnectionService
    {
        public string GetConnectionString(IConfiguration configuration);
        public string BuildConnectionString(string databaseUrl);
    }
}
