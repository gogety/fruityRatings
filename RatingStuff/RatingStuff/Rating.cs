using System;
using System.Collections.Generic;
using System.Text;

namespace RatingStuff
{
    public class Rating
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productid { get; set; }
        public int rating { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public string locationName { get; set; }
        public string userNotes { get; set; }
    }
}
