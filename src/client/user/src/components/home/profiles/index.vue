<template lang="pug">
.ig-main-layout
  aside.ig-sidebar.ig-sidebar-left-fixed
    LeftSidebar
  main.ig-profile-main
    // Profile Header
    section.profile-header
      img.profile-avatar(:src='`${gatewayUrl}users/image?userId=${userId}`' alt='avatar')
      .profile-info
        .profile-username(style='line-height: 30px;')
          div
            span {{ user.fullname }}
            br
            span.fs-6(style='font-weight: 100;') {{ user.username }}
          div(style='display: flex;justify-content: end;align-items: flex-start; flex-basis: 400px;')
            .btn-group
              div(v-if='userId != useUserStore.getUserId')
                button.btn(@click='sendFollowRequest' v-if='followStatus.status == FollowStatusEnum.NOTFOLLOWING')
                  span.material-icons(style='vertical-align: middle; font-size: 20px; margin-right: 6px;') person_add
                  |  Follow
                button.btn(@click='sendFollowRequest' v-else-if='followStatus.status == FollowStatusEnum.DECLINED')
                  span.material-icons(style='vertical-align: middle; font-size: 20px; margin-right: 6px;') person_add
                  |  Follow
                button.btn(v-else-if='followStatus.status == FollowStatusEnum.FOLLOWING' @click='unfollow' style='border: 1px solid')
                  span.material-icons(style='vertical-align: middle; font-size: 20px; margin-right: 6px;' alt='delet') person_off
                  |  Unfollow
                button.btn(v-else-if='followStatus.status == FollowStatusEnum.PENDING && followStatus.respondingUserId == useUserStore.getUserId' @click='declineFollowRequest' style='border: 1px solid')
                  |  Decline Request
                button.btn(v-if='followStatus.status == FollowStatusEnum.PENDING' @click='cancelFollowRequest' style='border: 1px solid')
                  |  Cancel Follow Request
                button.btn(v-if='followStatus.status != FollowStatusEnum.BANNED' @click='banFollower' class='ms-1' style='border: 1px solid')
                  |  Ban
                button.btn(v-if='followStatus.status == FollowStatusEnum.BANNED && useUserStore.getUserId == followStatus.requestingUserId' @click='removeBan' class='ms-1' style='border: 1px solid')
                  |  Remove Ban
                div(v-else-if='followStatus.status == FollowStatusEnum.BANNED && useUserStore.getUserId != followStatus.requestingUserId' class='text-center fs-5')
                  |  Banned !
            router-link.ig-nav-link.d-block(:to="{ name: 'setting' }" style='padding-top: 5px; padding-bottom: 5px;' v-if='showSettings')
              span.material-icons(style='margin-left: auto') settings
        .profile-stats
          span
            strong {{ totalPosts }} posts
          span(style='cursor: pointer' @click='openFollowersModal')
            strong {{ totalFollowers }} followers
        .profile-bio {{ user.bio }} 
    hr
    br
    // Posts Grid
    section.profile-posts-grid
      .profile-post-thumb(v-for='post in posts' :key='post.id' @click='openCommentsModal(post)')
        img(:src='post.imageUrl' alt='post')
        .profile-post-overlay
          span.material-icons(:style="{ color: post.likes.find(_ => _.user.id == userId) ? '#c13584' : '' }" @click.stop='likePost(post)') favorite_border
          span.px-1(@click.stop='openLikesModal(post)') {{ post.likes.length }}
          span.material-icons chat_bubble_outline
          span {{ post.comments.length }}
   
    div(style='text-align: center;' v-if='posts.length === 0 && (followStatus.status == FollowStatusEnum.FOLLOWING || userId == useUserStore.getUserId)')
      |  No post found...
    div(style='text-align: center;' v-else-if='followStatus.status == FollowStatusEnum.NOTFOLLOWING && userId != useUserStore.getUserId')
      |  Not Following...
    // Comments Modal
    .comments-modal-backdrop(v-if='showCommentsModal' @click.self='closeCommentsModal')
      .comments-modal
        button.close-modal-btn(@click='closeCommentsModal' aria-label='Close') &times;
        .post-image
          img(:src='`${selectedPost.imageUrl}`' alt='post')
        .comments-panel(v-if='selectedPost')
          .comments-list
            .comment(v-for='comment in selectedPost.comments' :key='comment.id')
              span(@click='goToProfile(comment.user.id)' style='cursor: pointer;')
                img.me-2(:src='`${gatewayUrl}users/image?userId=${comment.user.id}`' style='border-radius: 50%;width: 25px;height: 25px;' alt='post' height='30px')
                strong {{ comment.user.username }}
                span {{ comment.userComment }}
              span(style='display: block;margin-right: auto;text-align: end;font-size: small;')
                | {{ comment.createDate.toLocaleDateString(&apos;en-En&apos;) }}
              hr
          .add-comment
            input(v-model='newComment' @keyup.enter='addComment' type='text' placeholder='Add a comment...')
            button(@click='addComment' :disabled='!newComment.trim()') Post
    // Likes Modal
    .likes-modal-backdrop(v-if='showLikesModal' @click.self='closeLikesModal')
      .likes-modal
        button.close-modal-btn(@click='closeLikesModal' aria-label='Close') &times;
        .likes-panel(v-if='selectedPost')
          .likes-list
            .mb-3
              strong {{ selectedPost.likes.length }} Likes
            hr
            .like.mb-2(v-for='like in selectedPost.likes' :key='like.id')
              div(@click='goToProfile(like.user.id)' style='cursor: pointer;')
                img.me-2(:src='`${gatewayUrl}users/image?userId=${like.user.id}`' style='border-radius: 50%;width: 25px;height: 25px;' alt='post' height='30px')
                strong {{ like.user.username }}
    // Followers Modal
    .followers-modal-backdrop(v-if='showFollowersModal' @click.self='closeFollowersModal')
      .followers-modal
        button.close-modal-btn(@click='closeFollowersModal' aria-label='Close') &times;
        .followers-panel
          .followers-list(ref='followersList' @scroll='handleFollowersScroll')
            .mb-3
              strong {{ totalFollowers }} Followers
            hr
            .like.mb-2(v-for='follower in followers' :key='follower.user.id')
              div(@click='goToProfile(follower.user.id)' style='cursor: pointer;')
                img.me-2(:src='`${gatewayUrl}users/image?userId=${follower.user.id}`' style='border-radius: 50%;width: 25px;height: 25px;' alt='post' height='30px')
                strong {{ follower.user.fullname }}
