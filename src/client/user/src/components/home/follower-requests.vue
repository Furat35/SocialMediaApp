<template>
    <div class="ig-main-layout">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>
        <main class="ig-feed-main">
            <section class="follower-requests-section">
                <h2 style="text-align: center;">Follow Requests</h2>
                <div v-if="followers.length === 0" class="no-requests">
                    No Follow requests at the moment.
                </div>
                <div v-else>
                    <div v-for="follower in followers" :key="follower.userId" class="follower-request-card">
                        <img :src="`${gatewayUrl}users/image?userId=${follower.user.id}`"
                            class="follower-request-avatar" />
                        <div class="follower-request-info">
                            <strong @click="goToProfile(follower.user.id)" style="cursor: pointer;">{{
                                follower.user.fullname
                            }}</strong>
                            <div class="follower-request-username" @click="goToProfile(follower.user.id)"
                                style="cursor: pointer;">@{{ follower.user.username
                                }}</div>
                        </div>
                        <div class="follower-request-actions">
                            <button @click="acceptRequest(follower.user.id)" class="accept-btn">Accept</button>
                            <button @click="declineRequest(follower.user.id)" class="decline-btn">Decline</button>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { ScrollModel } from '@shared/models/ScrollModel';

export default {
    name: 'FollowerRequests',
    components: { LeftSidebar },
    data() {
        return {
            followers: [] as FollowerListModel[],
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            followerScrollModel: new ScrollModel()
        }
    },
    created() {
        this.getFollowerRequests();
    },
    mounted() {
        window.addEventListener('scroll', this.handleFollowersScroll);
    },
    beforeUnmount() {
        window.removeEventListener('scroll', this.handleFollowersScroll);
    },
    methods: {
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        async handleFollowersScroll() {
            const scrollBottom = window.innerHeight + window.scrollY
            const threshold = document.body.offsetHeight - 150
            if (scrollBottom >= threshold && this.followerScrollModel.hasMore && !this.followerScrollModel.isLoading)
                this.getFollowerRequests()
        },
        async getFollowerRequests() {
            if (this.followerScrollModel.isLoading) return
            this.followerScrollModel.isLoading = true
            try {
                const response = await this.$axios.get(`/aggregated/followers/follow-requests?page=${this.followerScrollModel.currentPage}&pageSize=25`)
                if (response.data.data) {
                    const newFollowers = response.data.data.map(f => new FollowerListModel(f))
                    this.followers.push(...newFollowers);
                    if (!response.data.hasNext) this.followerScrollModel.hasMore = false;
                    else this.followerScrollModel.currentPage++;
                }
            }
            finally {
                this.followerScrollModel.isLoading = false;
            }
        },
        async acceptRequest(userId: number) {
            await this.$axios.post(`/followers/accept/${userId}`);
            this.followers = this.followers.filter(r => r.user.id !== userId);
        },
        async declineRequest(userId: number) {
            await this.$axios.post(`/followers/decline/${userId}`);
            this.followers = this.followers.filter(r => r.user.id !== userId);
        }
    }
}
</script>

<style scoped>
.follower-requests-section {
    margin-top: 2rem;
    max-width: 500px;
    margin-left: auto;
    margin-right: auto;
}

.follower-request-card {
    display: flex;
    align-items: center;
    background: #fff;
    border: 1px solid #dbdbdb;
    border-radius: 8px;
    padding: 1rem;
    margin-bottom: 1rem;
    gap: 1rem;
}

.follower-request-avatar {
    width: 56px;
    height: 56px;
    border-radius: 50%;
    object-fit: cover;
    border: 2px solid #efefef;
}

.follower-request-info {
    flex: 1;
}

.follower-request-username {
    color: #888;
    font-size: 0.95rem;
}

.follower-request-actions {
    display: flex;
    gap: 0.5rem;
}

.accept-btn {
    background: #3897f0;
    color: #fff;
    border: none;
    border-radius: 6px;
    padding: 0.5rem 1rem;
    cursor: pointer;
}

.decline-btn {
    background: #eee;
    color: #222;
    border: none;
    border-radius: 6px;
    padding: 0.5rem 1rem;
    cursor: pointer;
}

.no-requests {
    color: #888;
    text-align: center;
    margin-top: 2rem;
}
</style>