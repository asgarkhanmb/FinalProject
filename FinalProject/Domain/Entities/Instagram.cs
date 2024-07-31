using Domain.Common;


namespace Domain.Entities
{
    public class Instagram :BaseEntity
    {
        public string Title { get; set; }
        public string SocialName { get; set; }
        public List<InstagramGallery> InstagramGalleries { get; set; }
    }
}
