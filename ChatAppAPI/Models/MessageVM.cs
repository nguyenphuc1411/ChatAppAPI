using ChatAppAPI.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.Models
{
    public class MessageVM
    {
        [Required]
        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
        public string From {  get; set; }
        public string Avatar { get; set; }
        public string Room { get; set; }
    }
}
