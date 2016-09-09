using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLublin.Web.Models.Api
{
    public class CommitNotification
    {
        public string tekst { get; set; }
        [Required]
        public string tytul { get; set; }
        public string url { get; set; }
    }
}
