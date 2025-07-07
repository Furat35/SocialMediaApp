<template>
  <div class="ig-main-layout">
    <aside class="ig-sidebar ig-sidebar-left-fixed">
      <LeftSidebar />
    </aside>
    <main class="ig-profile-main">
      <!-- Profile Header -->
      <section class="profile-header">
        <img class="profile-avatar" :src="`${gatewayUrl}users/image?userId=${userId}`" alt="avatar" />
        <div class="profile-info">
          <div class="profile-username" style="line-height: 30px;">
            <div>
              <span>{{ user.fullname }}</span> <br>
              <span class="fs-6" style="font-weight: 100;">{{ user.username }}</span>
            </div>
            <router-link :to="{ name: 'setting' }" class="ig-nav-link d-block" v-if="showSettings"><span
                class=" material-icons" style="margin-left: auto">settings</span></router-link>
          </div>
          <div class="profile-stats">
            <span><strong>{{ totalPosts }}</strong> posts</span>
            <span style="cursor: pointer" @click="openFollowersModal"><strong>{{ totalFollowers }}</strong>
              followers</span>
          </div>
          <div class="profile-bio">{{ user.bio }} </div>
        </div>
      </section>
      <!-- Posts Grid -->
      <section class="profile-posts-grid">
        <div class="profile-post-thumb" v-for="post in posts" :key="post.id" @click="openCommentsModal(post)">
          <img :src="post.imageUrl" alt="post" />
          <div class="profile-post-overlay">
            <span class="material-icons" :style="{ color: post.likes.find(_ => _.user.id == userId) ? '#c13584' : '' }"
              @click.stop="likePost(post)">favorite_border</span>
            <span class="px-1" @click.stop="openLikesModal(post)">{{ post.likes.length }}</span>
            <span class="material-icons">chat_bubble_outline</span> <span>{{ post.comments.length }}</span>
          </div>
        </div>
      </section>
      <div style="text-align: center;" v-if="posts.length === 0">No post found...</div>
      <!-- Comments Modal -->
      <div v-if="showCommentsModal" class="comments-modal-backdrop" @click.self="closeCommentsModal">
        <div class="comments-modal">
          <button class="close-modal-btn" @click="closeCommentsModal" aria-label="Close">&times;</button>
          <div class="post-image">
            <img :src="`${selectedPost.imageUrl}`" alt="post" />
          </div>
          <div v-if="selectedPost" class="comments-panel">
            <div class="comments-list">
              <div v-for="comment in selectedPost.comments" :key="comment.id" class="comment">
                <span @click="goToProfile(comment.user.id)" style="cursor: pointer;">
                  <img :src="`${gatewayUrl}users/image?userId=${comment.user.id}`" class="me-2"
                    style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                  <strong>{{ comment.user.username }}</strong> <span>{{ comment.userComment }}</span>
                </span>
                <span style="display: block;margin-right: auto;text-align: end;font-size: small;">
                  {{ comment.createDate.toLocaleDateString('en-En') }}
                </span>
                <hr>
              </div>
            </div>
            <div class="add-comment">
              <input v-model="newComment" @keyup.enter="addComment" type="text" placeholder="Add a comment..." />
              <button @click="addComment" :disabled="!newComment.trim()">Post</button>
            </div>
          </div>
        </div>
      </div>
      <!-- Likes Modal -->
      <div v-if="showLikesModal" class="likes-modal-backdrop" @click.self="closeLikesModal">
        <div class="likes-modal">
          <button class="close-modal-btn" @click="closeLikesModal" aria-label="Close">&times;</button>
          <div v-if="selectedPost" class="likes-panel">
            <div class="likes-list">
              <div class="mb-3">
                <strong>{{ selectedPost.likes.length }} Likes</strong>
              </div>
              <hr>
              <div v-for="like in selectedPost.likes" :key="like.id" class="like mb-2">
                <div @click="goToProfile(like.user.id)" style="cursor: pointer;">
                  <img :src="`${gatewayUrl}users/image?userId=${like.user.id}`" class="me-2"
                    style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                  <strong>{{ like.user.username }}</strong>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- Followers Modal -->
      <div v-if="showFollowersModal" class="followers-modal-backdrop" @click.self="closeFollowersModal">
        <div class="followers-modal">
          <button class="close-modal-btn" @click="closeFollowersModal" aria-label="Close">&times;</button>
          <div class="followers-panel">
            <div class="followers-list" ref="followersList" @scroll="handleFollowersScroll">
              <div class="mb-3">
                <strong>{{ totalFollowers }} Followers</strong>
              </div>
              <hr>
              <div v-for="follower in followers" :key="follower.user.id" class="like mb-2">
                <div @click="goToProfile(follower.user.id)" style="cursor: pointer;">
                  <img :src="`${gatewayUrl}users/image?userId=${follower.user.id}`" class="me-2"
                    style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                  <strong>{{ follower.user.fullname }}</strong>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PostLikeListDto } from '@user/src/models/posts/PostLikeListDto';
import { PostCommentListDto } from '@user/src/models/posts/PostCommentListDto';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ScrollModel } from '@shared/models/ScrollModel';

