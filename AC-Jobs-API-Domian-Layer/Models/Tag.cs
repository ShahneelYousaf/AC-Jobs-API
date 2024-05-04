using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class Tag :BaseEntity
    {
        public string Name { get; set; }
        public long? JobOrEventId { get; set; }
        public bool IsJobsTags { get; set; }

    }
}
