namespace YouTubeClone.Domain.Aggregates.Users
{
    public class UserProfileInfo
    {
        public string UserActualName { get; private set; }
        public string Bio { get; private set; }
        public string Avatar { get; private set; }
        public string ThemePreference { get; private set; }
        public string TopicsInterestedIn { get; private set; }

        public UserProfileInfo(string userActualName, string bio, string avatar, string themePreference, string topicsInterestedIn)
        {
            UserActualName = userActualName;
            Bio = bio;
            Avatar = avatar;
            ThemePreference = themePreference;
            TopicsInterestedIn = topicsInterestedIn;
        }
    }
}
