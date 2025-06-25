<!-- filepath: c:\Projects\SocialMediaApp\src\client\pages\profile.vue -->
<template>
  <div class="ig-main-layout">
    <aside class="ig-sidebar ig-sidebar-left-fixed">
      <LeftSidebar />
    </aside>

    <main class="ig-user-page">
      <!-- Stories -->
      <div class="ig-stories-bar">
        <div class="story-circle ig-story" v-for="(story, index) in stories" :key="index">
          <img :src="story.image" alt="story" />
        </div>
      </div>
      <!-- Feed -->
      <div class="posts-grid">
        <div class="ig-post" v-for="(post, index) in posts.data" :key="index">
          <div class="post-header ig-post-header">
            <!-- <img :src="post.profile" alt="profile" /> -->
            <strong>{{ post.description }}</strong>
            <span class="material-icons ig-post-menu ms-auto">more_horiz</span>
          </div>
          <div class="post-image ig-post-image">
            <img :src="`${gatewayUrl}posts/image?postId=${post.id}`" alt="post" />
          </div>
          <div class="ig-post-actions p-3">
            <span class="material-icons ig-action">favorite_border</span>
            <span class="material-icons ig-action">chat_bubble_outline</span>
            <span class="material-icons ig-action">send</span>
          </div>
          <div class="px-3 pb-2">
            <strong>{{ post.description }}</strong> başlık
            <div class="text-muted small mt-1">{{ post.createDate }}</div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import FriendSuggestions from '../shared/friend-suggestions.vue';
import { PostListDto } from '@user/src/models/PostListDto';
import { PaginationModel } from '@shared/models/PaginationModel';

export default {
  components: {
    LeftSidebar,
    FriendSuggestions
  },
  name: 'InstagramClone',
  created() {
    this.getPosts();
  },
  data() {
    return {
      stories: [
        { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
      ],
      posts: new PaginationModel<PostListDto>(),
      gatewayUrl: import.meta.env.VITE_GatewayUrl
    }
  },
  methods: {
    getPosts() {
      this.$axios.get('/posts?userId=1&&page=1&&pageSize=10')
        .then(response => {
          if (response.data && !response.data.isError) {
            this.posts = response.data.data;
          } else {
            console.error('Error fetching posts:', response.data.errorMessages);
          }
        })
        .catch(error => {
          console.error('Error fetching posts:', error);
        });
    },
  },
}
</script>

<style scoped>
.ig-user-page {
  flex: 0 0 670px;
  max-width: 1000px;
  width: 100%;
  margin: 0 auto;
  box-sizing: border-box;
}

.posts-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 2rem;
  width: 100%;
  box-sizing: border-box;
  margin-top: 2rem;
}

.ig-post {
  background: #fafafa;
  border: 1px solid #eee;
  border-radius: 8px;
  padding: 1rem 0 1rem 0;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
}

@media (max-width: 900px) {
  .posts-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>