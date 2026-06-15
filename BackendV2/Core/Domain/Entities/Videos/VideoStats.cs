namespace YouTubeClone.Domain.Entities.Videos
{
    public class VideoStats
    {
        public int watchCount { get; set; }
        public int likesCount { get; set; }
        public int dislikesCount { get; set; }

        public VideoStats() { }
        public VideoStats(int watchCount, int likesCount, int dislikesCount)
        {
            this.watchCount = watchCount;
            this.likesCount = likesCount;
            this.dislikesCount = dislikesCount;
        }
    }
}
