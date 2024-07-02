namespace ChatAppAPI.Models
{
    public class UserVM
    {
        public string Email {  get; set; }
        public string FullName { get; set; }

        public string? Avatar { get; set; }

        public string  CurrentGroup { get; set; }
    }
}
