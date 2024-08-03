using Domain.Common;


namespace Domain.Entities
{
    public class Setting :BaseEntity
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }
}
