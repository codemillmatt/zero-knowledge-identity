using System;
using System.Collections.Generic;

namespace Reviewer.SharedModels
{
    public class Business
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public string Phone { get; set; }

        public List<Review> RecentReviews { get; set; }

        public string FirstInitial { get => Name.Substring(0, 1); }
    }
}
