namespace api_gateway.models.DTOs
{
	public class AdmUsersAll
	{
		public List<UserProfileResponse> Users { get; set; }
		public string IpAddress { get; set; }
	}
}
