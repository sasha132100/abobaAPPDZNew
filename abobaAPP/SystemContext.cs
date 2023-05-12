using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace abobaAPP
{
    class SystemContext
    {
        public static User user { get; set; }
        public static Product product { get; set; }
        public static bool isGuest { get; set; }
        public static bool isEditing { get; set; }
        public static int countInBucket { get; set; } = 0;
        public static List<Product> bucketList { get; set; } = new List<Product>();
    }
}
