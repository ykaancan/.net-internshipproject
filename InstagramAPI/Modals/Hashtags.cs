using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramAPI.Modals
{
    public class Datum5
    {
        public string id { get; set; }
    }

    public class Root
    {
        public List<Datum5> data { get; set; }
        public string Hashtag { get; set; }
        public string Mail { get; set; }
    }

}
