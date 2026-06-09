using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.Aggregates.WatchHistories;
using YouTubeClone.Domain.Aggregates.Subscriptions;

namespace YouTubeClone.Domain.Aggregates.Users
{
    public class User : AggregateRoot<UserId>
    {
        public UserCredentials Credentials { get; private set; }
        public UserProfileInfo ProfileInfo { get; private set; }
        public WatchHistory WatchHistory { get; private set; }
        public LikedVideosPlaylist LikedVideosPlaylist { get; private set; }
        public Subscriptions Subscriptions { get; private set; }
        public Channel? Channel { get; private set; }

        private readonly List<CustomPlaylist> _customPlaylists = new();
        public IReadOnlyList<CustomPlaylist> CustomPlaylists => _customPlaylists.AsReadOnly();

        public User(
            UserId id,
            UserCredentials credentials,
            UserProfileInfo profileInfo,
            WatchHistory watchHistory,
            LikedVideosPlaylist likedVideosPlaylist,
            Subscriptions subscriptions,
            Channel? channel = null) : base(id)
        {
            Credentials = credentials;
            ProfileInfo = profileInfo;
            WatchHistory = watchHistory;
            LikedVideosPlaylist = likedVideosPlaylist;
            Subscriptions = subscriptions;
            Channel = channel;
        }

        public void AssignChannel(Channel channel)
        {
            Channel = channel;
        }

        public void AddCustomPlaylist(CustomPlaylist playlist)
        {
            _customPlaylists.Add(playlist);
        }
    }
}
