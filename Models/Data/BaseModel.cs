using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime Inserted { get; set; }
        public DateTime Updated { get; set; }
        public int OrganisationId { get; set; }
    }
}