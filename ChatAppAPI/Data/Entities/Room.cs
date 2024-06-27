using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAppAPI.Data.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string RoomName { get; set; }


        public string AdminId { get; set; }
        public ManageUser Admin { get; set; }

        public DateTime CreatedDate { get; set; }


        public ICollection<Message> Messages { get; set; }
    }

}
