using System;
using System.Collections.Generic;
using System.Text;

namespace Albert.Model
{
    class BuildConfig
    {
        public Config Pop3Config { get; set; }
        public Config Imap4Config { get; set; }
        public Config CsoConfig { get; set; }
        public string PersonalAccessToken { get; set; }
    }
}
