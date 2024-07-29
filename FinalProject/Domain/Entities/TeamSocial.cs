
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TeamSocial
    {
        public int Id { get; set; }
        public Team Team { get; set; }
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        public Social Social { get; set; } 
        [ForeignKey(nameof(Social))]
        public int SocialId { get; set; }
        public string Url { get; set; }
    }
}
