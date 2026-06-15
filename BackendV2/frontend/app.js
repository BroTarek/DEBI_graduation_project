// Base URL for the API
const API_BASE_URL = 'http://localhost:8080';

// Hardcoded user IDs for the seeded users to bypass auth
const TEST_USER_ID = '22222222-2222-2222-2222-222222222222';
const TEST_CHANNEL_ID = '22222222-2222-2222-2222-222222222223'; // Using seeded channel ID

/**
 * Fetch and render videos on the home page
 */
async function fetchVideos() {
    const grid = document.getElementById('video-grid');
    if (!grid) return;

    try {
        const response = await fetch(`${API_BASE_URL}/HomePageVideos?skip=0&take=20`);
        if (!response.ok) throw new Error('Failed to fetch videos');
        
        const data = await response.json();
        
        if (data.data && data.data.length > 0) {
            grid.innerHTML = data.data.map(video => createVideoCard(video)).join('');
        } else {
            grid.innerHTML = '<div class="loading">No videos found. Be the first to upload!</div>';
        }
    } catch (error) {
        console.error('Error fetching videos:', error);
        grid.innerHTML = `<div class="loading" style="color: #f44336;">Error loading videos. Ensure the backend is running.</div>`;
    }
}

function getDisplayName(nameOrId) {
    return nameOrId === TEST_CHANNEL_ID ? 'Test Channel' : (nameOrId && nameOrId.length > 20 ? 'Channel ' + nameOrId.substring(0, 5) : nameOrId || 'Unknown Channel');
}

function goToChannel(event, channelId) {
    event.stopPropagation();
    window.location.href = `channel.html?id=${channelId}`;
}

/**
 * Create HTML string for a single video card
 */
