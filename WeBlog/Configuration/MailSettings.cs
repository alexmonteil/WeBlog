namespace WeBlog.Configuration
{
    public class MailSettings
    {
        // used to configure and use smtp server

        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
