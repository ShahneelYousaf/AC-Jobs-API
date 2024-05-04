﻿using AC_Jobs_API_Domian_Layer.ENums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ac.Jobs.API.DTos
{
    public class DTOEvents
    {
        public long Id { get; set; }
        public string EventType { get; set; }
        public string EventPriority { get; set; }
        [MaxLength(50)]
        public string EventName { get; set; }
        public string EventStatus { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(50)]
        public string EstimatedDuration { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(200)]
        public string Tags { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        public long? JobId { get; set; }

    }
}