function createVideoCard(video) {
    const timeAgo = getTimeAgo(new Date(video.uploadDate));
    
    // Direct card click opens watch.html player details view in same tab
    const link = `watch.html?id=${video.videoId}`;
    const dispName = getDisplayName(video.channelName);
    
    return `
        <div class="video-card" onclick="window.location.href='${link}'">
            <div class="thumbnail-container">
                <img src="${video.thumbnailUrl || 'https://via.placeholder.com/640x360?text=No+Thumbnail'}" alt="Thumbnail" class="thumbnail">
                <div class="video-duration">${video.duration}</div>
            </div>
            <div class="video-info">
                <img src="https://ui-avatars.com/api/?name=${dispName}&background=random" alt="Channel" class="channel-avatar" onclick="goToChannel(event, '${video.channelName}')" style="cursor: pointer;">
                <div class="video-details" style="position: relative; width: 100%;">
                    <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 8px;">
                        <h3 class="video-title" title="${video.title}" style="margin: 0; flex: 1;">${video.title}</h3>
                        <button class="video-options-btn" onclick="showVideoOptions(event, '${video.videoId}', 'custom')" style="background: none; border: none; color: white; cursor: pointer; font-size: 18px; padding: 4px 8px; border-radius: 50%; outline: none; margin-top: -4px;">⋮</button>
                    </div>
                    <div class="video-meta">
                        <div onclick="goToChannel(event, '${video.channelName}')" style="cursor: pointer; font-weight: 500; display: inline-block;">${dispName}</div>
                        <div>${video.views} views • ${timeAgo}</div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

/**
 * Handle the upload form submission
 */
async function handleUpload(event) {
    event.preventDefault();
    
    const form = event.target;
    const submitBtn = document.getElementById('upload-btn');
    const statusDiv = document.getElementById('upload-status');
    
    // Prepare form data
    const formData = new FormData(form);
    // Add default duration for stub
    formData.append('DurationSeconds', '120'); 
    
    try {
        // UI Loading state
        submitBtn.disabled = true;
        submitBtn.textContent = 'Uploading... (This may take a minute)';
        statusDiv.className = 'status-message status-loading';
        statusDiv.textContent = 'Uploading video and thumbnail to Cloudinary...';

        const response = await fetch(`${API_BASE_URL}/upload?channelId=${TEST_CHANNEL_ID}`, {
            method: 'POST',
            body: formData
        });

        const data = await response.json();

        if (response.ok) {
            statusDiv.className = 'status-message status-success';
            statusDiv.textContent = 'Video uploaded successfully! Redirecting to home...';
            
            // Redirect after 2 seconds
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 2000);
        } else {
            throw new Error(data.message || 'Upload failed');
        }
    } catch (error) {
        console.error('Upload error:', error);
        statusDiv.className = 'status-message status-error';
        statusDiv.textContent = error.message || 'An error occurred during upload. Check console.';
        submitBtn.disabled = false;
        submitBtn.textContent = 'Upload Video';
    }
}

/**
 * Utility: Convert date to "time ago" string
 */
function getTimeAgo(date) {
    const seconds = Math.floor((new Date() - date) / 1000);
    
    let interval = seconds / 31536000;
    if (interval > 1) return Math.floor(interval) + " years ago";
    
    interval = seconds / 2592000;
    if (interval > 1) return Math.floor(interval) + " months ago";
    
    interval = seconds / 86400;
    if (interval > 1) return Math.floor(interval) + " days ago";
    
    interval = seconds / 3600;
    if (interval > 1) return Math.floor(interval) + " hours ago";
    
    interval = seconds / 60;
    if (interval > 1) return Math.floor(interval) + " minutes ago";
    
    return Math.floor(seconds) + " seconds ago";
}

/**
 * Fetch and display channel profile details
 */
async function fetchChannelAbout(channelId) {
    const titleEl = document.getElementById('channel-title');
    const statsEl = document.getElementById('channel-stats');
    const descEl = document.getElementById('channel-desc');
    const linkEl = document.getElementById('channel-link');
    const avatarEl = document.getElementById('channel-avatar');
    const bannerEl = document.getElementById('channel-banner');

    try {
        const response = await fetch(`${API_BASE_URL}/api/channel/${channelId}`);
        if (!response.ok) throw new Error('Channel not found');
        
        const res = await response.json();
        const data = res.data;

        if (titleEl) titleEl.textContent = data.name || 'Unnamed Channel';
        if (statsEl) statsEl.textContent = `@${(data.name || 'channel').toLowerCase().replace(/\s+/g, '')} • ${data.subscribersCount.toLocaleString()} subscribers`;
        if (descEl) descEl.textContent = data.channelsDescription || 'No description available.';
        
        if (linkEl) {
            if (data.links) {
                linkEl.href = data.links;
                linkEl.textContent = data.links;
                linkEl.style.display = 'inline-block';
            } else {
                linkEl.style.display = 'none';
            }
        }

        if (avatarEl && data.avatar) {
            avatarEl.src = data.avatar;
        }

        if (bannerEl && data.greaterImg) {
            bannerEl.innerHTML = `<img src="${data.greaterImg}" alt="Channel Banner">`;
        }
    } catch (error) {
        console.error('Error fetching channel about:', error);
        if (titleEl) titleEl.textContent = 'Error loading channel';
        if (descEl) descEl.textContent = 'Ensure the backend server is running and seeds are initialized.';
    }
}

/**
 * Fetch and display channel videos
 */
async function fetchChannelVideos(channelId) {
    const grid = document.getElementById('channel-video-grid');
    if (!grid) return;

    try {
        const response = await fetch(`${API_BASE_URL}/api/channel/${channelId}/videos`);
        if (!response.ok) throw new Error('Failed to fetch channel videos');
        
        const res = await response.json();
        const data = res.data;

        if (data && data.length > 0) {
            grid.innerHTML = data.map(video => {
                const timeAgo = getTimeAgo(new Date(video.publishDate));
                return `
                    <div class="video-card" onclick="window.location.href='watch.html?id=${video.id}'">
                        <div class="thumbnail-container">
                            <img src="${video.thumbnailURL || 'https://via.placeholder.com/640x360?text=No+Thumbnail'}" alt="Thumbnail" class="thumbnail">
                        </div>
                        <div class="video-info" style="padding-top: 8px;">
                            <div class="video-details" style="position: relative; width: 100%;">
                                <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 8px;">
                                    <h3 class="video-title" title="${video.title}" style="margin: 0; flex: 1;">${video.title}</h3>
                                    <button class="video-options-btn" onclick="showVideoOptions(event, '${video.id}', 'channel')" style="background: none; border: none; color: white; cursor: pointer; font-size: 18px; padding: 4px 8px; border-radius: 50%; outline: none; margin-top: -4px;">⋮</button>
                                </div>
                                <div class="video-meta">
                                    <div>${video.viewCount} views • ${timeAgo}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            }).join('');
        } else {
            grid.innerHTML = '<div class="loading">This channel has no videos yet.</div>';
        }
    } catch (error) {
        console.error('Error fetching channel videos:', error);
        grid.innerHTML = `<div class="loading" style="color: #f44336;">Error loading videos.</div>`;
    }
}

