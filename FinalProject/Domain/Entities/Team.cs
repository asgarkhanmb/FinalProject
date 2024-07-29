using Domain.Common;


namespace Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        List<TeamSocial> TeamSocials { get; set; }  
    }
}
