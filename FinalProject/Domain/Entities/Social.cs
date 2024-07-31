using Domain.Common;


namespace Domain.Entities
{
    public class Social :BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }

    }
}
