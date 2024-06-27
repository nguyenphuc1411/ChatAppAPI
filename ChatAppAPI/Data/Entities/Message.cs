using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.Data.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
        public string SenderId { get; set; }
        public ManageUser Sender { get; set; }


        public string RecipientId { get; set; }
        public ManageUser Recipient { get; set; }


        public int? RoomId { get; set; }
        public Room Room { get; set; }
    }
}
