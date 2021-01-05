using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BulletinBoard.Models
{
    [MetadataType(typeof(Post_Validation))]
    public partial class Post
    {
        //placeholder
    }

    public class Post_Validation
    {
        [Required]
        [MinLength(2)]
        public string message { get; set; }
    }
}