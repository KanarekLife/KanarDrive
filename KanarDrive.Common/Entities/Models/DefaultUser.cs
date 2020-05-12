namespace KanarDrive.Common.Entities.Models
{
    public class DefaultUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int AvailableCloudSpaceInGbs { get; set; }
    }
}