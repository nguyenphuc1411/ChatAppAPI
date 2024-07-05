using ChatAppAPI.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatAppAPI.Models
{
    public class RoomVM
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [MinLength(5, ErrorMessage ="Roomname min lenght is 5 letters"),
            MaxLength(100, ErrorMessage = "Roomname max lenght is 100 letters")]
      
        public string RoomName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string AdminId {  get; set; }
        public string? AdminName {  get; set; }
        public string? Avatar {  get; set; }
    }
}
