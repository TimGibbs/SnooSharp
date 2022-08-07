namespace SnooSharp;

internal class AuthPayload
{
    public AuthPayload(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }
    public string Password { get; }
}