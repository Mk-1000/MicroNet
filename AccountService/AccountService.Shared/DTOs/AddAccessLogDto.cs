using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Shared.DTOs
{
    public class AddAccessLogDto
    {
        public string AccessType { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string UserAgent { get; set; }
        public string Location { get; set; }
    }
}
