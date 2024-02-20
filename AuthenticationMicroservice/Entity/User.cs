namespace AuthenticationMicroservice.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool HasReadAccess { get; set; } = true;
        public bool HasCreateAccess { get; set; } = true;
        public bool HasWriteAccess { get; set; } = true;
        public bool HasDeleteAccess { get; set; } = true;
    }
}
