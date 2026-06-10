  public class PlaylistWithItemsSpecification : SoftBridge.Services.Specification.BaseSpecification<Playlist, PlaylistId>
    {
        public PlaylistWithItemsSpecification(PlaylistId playlistId) : base(p => p.Id.Value == playlistId.Value)
        {
            AddInclude("VideoItems");
        }
    }