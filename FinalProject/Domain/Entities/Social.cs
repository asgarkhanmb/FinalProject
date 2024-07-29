using Domain.Common;


namespace Domain.Entities
{
    public class Social :BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        List<TeamSocial> TeamSocials { get; set; }

    }
}
