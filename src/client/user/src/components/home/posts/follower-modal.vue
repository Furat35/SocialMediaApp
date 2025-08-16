<template lang="pug">
.followers-modal-backdrop
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
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { useUserStore } from '@user/src/helpers/store';
import { ScrollModel } from '@shared/models/ScrollModel';

export default {
    name: 'FollowersModal',
    props: {
        userId: {
            type: Number,
            required: true,
        },
    },
    async created() {
        this.getUserFollowers()
    },
    data() {
        return {
            followers: [] as FollowerListModel[],
            followerScrollModel: new ScrollModel(),
            totalFollowers: 0,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
        }
    },
    methods: {
        goToProfile(followerId: number) {
            if (followerId != this.userId)
                this.$router.push({ name: 'profile', query: { userId: followerId } });
        },
        handleFollowersScroll() {
            const el = this.$refs.followersList as HTMLElement;
            if (!el) return;
            const threshold = 200;
            if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerScrollModel.hasMore && !this.followerScrollModel.isLoading) {
                this.getUserFollowers();
            }
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
        closeFollowersModal() {
            this.$emit('close');
        }
    }
}
</script>