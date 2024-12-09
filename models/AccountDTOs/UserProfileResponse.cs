namespace api_gateway.models.DTOs
{
	public class UserProfileResponse
	{
		public Guid userId {  get; set; }
		public string username { get; set; }
		public string name { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }	
		public DateOnly dateOfBirth { get; set; }
		public bool isActive { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime? lastLogin { get; set; }
		public UserType userType { get; set; }
	}


	public enum UserType
	{
		Client, Admin
	}
}