/**
 * Fetch and display channel community posts
 */
async function fetchChannelPosts(channelId) {
    const list = document.getElementById('channel-posts-list');
    if (!list) return;

    try {
        const response = await fetch(`${API_BASE_URL}/api/channels/${channelId}/posts`);
        if (!response.ok) throw new Error('Failed to fetch posts');
        
        const resObj = await response.json();
        const posts = resObj.value || resObj;

        if (posts && posts.length > 0) {
            list.innerHTML = posts.map(post => {
                const postDate = '2 days ago'; 
                return `
                    <div class="post-card">
                        <div class="post-card-header">
                            <img src="https://ui-avatars.com/api/?name=Channel&background=random" alt="Avatar" class="post-avatar" id="post-avatar-img-${post.id}">
                            <div class="post-author-info">
                                <span class="post-author-name" id="post-author-name-${post.id}">Channel Post</span>
                                <span class="post-date">${postDate}</span>
                            </div>
                        </div>
                        <div class="post-body">
                            ${post.postContent}
                        </div>
                        <div class="post-actions">
                            <div class="post-action-btn">👍 12</div>
                            <div class="post-action-btn">👎</div>
                            <div class="post-action-btn" onclick="togglePostComments('${post.id}')">💬 Comments</div>
                        </div>
                        
                        <!-- Collapsible comments section -->
                        <div class="post-comments-section" id="comments-${post.id}" style="display: none;">
                            <div class="comment-input-group">
                                <input type="text" placeholder="Add a comment..." id="input-${post.id}">
                                <button onclick="submitPostComment('${post.id}')">Comment</button>
                            </div>
                            <div class="comments-container-list" id="list-${post.id}">
                                <!-- Loaded dynamically -->
                            </div>
                        </div>
                    </div>
                `;
            }).join('');
            
            setTimeout(() => {
                const chanTitle = document.getElementById('channel-title')?.textContent;
                const chanAvatar = document.getElementById('channel-avatar')?.src;
                posts.forEach(post => {
                    if (chanTitle) {
                        const nameEl = document.getElementById(`post-author-name-${post.id}`);
                        if (nameEl) nameEl.textContent = chanTitle;
                    }
                    if (chanAvatar) {
                        const imgEl = document.getElementById(`post-avatar-img-${post.id}`);
                        if (imgEl) imgEl.src = chanAvatar;
                    }
                });
            }, 500);
        } else {
            list.innerHTML = '<div class="loading">This channel has no posts yet.</div>';
        }
    } catch (error) {
        console.error('Error fetching channel posts:', error);
        list.innerHTML = `<div class="loading" style="color: #f44336;">Error loading community posts.</div>`;
    }
}

// --- Watch Page & Comments Functional Support ---

function getUserDisplayName(authorId) {
    if (authorId === TEST_USER_ID) return 'Test User';
    if (authorId === TEST_CHANNEL_ID) return 'Test Channel';
    return 'User ' + (authorId || '').substring(0, 5);
}

async function loadWatchPage(videoId) {
    try {
        const response = await fetch(`${API_BASE_URL}/watch?videoId=${videoId}`);
        if (!response.ok) throw new Error('Video not found');
        
        const res = await response.json();
        const video = res.data;
        
        const player = document.getElementById('video-player');
        if (player) {
            player.src = video.videoUrl;
        }
        
        const titleEl = document.getElementById('video-title');
        if (titleEl) titleEl.textContent = video.title;
        
        const metaEl = document.getElementById('video-meta');
        if (metaEl) {
            const uploadDate = new Date(video.uploadDate);
            metaEl.textContent = `${video.views.toLocaleString()} views • ${getTimeAgo(uploadDate)}`;
        }
        
        const descEl = document.getElementById('video-description');
        if (descEl) descEl.textContent = video.description || 'No description available.';
        
        loadVideoComments(videoId);
        loadSidebarRecommendations(videoId);
        
    } catch (error) {
        console.error('Error loading watch page:', error);
        const titleEl = document.getElementById('video-title');
        if (titleEl) titleEl.textContent = 'Error loading video details';
    }
}

