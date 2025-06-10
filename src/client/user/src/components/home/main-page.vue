<template>
    <!-- Feed -->
    <div class="ig-main-layout">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>

        <main class="ig-feed-main">
            <!-- Stories -->
            <div class="ig-stories-bar">
                <div class="story-circle ig-story" v-for="(story, index) in stories" :key="index">
                    <img :src="story.image" alt="story" />
                </div>
            </div>
            <!-- Feed -->
            <div>
                <div class="post-card ig-post" v-for="(post, index) in posts" :key="index">
                    <div class="post-header ig-post-header">
                        <img :src="post.profile" alt="profile" />
                        <strong>{{ post.username }}</strong>
                        <span class="material-icons ig-post-menu ms-auto">more_horiz</span>
                    </div>
                    <div class="post-image ig-post-image">
                        <img :src="post.image" alt="post" />
                    </div>
                    <div class="ig-post-actions p-3">
                        <span class="material-icons ig-action">favorite_border</span>
                        <span class="material-icons ig-action">chat_bubble_outline</span>
                        <span class="material-icons ig-action">send</span>
                    </div>
                    <div class="px-3 pb-2">
                        <strong>{{ post.username }}</strong> {{ post.caption }}
                        <div class="text-muted small mt-1">{{ post.time }}</div>
                    </div>
                </div>
            </div>
        </main>

        <FriendSuggestions></FriendSuggestions>
    </div>

</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import FriendSuggestions from '../shared/friend-suggestions.vue';

export default {
    components: {
        LeftSidebar,
        FriendSuggestions
    },
    name: 'InstagramClone',
    data() {
        return {
            stories: [
                { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/2.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/3.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/4.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/5.jpg' }
            ],
            posts: [
                {
                    username: 'jane_doe',
                    profile: 'https://randomuser.me/api/portraits/women/1.jpg',
                    image: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80',
                    caption: 'Enjoying the view! 🌄',
                    time: '2 hours ago'
                },
                {
                    username: 'john_smith',
                    profile: 'https://randomuser.me/api/portraits/men/2.jpg',
                    image: 'https://images.unsplash.com/photo-1465101046530-73398c7f28ca?auto=format&fit=crop&w=800&q=80',
                    caption: 'Great vibes only.',
                    time: '3 hours ago'
                }
            ],
        }
    }
}
</script>
