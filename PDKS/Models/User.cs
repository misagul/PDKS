namespace PDKS.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
        public int Shift { get; set; }
        public User()
        {
            IsActive = true;
            Shift = 0;
        }
    }
}
