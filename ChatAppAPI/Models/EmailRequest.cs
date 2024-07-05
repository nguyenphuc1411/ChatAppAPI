using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.Models
{
    public class EmailRequest
    {
        [DataType(DataType.EmailAddress)]
        public string To { get; set; }
        public string Subject { get; set; }
        public string ResetLink { get; set; }
    }
}
