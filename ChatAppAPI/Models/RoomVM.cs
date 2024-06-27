using ChatAppAPI.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAppAPI.Models
{
    public class RoomVM
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [MinLength(5, ErrorMessage ="Roomname min lenght is 5 letters"),
            MaxLength(100, ErrorMessage = "Roomname max lenght is 100 letters")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "RoomName must contain only letters and digits")]
        public string RoomName { get; set; }

        public string AdminId {  get; set; }
    }
}
