namespace WeBlog.Configuration
{
    public class MailSettings
    {
        // used to configure and use smtp server

        public string MailAddress { get; set; }
        public string DisplayName { get; set; }
        public string MailPassword { get; set; }
        public string MailHost { get; set; }
        public int MailPort { get; set; }
    }
}