async function loadVideoComments(videoId) {
    const container = document.getElementById('video-comments-list');
    const countEl = document.getElementById('comments-count');
    if (!container) return;

    try {
        const response = await fetch(`${API_BASE_URL}/api/comment/video/${videoId}`);
        if (!response.ok) throw new Error('Failed to fetch video comments');
        
        const comments = await response.json();
        
        const totalComments = countTotalComments(comments);
        if (countEl) {
            countEl.textContent = `${totalComments} Comment${totalComments === 1 ? '' : 's'}`;
        }

        if (comments && comments.length > 0) {
            container.innerHTML = comments.map(comment => renderCommentItem(comment, videoId, 'video')).join('');
        } else {
            container.innerHTML = '<div class="loading">No comments yet. Be the first to share your thoughts!</div>';
        }
    } catch (error) {
        console.error('Error loading video comments:', error);
        container.innerHTML = '<div style="color: #f44336;">Failed to load comments.</div>';
    }
}

function countTotalComments(comments) {
    let count = 0;
    if (!comments) return count;
    for (const c of comments) {
        count++;
        if (c.replies) {
            count += countTotalComments(c.replies);
        }
    }
    return count;
}

async function loadPostComments(postId) {
    const container = document.getElementById(`list-${postId}`);
    if (!container) return;

    try {
        const response = await fetch(`${API_BASE_URL}/api/comment/post/${postId}`);
        if (!response.ok) throw new Error('Failed to fetch post comments');
        
        const comments = await response.json();

        if (comments && comments.length > 0) {
            container.innerHTML = comments.map(comment => renderCommentItem(comment, postId, 'post')).join('');
        } else {
            container.innerHTML = '<div class="loading" style="font-size: 13px;">No comments yet.</div>';
        }
    } catch (error) {
        console.error('Error loading post comments:', error);
        container.innerHTML = '<div style="color: #f44336; font-size: 13px;">Failed to load comments.</div>';
    }
}

function renderCommentItem(comment, targetId, type) {
    const displayName = getUserDisplayName(comment.authorId);
    const avatar = `https://ui-avatars.com/api/?name=${encodeURIComponent(displayName)}&background=random`;
    const repliesHtml = comment.replies && comment.replies.length > 0 
        ? `<div class="replies-container">
            ${comment.replies.map(reply => renderCommentItem(reply, targetId, type)).join('')}
           </div>`
        : '';

    return `
        <div class="comment-wrapper" id="comment-wrapper-${comment.id}">
            <div class="comment-item">
                <img src="${avatar}" class="comment-avatar" alt="Avatar">
                <div class="comment-item-body">
                    <div class="comment-item-header">
                        <span class="comment-author">${displayName}</span>
                        <span class="comment-date">${new Date(comment.createdAt).toLocaleDateString()}</span>
                    </div>
                    <div class="comment-content">${comment.content}</div>
                    <div class="comment-actions">
                        <div class="comment-action-btn" onclick="showReplyInput('${comment.id}')">💬 Reply</div>
                        ${comment.authorId === TEST_USER_ID ? `<div class="comment-action-btn" style="color: #ff4d4d;" onclick="deleteComment('${comment.id}', '${targetId}', '${type}')">🗑️ Delete</div>` : ''}
                    </div>
                    
                    <!-- Hidden Reply Input Group -->
                    <div class="comment-input-group reply-input-group" id="reply-form-${comment.id}" style="display: none; margin-top: 10px;">
                        <input type="text" placeholder="Reply to this comment..." id="reply-text-${comment.id}">
                        <button onclick="submitReply('${comment.id}', '${targetId}', '${type}')">Reply</button>
                        <button class="btn-cancel" onclick="hideReplyInput('${comment.id}')" style="background: none; border: none; color: white; cursor: pointer; margin-left: 8px;">Cancel</button>
                    </div>
                </div>
            </div>
            ${repliesHtml}
        </div>
    `;
}

function showReplyInput(commentId) {
    const el = document.getElementById(`reply-form-${commentId}`);
    if (el) el.style.display = 'flex';
}

function hideReplyInput(commentId) {
    const el = document.getElementById(`reply-form-${commentId}`);
    if (el) el.style.display = 'none';
}

