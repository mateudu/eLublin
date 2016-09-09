using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLublin.Web.Models.Api
{
    public class CommitReport
    {
        public string tekst { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public byte[] image { get; set; }
    }
}
