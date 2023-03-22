<<<<<<< HEAD:MyWideIO.API/MyWideIO.API/Models/CommentModel.cs
﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class CommentModel 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        [Required]
        public ViewerModel Author { get; set; }

        public IEnumerable<CommentModel> Replies { get; set;}

    }
}
=======
﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyVideIO.Models
{
    public class CommentModel 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        [Required]
        public ViewerModel Author { get; set; }

        public IEnumerable<CommentModel> Replies { get; set;}

    }
}
>>>>>>> origin/develop:WideIO.API/WideIO.API/Models/CommentModel.cs