async function submitComment(targetId, type, parentId, text, callback) {
    const isVideo = type === 'video';
    const endpoint = isVideo ? `${API_BASE_URL}/api/comment/video` : `${API_BASE_URL}/api/comment/post`;
    const payload = {
        userId: TEST_USER_ID,
        content: text,
        parentCommentId: parentId || null
    };
    if (isVideo) {
        payload.videoId = targetId;
    } else {
        payload.postId = targetId;
    }

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-User-Id': TEST_USER_ID
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error('Failed to submit comment');
        
        if (callback) callback();
        
        if (isVideo) {
            loadVideoComments(targetId);
        } else {
            loadPostComments(targetId);
        }
    } catch (error) {
        console.error('Error submitting comment:', error);
        alert('Failed to submit comment: ' + error.message);
    }
}

function submitPostComment(postId) {
    const input = document.getElementById(`input-${postId}`);
    if (!input) return;
    const text = input.value.trim();
    if (!text) return;

    submitComment(postId, 'post', null, text, () => {
        input.value = '';
    });
}

function submitReply(commentId, targetId, type) {
    const input = document.getElementById(`reply-text-${commentId}`);
    if (!input) return;
    const text = input.value.trim();
    if (!text) return;

    submitComment(targetId, type, commentId, text, () => {
        input.value = '';
        hideReplyInput(commentId);
    });
}

async function deleteComment(commentId, targetId, type) {
    if (!confirm('Are you sure you want to delete this comment?')) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/api/comment/${commentId}`, {
            method: 'DELETE',
            headers: {
                'X-User-Id': TEST_USER_ID
            }
        });

        if (!response.ok) throw new Error('Failed to delete comment');
        
        if (type === 'video') {
            loadVideoComments(targetId);
        } else {
            loadPostComments(targetId);
        }
    } catch (error) {
        console.error('Error deleting comment:', error);
        alert('Failed to delete comment: ' + error.message);
    }
}

async function loadSidebarRecommendations(currentVideoId) {
    const container = document.getElementById('recommendations-list');
    if (!container) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/HomePageVideos?skip=0&take=10`);
        if (!response.ok) throw new Error('Failed to fetch recommendations');
        
        const res = await response.json();
        const videos = res.data;
        
        if (videos && videos.length > 0) {
            container.innerHTML = videos
                .filter(v => v.videoId !== currentVideoId)
                .map(video => {
                    const timeAgo = getTimeAgo(new Date(video.uploadDate));
                    const link = `watch.html?id=${video.videoId}`;
                    const dispName = getDisplayName(video.channelName);
                    return `
                        <div class="rec-card" onclick="window.location.href='${link}'">
                            <div class="rec-thumbnail-container">
                                <img src="${video.thumbnailUrl || 'https://via.placeholder.com/640x360?text=No+Thumbnail'}" alt="Thumbnail" class="rec-thumbnail">
                            </div>
                            <div class="rec-details">
                                <h4 class="rec-title" title="${video.title}">${video.title}</h4>
                                <div class="rec-meta">
                                    <div>${dispName}</div>
                                    <div>${video.views} views • ${timeAgo}</div>
                                </div>
                            </div>
                        </div>
                    `;
                }).join('');
        } else {
            container.innerHTML = '<div class="loading">No recommendations.</div>';
        }
    } catch (error) {
        console.error('Error loading recommendations:', error);
        container.innerHTML = '<div style="color: #f44336; font-size: 12px;">Failed to load recommendations.</div>';
    }
}

function togglePostComments(postId) {
    const el = document.getElementById(`comments-${postId}`);
    if (!el) return;
    
    if (el.style.display === 'none') {
        el.style.display = 'flex';
        loadPostComments(postId);
    } else {
        el.style.display = 'none';
    }
}

// Bind to window for global inline onclick callbacks
window.showReplyInput = showReplyInput;
window.hideReplyInput = hideReplyInput;
window.submitReply = submitReply;
window.deleteComment = deleteComment;
window.togglePostComments = togglePostComments;
window.submitPostComment = submitPostComment;
window.submitComment = submitComment;
window.loadWatchPage = loadWatchPage;

// --- Playlist Management Operations ---

let activeVideoId = null;
let activePlaylistType = null; // 'custom' or 'channel'

