// Base URL for the API
const API_BASE_URL = 'http://localhost:8080/api';

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
        const response = await fetch(`${API_BASE_URL}/Video/homePageVideos?PageIndex=1&PageSize=20`);
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

/**
 * Create HTML string for a single video card
 */
function createVideoCard(video) {
    const timeAgo = getTimeAgo(new Date(video.uploadDate));
    
    // In a real app we'd link to the video player page. 
    // Here we'll just link directly to the video URL for quick verification.
    const link = video.videoUrl ? video.videoUrl : '#';
    
    return `
        <div class="video-card" onclick="window.open('${link}', '_blank')">
            <div class="thumbnail-container">
                <img src="${video.thumbnailUrl || 'https://via.placeholder.com/640x360?text=No+Thumbnail'}" alt="Thumbnail" class="thumbnail">
                <div class="video-duration">${video.duration}</div>
            </div>
            <div class="video-info">
                <img src="https://ui-avatars.com/api/?name=${video.channelName || 'User'}&background=random" alt="Channel" class="channel-avatar">
                <div class="video-details">
                    <h3 class="video-title" title="${video.title}">${video.title}</h3>
                    <div class="video-meta">
                        <div>${video.channelName || 'Unknown Channel'}</div>
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

        const response = await fetch(`${API_BASE_URL}/Video/uploadVideo?channelId=${TEST_CHANNEL_ID}`, {
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
