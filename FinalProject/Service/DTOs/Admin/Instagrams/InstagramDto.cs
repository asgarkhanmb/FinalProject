using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Admin.Instagrams
{
    public class InstagramDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SocialName { get; set; }
        public List<string> Images { get; set; }
    }

}
