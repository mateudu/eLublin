//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace eLublin.Web.Models.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class Report
    {
        public int id { get; set; }
        public Nullable<int> userInfoId { get; set; }
        public Nullable<int> glosy { get; set; }
        public string tekst { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        public string imagePath { get; set; }
        public byte[] imageStream { get; set; }
        public Nullable<System.DateTime> timeAdded { get; set; }
        
        [JsonIgnore]
        public virtual UserInfo UserInfo { get; set; }
    }
}
