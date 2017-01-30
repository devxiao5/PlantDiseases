using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantDiseases.Models
{
    public class TargetLink
    {
        [Key]
        public int TargetLinkId { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }
        public bool Done { get; set; }
    }
}
