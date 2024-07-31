

namespace Domain.Entities
{
    public class InstagramGallery 
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int InstagramId { get; set; }
        public Instagram Instagram { get; set; }

    }
}
