using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.Data.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string SenderId { get; set; }
        public ManageUser Sender { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
