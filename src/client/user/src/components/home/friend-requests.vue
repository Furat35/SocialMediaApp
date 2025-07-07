<!-- filepath: c:\Projects\SocialMediaApp\src\client\user\src\components\home\friend-requests.vue -->
<template>
    <div class="ig-main-layout">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>
        <main class="ig-feed-main">
            <section class="friend-requests-section">
                <h2 style="text-align: center;">Friend Requests</h2>
                <div v-if="requests.length === 0" class="no-requests">
                    No friend requests at the moment.
                </div>
                <div v-else>
                    <div v-for="req in requests" :key="req.id" class="friend-request-card">
                        <img :src="`${gatewayUrl}users/image?userId=${req.user.id}`" class="friend-request-avatar" />
                        <div class="friend-request-info">
                            <strong @click="goToProfile(req.user.id)" style="cursor: pointer;">{{ req.user.fullname
                            }}</strong>
                            <div class="friend-request-username" @click="goToProfile(req.user.id)"
                                style="cursor: pointer;">@{{ req.user.username
                                }}</div>
                        </div>
                        <div class="friend-request-actions">
                            <button @click="acceptRequest(req.user.id)" class="accept-btn">Accept</button>
                            <button @click="declineRequest(req.user.id)" class="decline-btn">Decline</button>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>

<script lang="ts">
import { UserListDto } from '@shared/models/users/UserListDto';
import LeftSidebar from '../shared/left-sidebar.vue';

export default {
    name: 'FriendRequests',
    components: { LeftSidebar },
    data() {
        return {
            requests: [] as Array<{
                id: number, requestingUserId: number, user: UserListDto, createDate: Date
            }>,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            currentPage: 1
        }
    },
    created() {
        this.getFriendRequests();
    },
    methods: {
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        async getFriendRequests() {
            // Replace with your actual API endpoint
            const response = await this.$axios.get(`/aggregated/friends/follow-requests?page=${this.currentPage}&pageSize=12`);
            console.log(response)
            console.log(response.data.data)
            if (response.data && !response.data.isError) {
                Object.assign(this.requests, response.data.data)
                console.log(this.requests)
            }
        },
        async acceptRequest(userId: number) {
            await this.$axios.post(`/friends/accept/${userId}`);
            this.requests = this.requests.filter(r => r.user.id !== userId);
        },
        async declineRequest(userId: number) {
            await this.$axios.post(`/friends/decline/${userId}`);
            this.requests = this.requests.filter(r => r.user.id !== userId);
        }
    }
}
</script>

<style scoped>
.friend-requests-section {
    margin-top: 2rem;
    max-width: 500px;
    margin-left: auto;
    margin-right: auto;
}

.friend-request-card {
    display: flex;
    align-items: center;
    background: #fff;
    border: 1px solid #dbdbdb;
    border-radius: 8px;
    padding: 1rem;
    margin-bottom: 1rem;
    gap: 1rem;
}

.friend-request-avatar {
    width: 56px;
    height: 56px;
    border-radius: 50%;
    object-fit: cover;
    border: 2px solid #efefef;
}

.friend-request-info {
    flex: 1;
}

.friend-request-username {
    color: #888;
    font-size: 0.95rem;
}

.friend-request-actions {
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