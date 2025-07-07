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
            <span style="cursor: pointer" @click="openFriendsModal"><strong>{{ totalFriends }}</strong>
              friends</span>
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
      <!-- Friends Modal -->
      <div v-if="showFriendsModal" class="friends-modal-backdrop" @click.self="closeFriendsModal">
        <div class="friends-modal">
          <button class="close-modal-btn" @click="closeFriendsModal" aria-label="Close">&times;</button>
          <div class="friends-panel">
            <div class="friends-list" ref="friendsList" @scroll="handleFriendsScroll">
              <div class="mb-3">
                <strong>{{ totalFriends }} Friends</strong>
              </div>
              <hr>
              <div v-for="friend in friends" :key="friend.userId" class="like mb-2">
                <div @click="goToProfile(friend.userId)" style="cursor: pointer;">
                  <img :src="`${gatewayUrl}users/image?userId=${friend.userId}`" class="me-2"
                    style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                  <strong>{{ friend.user.fullname }}</strong>
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
import { FriendListModel } from '@shared/models/friends/FriendListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PostLikeListDto } from '@user/src/models/posts/PostLikeListDto';
import { PostCommentListDto } from '@user/src/models/posts/PostCommentListDto';
import { UserListDto } from '@shared/models/users/UserListDto';

export default {
  components: { LeftSidebar },
  name: 'InstagramProfile',
  async created() {
    this.getPosts()
    this.getUserInfo()
    this.getUserFriends()
  },
  data() {
    return {
      totalPosts: 0,
      totalFriends: 0,
      posts: [] as PostListDto[],
      friends: [] as FriendListModel[],
      user: new UserListDto(),
      currentPage: 1,
      isLoading: false,
      hasMore: true,
      friendsCurrentPage: 1,
      friendsIsLoading: false,
      friendsHasMore: true,
      userId: parseInt(this.$route.query.userId as string),
      showCommentsModal: false,
      showLikesModal: false,
      showFriendsModal: false,
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
    }
  },
  watch: {
    "$route.query.userId"(newVal, oldVal) {
      if (newVal !== oldVal) {
        this.currentPage = 1
        this.isLoading = false
        this.hasMore = true
        this.friendsCurrentPage = 1
        this.friendsIsLoading = false
        this.friendsHasMore = true
        this.userId = newVal
        this.posts = []
        this.friends = []
        this.getUserInfo()
        this.getUserFriends()
        this.getPosts()
        if (this.showCommentsModal)
          this.closeCommentsModal()
        if (this.showLikesModal)
          this.closeLikesModal()
        if (this.showFriendsModal)
          this.closeFriendsModal()
      }
    }
  },
  methods: {
    handleScroll() {
      const scrollBottom = window.innerHeight + window.scrollY
      const threshold = document.body.offsetHeight - 600

      if (scrollBottom >= threshold && this.hasMore && !this.isLoading)
        this.getPosts()
    },
    goToProfile(newUserId: number) {
      if (newUserId !== this.userId)
        this.$router.push({ name: 'profile', query: { userId: newUserId } });
    },
    handleFriendsScroll() {
      const el = this.$refs.friendsList as HTMLElement;
      if (!el) return;
      const threshold = 200;
      console.log(el.scrollTop)
      console.log(el.clientHeight)
      console.log(el.scrollHeight)
      console.log(el)
      if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.friendsHasMore && !this.friendsIsLoading) {
        this.getUserFriends();
      }
    },
    async getUserInfo() {
      var response = await this.$axios.get(`/users/${this.userId}`)
      Object.assign(this.user, response.data.data);
    },
    async getUserFriends() {
      if (this.friendsIsLoading) return;
      this.friendsIsLoading = true;
      try {
        var response = await this.$axios.get(`/aggregated/friends/byUser?userId=${this.userId}&page=${this.friendsCurrentPage}&pageSize=30`)
        if (response.data.data && !response.data.data.isError) {
          const newFriends = response.data.data.data.map(f => new FriendListModel(f))
          this.totalFriends = response.data.data.totalEntities
          this.friends.push(...newFriends);
          if (!response.data.data.hasNext) this.friendsHasMore = false;
          else this.friendsCurrentPage++;
        }
      }
      finally {
        this.friendsIsLoading = false;
      }

    },
    async getPosts() {
      if (this.isLoading) return;
      this.isLoading = true;
      try {
        var response = await this.$axios.get(`/aggregated/feeds/${this.userId}?page=${this.currentPage}&pageSize=18`)
        if (response.data.data && !response.data.data.isError) {
          this.totalPosts = response.data.data.totalEntities
          const newPosts = response.data.data.data.map(p => new PostListDto(p))
          await this.setPostImages(newPosts);
          this.posts.push(...newPosts);
          if (!response.data.data.hasNext) this.hasMore = false;
          else this.currentPage++;
        }
        else
          console.error('Error fetching posts:', response.data.errorMessages);
      } catch (error) {
        console.error('Error fetching posts:', error);
      } finally {
        this.isLoading = false;
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
      if (!response.data.data.isError) {
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
      if (!response.data.data.isError) {
        var like = new PostLikeListDto({ user: new UserListDto({ id: currentUserId, username: useUserStore().getUsername }), postId: post.id });
        post.likes.push(like)
      }
    },
    async removeLike(post: PostListDto) {
      var currentUserId = useUserStore().getUserId;
      var response = await this.$axios.post(`/posts/unlike?postId=${post.id}`)
      if (!response.data.data.isError)
        post.likes = post.likes.filter(_ => _.user.id != currentUserId)
    },
    openFriendsModal() {
      this.showFriendsModal = true;
    },
    closeFriendsModal() {
      this.showFriendsModal = false;
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
.friends-list {
  max-height: 400px;
  overflow-y: auto;
}
</style>