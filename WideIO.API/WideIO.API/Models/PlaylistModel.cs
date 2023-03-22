using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideIO.Models
{
    public class PlaylistModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "please name your playlist")]
        public string Name { get; set; }

        public IEnumerable<VideoModel> Videos { get; set; }


    }
}
