﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class TeamMembers : BaseEntity
    {
        public long? JobId { get; set; }
        public long? WorkOrderId { get; set; }
        public long TeamMemberId { get; set; }
    }
}
