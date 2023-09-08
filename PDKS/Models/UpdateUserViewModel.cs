namespace PDKS.Models
{
	public class UpdateUserViewModel
	{
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int Shift { get; set; }
        public bool IsActive { get; set; }
    }
}
