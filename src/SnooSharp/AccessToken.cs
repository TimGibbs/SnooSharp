namespace SnooSharp;

internal class AccessToken
{
    public AccessToken(TokenResponse tokenResponse)
    {
        Token = tokenResponse.access_token 
                ?? throw new ArgumentException("token should not be null");
        ValidTill = (DateTime.Now + TimeSpan.FromSeconds(tokenResponse.expires_in)).AddSeconds(-5);
    }
    public string Token { get; }
    public DateTime ValidTill { get; }
}