<template>
    <div style="display: flex;margin-top: 40px;">
        <div style="width: 80px;" class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar></LeftSidebar>
        </div>

        <div class="ig-messages-container">
            <!-- Sidebar: User List -->
            <aside class="ig-messages-sidebar" @scroll="handleFollowersScroll">
                <div class="ig-messages-sidebar-header">Chats</div>
                <div v-for="follower in followers" :key="follower.user.id"
                    :class="['ig-messages-user', { active: selectedUser && selectedUser.id === follower.user.id }]"
                    @click="selectUser(follower.user)">
                    <img :src="`${gatewayUrl}users/image?userId=${follower.user.id}`" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ follower.user.fullname }}</span>
                </div>
            </aside>

            <!-- Chat Area -->
            <section class="ig-messages-chat" v-if="selectedUser">
                <div class="ig-messages-chat-header">
                    <img :src="`${gatewayUrl}users/image?userId=${selectedUser.id}`" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ selectedUser.fullname }}</span>
                </div>
                <div class="ig-messages-chat-body" ref="chatBody" @scroll="handleChatScroll">
                    <div v-for="(msg, idx) in messageHistory" :key="idx"
                        :class="['ig-message-bubble', msg.to == selectedUser.id ? 'self' : 'other']">
                        {{ msg.userMessage }}
                    </div>
                </div>
                <form class="ig-messages-chat-input" @submit.prevent="sendMessage">
                    <input v-model="newMessage" type="text" placeholder="Type a message..." autocomplete="off" />
                    <!-- <button type="submit" :disabled="!newMessage.trim()">Send</button> -->
                    <button @click="sendMessage()">gönder</button>
                </form>
            </section>

            <!-- Placeholder if no user selected -->
            <section class="ig-messages-chat ig-messages-placeholder" v-else>
                <div>Select a user to start chatting</div>
            </section>
        </div>
    </div>
</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import 'https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js'
import { useSignalRConnection } from '@user/src/services/signalr'
import { useUserStore } from '@user/src/helpers/store';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ChatListDto } from '@user/src/models/chats/chatListDto';
import { ScrollModel } from '@shared/models/ScrollModel';

export default {
    components: {
        LeftSidebar
    },
    data() {
        return {
            name: 'MessagePage',
            followers: [] as FollowerListModel[],
            selectedUser: null as UserListDto,
            chatScrollModel: new ScrollModel(),
            followerScrollModel: new ScrollModel(),
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            newMessage: '',
            messageHistory: [] as ChatListDto[],
            connection: useSignalRConnection()
        }
    },
    created() {
        this.getUserFollowers()
    },
    async mounted() {
        this.startSignalRConnection()
    },
    unmounted() {
        this.connection.stop();
    },
    methods: {
        handleChatScroll() {
            const el = this.$refs.chatBody as HTMLElement;
            if (!el) return;
            if (el.scrollTop < 250 && this.chatScrollModel.hasMore && !this.chatScrollModel.isLoading)
                this.getMessages(this.selectedUser.id);
        },
        handleFollowersScroll() {
            const el = this.$refs.followersList as HTMLElement;
            if (!el) return;
            const threshold = 200;
            if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerScrollModel.hasMore && !this.followerScrollModel.isLoading)
                this.getUserFollowers();
        },
        async startSignalRConnection() {
            this.connection.on("ReceiveMessage", (receivedMessage) => {
                if (this.selectedUser.id == receivedMessage.from) {
                    this.messageHistory.push({
                        to: receivedMessage.to,
                        from: receivedMessage.from,
                        userMessage: receivedMessage.userMessage
                    })
                }
                console.log("Message from server:", receivedMessage.from, receivedMessage);
            });
            try {
                await this.connection.start();
                console.log("SignalR connected");
            } catch (err) {
                console.error("SignalR connection error:", err);
            }
        },
        async getUserFollowers() {
            if (this.followerScrollModel.isLoading) return;
            this.followerScrollModel.isLoading = true;
            try {
                var response = await this.$axios.get(`/aggregated/followers/byUser?userId=${useUserStore().getUserId}&page=${this.followerScrollModel.currentPage}&pageSize=25`)
                if (response.data.data) {
                    const newFollowers = response.data.data.map(f => new FollowerListModel(f))
                    this.totalFollowers = response.data.totalEntities
                    this.followers.push(...newFollowers);
                    if (!response.data.hasNext) this.followerScrollModel.hasMore = false;
                    else this.followerScrollModel.currentPage++;
                }
            }
            finally {
                this.followerScrollModel.isLoading = false;
            }
        },
        async getMessages(userId: number) {
            if (this.chatScrollModel.isLoading) return;
            this.chatScrollModel.isLoading = true;
            try {
                var response = await this.$axios.get(`/chats/${userId}?page=${this.chatScrollModel.currentPage}&pageSize=25`)
                const messages = response.data.data.map(f => new ChatListDto(f)) as ChatListDto[]
                this.messageHistory.push(...messages)
                if (!response.data.hasNext) this.chatScrollModel.hasMore = false;
                else this.chatScrollModel.currentPage++;

                return messages;
            }
            finally {
                this.chatScrollModel.isLoading = false;
            }
        },
        async selectUser(user: UserListDto) {
            this.selectedUser = user;
            this.messageHistory = [] as UserListDto[]
            this.chatScrollModel.currentPage = 1
            this.chatScrollModel.isLoading = false
            this.chatScrollModel.hasMore = true
            await this.getMessages(this.selectedUser.id)
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        sendMessage() {
            if (!this.selectedUser || !this.newMessage.trim()) return;
            this.messageHistory.push({
                to: this.selectedUser.id,
                from: useUserStore().getUserId,
                userMessage: this.newMessage
            })
            this.connection.invoke("SendMessage", this.selectedUser.id, this.newMessage);
            this.newMessage = '';
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        scrollToBottom() {
            const chatBody = this.$refs.chatBody as HTMLElement;
            if (chatBody) {
                chatBody.scrollTop = chatBody.scrollHeight;
            }
        }
    }
}
</script>