</template>

<script lang="ts">
import LeftSidebar from '@user/src/components/shared/left-sidebar.vue';
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PostLikeListDto } from '@user/src/models/posts/PostLikeListDto';
import { PostCommentListDto } from '@user/src/models/posts/PostCommentListDto';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ScrollModel } from '@shared/models/ScrollModel';
import { toast } from '@user/src/helpers/toast';
import { FollowStatusEnum } from '@shared/models/followers/FollowStatusEnum';

export default {
  components: { LeftSidebar },
  name: 'InstagramProfile',
  async created() {
    this.getFollowStatus()
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
      followStatus: new FollowerListModel(),
      gatewayUrl: import.meta.env.VITE_GatewayUrl,
      FollowStatusEnum,
      useUserStore: useUserStore(),
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
      return this.useUserStore.getUserId === this.userId;
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
        this.showFollower = false
        this.getFollowStatus()
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
      if (this.followStatus.status == FollowStatusEnum.PENDING) return;
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
    async sendFollowRequest() {
      try {
        const response = await this.$axios.post(`/followers/follow/${this.userId}`);
        if (!response.data.isError) {
          toast.success('Follow request sent!');
          this.followStatus.status = FollowStatusEnum.PENDING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not send follow request');
      }
    },
    async declineFollowRequest() {
      try {
        const response = await this.$axios.post(`/followers/decline/${this.userId}`);
        if (!response.data.isError) {
          toast.success('Request declined!');
          this.followStatus.status = FollowStatusEnum.NOTFOLLOWING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not decline request');
      }
    },
    async unfollow() {
      try {
        const response = await this.$axios.post(`/followers/unfollow/${this.userId}`);
        if (!response.data.isError) {
          toast.success('Unfollowed successfully!');
          this.followStatus.status = FollowStatusEnum.NOTFOLLOWING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not unfollow');
      }
    },
    async cancelFollowRequest() {
      try {
        const response = await this.$axios.post(`/followers/cancel/${this.userId}`);
        if (!response.data.isError) {
          toast.success('successfully cancelled!');
          this.followStatus.status = FollowStatusEnum.NOTFOLLOWING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not cancel follow request');
      }
    },
    async banFollower() {
      try {
        const response = await this.$axios.post(`/followers/ban/${this.userId}`);
        if (!response.data.isError) {
          toast.success('Follower banned successfully!');
          this.followStatus.status = FollowStatusEnum.BANNED;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not ban follower');
      }
    },
    async removeBan() {
      try {
        const response = await this.$axios.post(`/followers/removeBan/${this.userId}`);
        if (!response.data.isError) {
          toast.success('Ban removed successfully!');
          this.followStatus.status = FollowStatusEnum.NOTFOLLOWING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not remove ban');
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
        var response = await this.$axios.get(`/aggregated/followers/byUser?status=1&userId=${this.userId}&page=${this.followerScrollModel.currentPage}&pageSize=30`)
        if (response.data.data.length > 0) {
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
    getFollowStatus() {
      this.$axios.get(`/followers/status?userId=${this.userId}`)
        .then(response => {
          Object.assign(this.followStatus, response.data.data);
        })
        .catch(error => {
          console.error('Error fetching follow status:', error);
        });
    },
    async getPosts() {
      if (this.postsScrollModel.isLoading) return;
      this.postsScrollModel.isLoading = true;
      try {
        var response = await this.$axios.get(`/aggregated/feeds/${this.userId}?page=${this.postsScrollModel.currentPage}&pageSize=18`)
        this.totalPosts = response.data.totalEntities
        if (response.data.data.length > 0) {
          const newPosts = response.data.data.map(p => new PostListDto(p))
          await this.setPostImages(newPosts);
          this.posts.push(...newPosts);
          if (!response.data.hasNext) this.postsScrollModel.hasMore = false;
          else this.postsScrollModel.currentPage++;
        }
      } catch (error) {
        if (error.response.data.statusCode === 403) {
          this.totalPosts = error.response.data.totalEntities
        }
        console.log(error.response.data.errorMessages.join(', ') || 'Error fetching posts');
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
        this.selectedPost.comments.push(new PostCommentListDto({
          postId: this.selectedPost.id, userComment: this.newComment,
          user: new UserListDto({ id: this.useUserStore.getUserId, username: this.useUserStore.getUsername }), createDate: new Date()
        }))
        this.newComment = '';
      }
    },
    async likePost(post: PostListDto) {
      var currentUserId = this.useUserStore.getUserId;
      if (post.likes.find(_ => _.user.id == currentUserId)) {
        this.removeLike(post);
        return;
      }
      var response = await this.$axios.post(`/posts/like?postId=${post.id}`)
      if (!response.data.isError) {
        var like = new PostLikeListDto({ user: new UserListDto({ id: currentUserId, username: this.useUserStore.getUsername }), postId: post.id });
        post.likes.push(like)
      }
    },
    async removeLike(post: PostListDto) {
      var currentUserId = this.useUserStore.getUserId;
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

.btn-gradient-follow {
  background: linear-gradient(90deg, #f58529 0%, #dd2a7b 50%, #8134af 100%);
  color: #fff;
  border: none;
  border-radius: 24px;
  padding: 8px 24px;
  font-weight: 500;
  font-size: 1rem;
  box-shadow: 0 2px 8px rgba(221, 42, 123, 0.08);
  transition: background 0.2s, box-shadow 0.2s;
  display: inline-flex;
  align-items: center;
  cursor: pointer;
}
</style>