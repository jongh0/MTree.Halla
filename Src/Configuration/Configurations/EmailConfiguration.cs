using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class EmailConfiguration
    {
        [JsonIgnore]
        public string UserId { get; set; } = "mtree.halla@gmail.com";

        [JsonIgnore]
        public string UserPw { get; set; } = "ajslxmfl"; // 머니트리

        public string[] MailTo { get; set; } = { "mtree.halla@gmail.com" };
    }
}
