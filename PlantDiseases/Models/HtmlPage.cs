using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantDiseases.Models
{
    public class HtmlPage
    {
        [Key]
        public int DetailId { get; set; }
        [StringLength(700)]
        public string Url { get; set; }
        [StringLength(700)]
        public string Title { get; set; }
        [StringLength(700)]
        public string Subject { get; set; }
        [StringLength(700)]
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
