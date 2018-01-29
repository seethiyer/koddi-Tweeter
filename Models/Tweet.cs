using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweeter.Models
{
    public class Tweet
    {
        public Tweet()
        {
            CreatedOn = DateTime.UtcNow;
        }

        public int ID { get; set; }

        public string OwnerID { get; set; }

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
