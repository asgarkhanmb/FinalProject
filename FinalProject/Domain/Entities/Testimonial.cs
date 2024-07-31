﻿using Domain.Common;


namespace Domain.Entities
{
    public class Testimonial :BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }


    }
}