function showVideoOptions(event, videoId, playlistType) {
    event.stopPropagation();
    activeVideoId = videoId;
    activePlaylistType = playlistType;
    
    // Get button element
    const btn = event.currentTarget;
    const rect = btn.getBoundingClientRect();
    
    // Create or locate global dropdown
    let dropdown = document.getElementById('global-video-options-dropdown');
    if (!dropdown) {
        dropdown = document.createElement('div');
        dropdown.id = 'global-video-options-dropdown';
        dropdown.className = 'video-options-dropdown';
        document.body.appendChild(dropdown);
    }
    
    dropdown.innerHTML = `
        <div class="dropdown-item" onclick="openAddToPlaylistModal()">📁 Add to playlist</div>
    `;
    
    // Position dropdown
    dropdown.style.display = 'block';
    dropdown.style.left = `${rect.left + window.scrollX - 120}px`;
    dropdown.style.top = `${rect.bottom + window.scrollY + 5}px`;
    
    // Close dropdown on click outside
    document.addEventListener('click', closeDropdownOutside);
}

function closeDropdownOutside(e) {
    const dropdown = document.getElementById('global-video-options-dropdown');
    if (dropdown && !dropdown.contains(e.target) && !e.target.classList.contains('video-options-btn')) {
        dropdown.style.display = 'none';
        document.removeEventListener('click', closeDropdownOutside);
    }
}

async function openAddToPlaylistModal() {
    // Hide dropdown
    const dropdown = document.getElementById('global-video-options-dropdown');
    if (dropdown) dropdown.style.display = 'none';
    
    // Create or locate modal
    let modal = document.getElementById('global-playlist-modal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'global-playlist-modal';
        modal.className = 'modal-overlay';
        document.body.appendChild(modal);
    }
    
    modal.style.display = 'flex';
    modal.innerHTML = `
        <div class="modal-content">
            <h3 class="modal-title">Add video to playlist</h3>
            <div class="playlists-list-container" id="modal-playlists-list">
                <div class="loading">Loading playlists...</div>
            </div>
            <div style="margin-top: 20px; display: flex; gap: 10px; justify-content: flex-end;">
                <button class="btn btn-cancel" onclick="closePlaylistModal()" style="background: none; border: none; color: white; cursor: pointer; padding: 8px 16px;">Close</button>
                <button class="btn btn-primary-blue" onclick="openCreatePlaylistForm()" style="border: none; padding: 8px 16px; border-radius: 20px; font-weight: 500; cursor: pointer;">Create New</button>
            </div>
        </div>
    `;
    
    // Fetch playlists based on type
    const channelId = new URLSearchParams(window.location.search).get('id') || TEST_CHANNEL_ID;
    const url = activePlaylistType === 'channel' 
        ? `${API_BASE_URL}/api/Playlist/channel/${channelId}`
        : `${API_BASE_URL}/api/Playlist/user/${TEST_USER_ID}`;
        
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error('Failed to load playlists');
        const res = await response.json();
        const playlists = res.data || [];
        
        const listEl = document.getElementById('modal-playlists-list');
        if (playlists.length > 0) {
            listEl.innerHTML = playlists.map(pl => `
                <div class="playlist-select-item" onclick="addVideoToPlaylist('${pl.id}')">
                    <span class="playlist-icon">📁</span>
                    <div style="flex: 1; text-align: left;">
                        <div class="playlist-select-name">${pl.name}</div>
                        <div class="playlist-select-count" style="font-size: 11px; color: var(--text-secondary);">${pl.videoIds.length} videos</div>
                    </div>
                </div>
            `).join('');
        } else {
            listEl.innerHTML = '<div style="font-size: 13px; color: var(--text-secondary); padding: 10px 0; text-align: left;">No playlists available. Click "Create New" to make one!</div>';
        }
    } catch (error) {
        console.error('Error loading playlists:', error);
        document.getElementById('modal-playlists-list').innerHTML = '<div style="color: #f44336; font-size: 13px;">Error loading playlists.</div>';
    }
}

async function addVideoToPlaylist(playlistId) {
    if (!activeVideoId) return;
    try {
        const response = await fetch(`${API_BASE_URL}/api/Playlist/${playlistId}/videos/${activeVideoId}?userId=${TEST_USER_ID}`, {
            method: 'POST',
            headers: {
                'X-User-Id': TEST_USER_ID
            }
        });
        
        if (!response.ok) {
            const data = await response.json();
            throw new Error(data.message || 'Failed to add video to playlist');
        }
        
        alert('Video added to playlist successfully!');
        closePlaylistModal();
    } catch (error) {
        console.error('Error adding video to playlist:', error);
        alert(error.message);
    }
}

