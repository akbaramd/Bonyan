namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityJwtResultDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public TimeSpan Expired { get; set; }
}   