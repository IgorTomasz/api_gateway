namespace api_gateway.models.DTOs
{
	public class UserProfileResponse
	{
		public bool Success { get; set; }
		public string? Error { get; set; }
		public UserResponse User { get; set; }
	}

	public class UserResponse
	{
		public Guid UserId { get; set; }
		public string Username { get; set; }
		public string Name { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? LastLogin { get; set; }
		public UserType UserType { get; set; }
	}


	public enum UserType
	{
		Client, Admin
	}
}
