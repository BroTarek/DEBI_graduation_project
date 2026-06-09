using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.Playlists;

namespace YouTubeClone.Domain.Aggregates.Channels
{
    public class Channel : AggregateRoot<ChannelId>
    {
        public User Owner { get; private set; }
        public ChannelProfile Profile { get; private set; }

        private readonly List<Post> _posts = new();
        public IReadOnlyList<Post> Posts => _posts.AsReadOnly();

        private readonly List<Video> _videos = new();
        public IReadOnlyList<Video> Videos => _videos.AsReadOnly();

        private readonly List<ChannelPlaylist> _channelPlaylists = new();
        public IReadOnlyList<ChannelPlaylist> ChannelPlaylists => _channelPlaylists.AsReadOnly();

        public Channel(ChannelId id, User owner, ChannelProfile profile) : base(id)
        {
            Owner = owner;
            Profile = profile;
        }

        public void AddPost(Post post)
        {
            _posts.Add(post);
        }

        public void AddVideo(Video video)
        {
            _videos.Add(video);
        }

        public void AddPlaylist(ChannelPlaylist playlist)
        {
            _channelPlaylists.Add(playlist);
        }
    }
}
