using System;
using Shared.Common.Entities;

namespace UserService.Core.Entities
{
    /// <summary>
    /// Extended user profile information
    /// </summary>
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Thailand";
        public string LinkedInUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Occupation { get; set; }
        public string Company { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}