using System.Text;
using WeBlog.Data;
using WeBlog.Services.Interfaces;

namespace WeBlog.Services
{
    public class DefaultSlugService : ISlugService
    {

        private readonly ApplicationDbContext _dbContext;

        public DefaultSlugService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsUnique(string slug)
        {
            return !_dbContext.Posts.Any(p => p.Slug == slug);
        }

        public string UrlFriendly(string title)
        {
            if (title == null)
            {
                return "";
            }

            const int maxlen = 80;
            var len = title.Length;
            var prevdash = false;

            var stringBuilder = new StringBuilder(len);

            char c;
            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    stringBuilder.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    stringBuilder.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                        c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                    }
                }
                else if (c == '#')
                {
                    if (i > 0)
                    {
                        if (title[i - 1] == 'C' || title[i - 1] == 'F')
                        {
                           stringBuilder.Append("-sharp");
                        } 
                    }
                }
                else if (c == '+')
                {
                    stringBuilder.Append("-plus");
                }
                else if ((int)c >= 128)
                {
                    int prevlen = stringBuilder.Length;
                    stringBuilder.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != stringBuilder.Length) prevdash = false;
                }
                if (stringBuilder.Length == maxlen)
                {
                    break;
                }
            }

            if (prevdash)
            {
                return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }
            else
            {
                return stringBuilder.ToString();
            }
        }

        private string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}
