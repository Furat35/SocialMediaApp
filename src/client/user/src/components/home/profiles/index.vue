<template lang="pug">
.ig-main-layout
  aside.ig-sidebar.ig-sidebar-left-fixed
    LeftSidebar
  main.ig-profile-main
    section.profile-header
      img.profile-avatar(:src='`${gatewayUrl}users/image?userId=${userId}`' alt='avatar' 
      :class='this.story ? "profile-avatar-story-border" : ""' @click='openStoryModal')
      .profile-info
        .profile-username(style='line-height: 30px;')
          div
            span {{ user.fullname }}
            br
            span.fs-6(style='font-weight: 100;') {{ user.username }}
          div(style='display: flex;justify-content: end;align-items: flex-start; flex-basis: 400px;')
            .btn-group
              div(v-if='userId != useUserStore.getUserId')
                button.btn(@click='sendFollowRequest' v-if='followStatus.status == FollowStatusEnum.NOTFOLLOWING || followStatus.status == FollowStatusEnum.DECLINED' class='me-1')
                  span.material-icons(style='vertical-align: middle; font-size: 20px; margin-right: 6px;') person_add
                  |  Follow
                button.btn(v-else-if='followStatus.status == FollowStatusEnum.FOLLOWING' @click='unfollow' class='me-1' style='border: 1px solid')
                  span.material-icons(style='vertical-align: middle; font-size: 20px; margin-right: 6px;' alt='delet') person_off
                  |  Unfollow
                button.btn(v-if='followStatus.status == FollowStatusEnum.PENDING  && followStatus.respondingUserId == useUserStore.getUserId' class='me-1' @click='acceptFollowRequest' class='me-1' style='border: 1px solid')
                  |  Accept Follow Request
                button.btn(v-if='followStatus.status == FollowStatusEnum.PENDING && followStatus.respondingUserId == useUserStore.getUserId' class='me-1' @click='declineFollowRequest' style='border: 1px solid')
                  |  Decline Request
                button.btn(v-else-if='followStatus.status == FollowStatusEnum.PENDING  && followStatus.requestingUserId == useUserStore.getUserId' @click='cancelFollowRequest' class='me-1' style='border: 1px solid')
                  |  Cancel Follow Request
         
                button.btn(v-if='followStatus.status != FollowStatusEnum.BANNED' @click='banFollower' style='border: 1px solid' class='me-1')
                  |  Ban
                button.btn(v-else-if='followStatus.status == FollowStatusEnum.BANNED && useUserStore.getUserId == followStatus.requestingUserId' @click='removeBan' class='ms-1' style='border: 1px solid')
                  |  Remove Ban
                div(v-else-if='followStatus.status == FollowStatusEnum.BANNED && useUserStore.getUserId != followStatus.requestingUserId' class='text-center fs-5' class='me-1')
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

    CommentModal(:selectedPost='selectedPost' v-if='showCommentsModal' @close='showCommentsModal = false')
    LikesModal(:selectedPost='selectedPost' v-if='showLikesModal' @close='showLikesModal = false')
    FollowersModal(:selectedPost='selectedPost' :userId='userId' v-if='showFollowersModal' @close='showFollowersModal = false')

    ViewStoryModal(
      v-if="showStoryModal && story"
      :selectedStory="story"
      @close="closeStoryModal"
      @storyRemoved='storyRemoved'
    )
                
</template>

<script lang="ts">
import LeftSidebar from '@user/src/components/shared/left-sidebar.vue';
import ViewStoryModal from '@user/src/components/home/stories/view-story-modal.vue';
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PostLikeListDto } from '@user/src/models/posts/PostLikeListDto';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ScrollModel } from '@shared/models/ScrollModel';
import { toast } from '@user/src/helpers/toast';
import { FollowStatusEnum } from '@shared/models/followers/FollowStatusEnum';
import { StoryListModel } from '@shared/models/stories/StoryListModel';
import CommentModal from '../posts/comment-modal.vue';
import LikesModal from '../posts/likes-modal.vue';
import FollowersModal from '../posts/follower-modal.vue';

export default {
  components: { LeftSidebar, ViewStoryModal, LikesModal, CommentModal, FollowersModal },
  name: 'ProfileComponent',
  async created() {
    this.userId = parseInt(this.$route.query.userId as string);
    this.getUserInfo()
      .then(async () => {
        this.story = await this.getStoryByUserId(this.userId);
        this.totalFollowers = await this.getFollowerCount()
      });
    this.getFollowStatus()
    this.getPosts()
  },
  data() {
    return {
      totalPosts: 0,
      totalFollowers: 0,
      posts: [] as PostListDto[],
      user: new UserListDto(),
      postsScrollModel: new ScrollModel(),
      selectedPost: null as PostListDto | null,
      story: null as StoryListModel | null,
      followStatus: new FollowerListModel(),
      FollowStatusEnum,
      showStoryModal: false,
      showCommentsModal: false,
      showLikesModal: false,
      showFollowersModal: false,
      userId: parseInt(this.$route.query.userId as string),
      useUserStore: useUserStore(),
      gatewayUrl: import.meta.env.VITE_GatewayUrl,
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
        this.userId = parseInt(newVal)
        this.posts = []
        this.showFollower = false
        this.getFollowStatus()
        this.getUserInfo().then(async () => {
          this.story = await this.getStoryByUserId(this.userId)
          this.totalFollowers = await this.getFollowerCount()
        });
        this.getPosts()
        if (this.showCommentsModal)
          this.showCommentsModal = false
        if (this.showLikesModal)
          this.showLikesModal = false
        if (this.showFollowersModal)
          this.showFollowersModal = false
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
    openStoryModal() {
      this.showStoryModal = true;
    },
    closeStoryModal() {
      this.showStoryModal = false;
    },
    storyRemoved() {
      this.story = null;
      this.showStoryModal = false;
    },
    async getStoryByUserId(userId: number) {
      try {
        var storyResponse = await this.$axios.get(`/stories/${userId}`);
        storyResponse.data.data.user = new UserListDto();
        Object.assign(storyResponse.data.data.user, this.user);
        return storyResponse.data.data;
      } catch (error) {
        if (error.response && error.response.status === 404) {
          console.warn('No story found for user:', userId);
          return null;
        }
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
    async acceptFollowRequest() {
      try {
        const response = await this.$axios.post(`/followers/accept/${this.userId}`);
        if (!response.data.isError) {
          toast.success('successfully accepted!');
          this.followStatus.status = FollowStatusEnum.FOLLOWING;
        }
      } catch (error) {
        toast.warning(error.response.data.errorMessages.join(', ') || 'Could not accept follow request');
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
        if (error.response.data.statusCode == 403) {
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
    async getFollowerCount() {
      var followerCount = await this.$axios.get(`/followers/count?userId=${this.userId}`)
      return followerCount.data;
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
    openCommentsModal(post: PostListDto) {
      this.selectedPost = post;
      this.showCommentsModal = true;
    },
    openLikesModal(post: PostListDto) {
      this.selectedPost = post;
      this.showLikesModal = true;
    }
  }
}
</script>
