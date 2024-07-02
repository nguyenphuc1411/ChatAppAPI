using ChatAppAPI.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.Models
{
    public class MessageVM
    {
        [Required]
        public string Content { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string? FullName { get; set; }
        public string? Email {  get; set; }
        public string? Avatar { get; set; }
        public int RoomId { get; set; }
    }
}
