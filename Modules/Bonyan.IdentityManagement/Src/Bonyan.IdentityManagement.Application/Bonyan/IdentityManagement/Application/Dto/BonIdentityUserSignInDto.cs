namespace Bonyan.IdentityManagement.Application.Dto;

public class BonIdentityUserJwtSignInDto
{
    public string Username { get; set; }
    public string Password { get; set; }


}
public class BonIdentityUserCookieSignInDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsPersistent { get; set; }

}

public class BonIdentityUserRefreshTokenDto
{
    public string RefreshToken { get; set; }
   
}