using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonelBlogSite.Models
{
    public class Mail
    {
        [Required]
        public string Konu { get; set; }
        [Required]
        public string Mesaj { get; set; }
        [Required]
        public string FromName { get; set; }
        [Required]
        public string Email { get; set; }


    }
}
