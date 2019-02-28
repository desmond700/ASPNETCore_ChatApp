using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int? Group_id { get; set; }
        public string Message_text { get; set; }
        public string Date_sent { get; set; }
        public string Image { get; set; }
    }
}
