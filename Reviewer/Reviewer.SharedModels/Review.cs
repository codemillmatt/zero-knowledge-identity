using System;
using System.Collections.Generic;
namespace Reviewer.SharedModels
{
    public class Review
    {
        public Review()
        {
        }

        public string Id { get; set; }

        public string BusinessId { get; set; }

        public string Author { get; set; }

        public string ReviewText { get; set; }

        public DateTime Date { get; set; }

        public List<string> Photos { get; set; }

        public int Rating { get; set; }
    }
}
