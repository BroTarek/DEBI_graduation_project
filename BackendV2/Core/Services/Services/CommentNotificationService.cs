using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Services
{
    public interface ICommentNotificationService
    {
        void NotifyCommentAuthorOfReply(Comment parentComment, Comment reply);
        void NotifyVideoOwnerOfComment(Video video, Comment comment);
    }

    public class CommentNotificationService : ICommentNotificationService
    {
        public void NotifyCommentAuthorOfReply(Comment parentComment, Comment reply)
        {
            // Logic to send notification
        }

        public void NotifyVideoOwnerOfComment(Video video, Comment comment)
        {
            // Logic to send notification
        }
    }
}
