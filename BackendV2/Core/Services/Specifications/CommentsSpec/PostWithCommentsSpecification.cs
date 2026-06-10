 public class PostWithCommentsSpecification : SoftBridge.Services.Specification.BaseSpecification<Post, PostId>{
        public PostWithCommentsSpecification(Guid postId) : base(p => p.Id.Value == postId.Value)
        {
            AddInclude("Comments");
            AddInclude("Comments.Replies");
        }
    }