function openCreatePlaylistForm() {
    const modalContent = document.querySelector('.modal-content');
    if (!modalContent) return;
    
    modalContent.innerHTML = `
        <h3 class="modal-title">Create new playlist</h3>
        <div class="form-group" style="display: flex; flex-direction: column; gap: 12px; margin-top: 15px; text-align: left;">
            <div style="display: flex; flex-direction: column; gap: 4px;">
                <label style="font-size: 12px; color: var(--text-secondary);">Name</label>
                <input type="text" id="new-playlist-name" placeholder="Playlist Name" style="padding: 8px 12px; border-radius: 6px; border: 1px solid var(--border); background: #181818; color: white; outline: none;">
            </div>
            <div style="display: flex; flex-direction: column; gap: 4px;">
                <label style="font-size: 12px; color: var(--text-secondary);">Description (Optional)</label>
                <textarea id="new-playlist-desc" placeholder="Description" style="padding: 8px 12px; border-radius: 6px; border: 1px solid var(--border); background: #181818; color: white; resize: none; height: 60px; outline: none; font-family: inherit; font-size: 13px;"></textarea>
            </div>
            <div style="display: flex; align-items: center; gap: 8px; margin-top: 5px;">
                <input type="checkbox" id="new-playlist-public" checked style="cursor: pointer; width: 16px; height: 16px;">
                <label for="new-playlist-public" style="font-size: 13px; cursor: pointer; user-select: none;">Public playlist</label>
            </div>
        </div>
        <div style="margin-top: 25px; display: flex; gap: 10px; justify-content: flex-end;">
            <button class="btn btn-cancel" onclick="${activeVideoId ? 'openAddToPlaylistModal()' : 'closePlaylistModal()'}" style="background: none; border: none; color: white; cursor: pointer; padding: 8px 16px;">Cancel</button>
            <button class="btn btn-primary-blue" onclick="submitCreatePlaylist()" style="border: none; padding: 8px 16px; border-radius: 20px; font-weight: 500; cursor: pointer;">Create</button>
        </div>
    `;
}

async function submitCreatePlaylist() {
    const nameInput = document.getElementById('new-playlist-name');
    const descInput = document.getElementById('new-playlist-desc');
    const publicCheckbox = document.getElementById('new-playlist-public');
    
    if (!nameInput) return;
    const name = nameInput.value.trim();
    if (!name) {
        alert('Playlist name is required.');
        return;
    }
    
    const description = descInput ? descInput.value.trim() : '';
    const isPublic = publicCheckbox ? publicCheckbox.checked : true;
    
    const channelId = new URLSearchParams(window.location.search).get('id') || TEST_CHANNEL_ID;
    
    const isChannel = activePlaylistType === 'channel';
    const endpoint = isChannel 
        ? `${API_BASE_URL}/api/Playlist/channel/${channelId}?userId=${TEST_USER_ID}`
        : `${API_BASE_URL}/api/Playlist/custom?userId=${TEST_USER_ID}`;
        
    const payload = {
        name: name,
        description: description,
        isPublic: isPublic,
        thumbnailUrl: ""
    };
    
    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-User-Id': TEST_USER_ID
            },
            body: JSON.stringify(payload)
        });
        
        if (!response.ok) {
            const data = await response.json();
            throw new Error(data.message || 'Failed to create playlist');
        }
        
        alert('Playlist created successfully!');
        
        if (!activeVideoId) {
            closePlaylistModal();
        } else {
            openAddToPlaylistModal();
        }
    } catch (error) {
        console.error('Error creating playlist:', error);
        alert(error.message);
    }
}

function openCreateChannelPlaylistDirectly() {
    activeVideoId = null;
    activePlaylistType = 'channel';
    
    let modal = document.getElementById('global-playlist-modal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'global-playlist-modal';
        modal.className = 'modal-overlay';
        document.body.appendChild(modal);
    }
    
    modal.style.display = 'flex';
    openCreatePlaylistForm();
}

function closePlaylistModal() {
    const modal = document.getElementById('global-playlist-modal');
    if (modal) modal.style.display = 'none';
}

// Bind to window for click access
window.showVideoOptions = showVideoOptions;
window.openAddToPlaylistModal = openAddToPlaylistModal;
window.addVideoToPlaylist = addVideoToPlaylist;
window.openCreatePlaylistForm = openCreatePlaylistForm;
window.submitCreatePlaylist = submitCreatePlaylist;
window.openCreateChannelPlaylistDirectly = openCreateChannelPlaylistDirectly;
window.closePlaylistModal = closePlaylistModal;
