namespace YouTubeClone.Domain.Aggregates.Users
{
    public class UserCredentials
    {
        public string Username { get; private set; }
        public string HashedPasswords { get; private set; }
        public string Salt { get; private set; }
        public string MultiFactorTokens { get; private set; }

        // EF Core requires a parameterless constructor
        private UserCredentials() { }

        public UserCredentials(string username, string hashedPasswords, string salt, string multiFactorTokens)
        {
            Username = username;
            HashedPasswords = hashedPasswords;
            Salt = salt;
            MultiFactorTokens = multiFactorTokens;
        }
    }
}
