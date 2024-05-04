using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class WorkFlow : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? JobId { get; set; }

    }
}