export default {
  components: { LeftSidebar },
  name: 'InstagramProfile',
  async created() {
    this.getPosts()
    this.getUserInfo()
    this.getUserFollowers()
  },
  data() {
    return {
      totalPosts: 0,
      totalFollowers: 0,
      posts: [] as PostListDto[],
      followers: [] as FollowerListModel[],
      user: new UserListDto(),
      postsScrollModel: new ScrollModel(),
      followerScrollModel: new ScrollModel(),
      userId: parseInt(this.$route.query.userId as string),
      showCommentsModal: false,
      showLikesModal: false,
      showFollowersModal: false,
      selectedPost: null as PostListDto | null,
      newComment: '',
      gatewayUrl: import.meta.env.VITE_GatewayUrl,
      profileChanged: false
    }
  },
  mounted() {
    window.addEventListener('scroll', this.handleScroll)
  },
  beforeUnmount() {
    window.removeEventListener('scroll', this.handleScroll)
  },
  computed: {
    showSettings() {
      return useUserStore().getUserId === this.userId;
    },
  },
  watch: {
    "$route.query.userId"(newVal, oldVal) {
      if (newVal !== oldVal) {
        Object.assign(this.postsScrollModel, new ScrollModel())
        Object.assign(this.followerScrollModel, new ScrollModel())
        this.userId = parseInt(newVal)
        this.posts = []
        this.followers = []
        this.getUserInfo()
        this.getUserFollowers()
        this.getPosts()
        if (this.showCommentsModal)
          this.closeCommentsModal()
        if (this.showLikesModal)
          this.closeLikesModal()
        if (this.showFollowersModal)
          this.closeFollowersModal()
      }
    }
  },
  methods: {
    handleScroll() {
      const scrollBottom = window.innerHeight + window.scrollY
      const threshold = document.body.offsetHeight - 600
      if (scrollBottom >= threshold && this.postsScrollModel.hasMore && !this.postsScrollModel.isLoading)
        this.getPosts()
    },
    goToProfile(newUserId: number) {
      if (newUserId !== this.userId)
        this.$router.push({ name: 'profile', query: { userId: newUserId } });
    },
    handleFollowersScroll() {
      const el = this.$refs.followersList as HTMLElement;
      if (!el) return;
      const threshold = 200;
      if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerScrollModel.hasMore && !this.followerScrollModel.isLoading) {
        this.getUserFollowers();
      }
    },

    async getUserInfo() {
      var response = await this.$axios.get(`/users/${this.userId}`)
      Object.assign(this.user, response.data.data);
    },
    async getUserFollowers() {
      if (this.followerScrollModel.isLoading) return;
      this.followerScrollModel.isLoading = true;
      try {
        var response = await this.$axios.get(`/aggregated/followers/byUser?userId=${this.userId}&page=${this.followerScrollModel.currentPage}&pageSize=30`)
        if (response.data.data) {
          const newFollowers = response.data.data.map(f => new FollowerListModel(f))
          this.totalFollowers = response.data.totalEntities
          this.followers.push(...newFollowers)
          if (!response.data.hasNext) this.followerScrollModel.hasMore = false;
          else this.followerScrollModel.currentPage++;
        }
      }
      finally {
        this.followerScrollModel.isLoading = false;
      }
    },
    async getPosts() {
      if (this.postsScrollModel.isLoading) return;
      this.postsScrollModel.isLoading = true;
      try {
        var response = await this.$axios.get(`/aggregated/feeds/${this.userId}?page=${this.postsScrollModel.currentPage}&pageSize=18`)
        if (response.data.data) {
          this.totalPosts = response.data.totalEntities
          const newPosts = response.data.data.map(p => new PostListDto(p))
          await this.setPostImages(newPosts);
          this.posts.push(...newPosts);
          if (!response.data.hasNext) this.postsScrollModel.hasMore = false;
          else this.postsScrollModel.currentPage++;
        }
      } catch (error) {
        console.error('Error fetching posts:', error);
      } finally {
        this.postsScrollModel.isLoading = false;
      }
    },
    async setPostImages(postsToUpdate: PostListDto[]) {
      for (const post of postsToUpdate) {
        const imageResponse = await this.$axios.get(`/posts/image?postId=${post.id}`, {
          responseType: 'blob',
        });
        post.imageUrl = URL.createObjectURL(imageResponse.data);
      }
    },
    async addComment() {
      if (!this.newComment.trim() || !this.selectedPost) return;
      var response = await this.$axios.post(`/posts/add-comment?postId=${this.selectedPost.id}&userComment=${this.newComment}`)
      if (!response.data.isError) {
        this.selectedPost.comments.push(new PostCommentListDto({ postId: this.selectedPost.id, userComment: this.newComment, user: new UserListDto({ id: useUserStore().getUserId, username: useUserStore().getUsername }), createDate: new Date() }))
      }
      this.newComment = '';
    },
    async likePost(post: PostListDto) {
      var currentUserId = useUserStore().getUserId;
      if (post.likes.find(_ => _.user.id == currentUserId)) {
        this.removeLike(post);
        return;
      }
      var response = await this.$axios.post(`/posts/like?postId=${post.id}`)
      if (!response.data.isError) {
        var like = new PostLikeListDto({ user: new UserListDto({ id: currentUserId, username: useUserStore().getUsername }), postId: post.id });
        post.likes.push(like)
      }
    },
    async removeLike(post: PostListDto) {
      var currentUserId = useUserStore().getUserId;
      var response = await this.$axios.post(`/posts/unlike?postId=${post.id}`)
      if (!response.data.isError)
        post.likes = post.likes.filter(_ => _.user.id != currentUserId)
    },
    openFollowersModal() {
      this.showFollowersModal = true;
    },
    closeFollowersModal() {
      this.showFollowersModal = false;
    },
    openCommentsModal(post: PostListDto) {
      this.selectedPost = post;
      this.showCommentsModal = true;
    },
    closeCommentsModal() {
      this.showCommentsModal = false;
      this.selectedPost = null;
    },
    openLikesModal(post: PostListDto) {
      this.selectedPost = post;
      this.showLikesModal = true;
    },
    closeLikesModal() {
      this.showLikesModal = false;
      this.selectedPost = null;
    }

  },
}
</script>

<style>
.follower-list {
  max-height: 400px;
  overflow-y: auto;
}
</style>