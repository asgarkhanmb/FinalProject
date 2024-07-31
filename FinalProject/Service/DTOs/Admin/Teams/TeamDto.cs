using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Admin.Teams
{
    public class TeamDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Image { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public List<string> SocialNames { get; set; }
    }
}
