<template>
    <div style="display: flex;margin-top: 40px;">
        <div style="width: 80px;" class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar></LeftSidebar>
        </div>

        <div class="ig-messages-container">
            <!-- Sidebar: User List -->
            <aside class="ig-messages-sidebar" ref="friendsList" @scroll="handleFriendsScroll">
                <div class="ig-messages-sidebar-header">Chats</div>
                <div v-for="friend in friends" :key="friend.userId"
                    :class="['ig-messages-user', { active: selectedUser && selectedUser.id === friend.userId }]"
                    @click="selectUser(friend.user)">
                    <img :src="`${gatewayUrl}users/image?userId=${friend.userId}`" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ friend.user.fullname }}</span>
                </div>
            </aside>

            <!-- Chat Area -->
            <section class="ig-messages-chat" v-if="selectedUser">
                <div class="ig-messages-chat-header">
                    <img :src="`${gatewayUrl}users/image?userId=${selectedUser.id}`" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ selectedUser.fullname }}</span>
                </div>
                <div class="ig-messages-chat-body" ref="chatBody" @scroll="handleScroll">
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
import { FriendListModel } from '@shared/models/friends/FriendListModel';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ChatListDto } from '@user/src/models/chats/chatListDto';

export default {
    components: {
        LeftSidebar
    },
    data() {
        return {
            name: 'MessagePage',
            friends: [] as FriendListModel[],
            selectedUser: null as UserListDto,
            currentPage: 1,
            isLoading: false,
            hasMore: true,
            friendsCurrentPage: 1,
            friendsIsLoading: false,
            friendsHasMore: true,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            newMessage: '',
            messageHistory: [] as ChatListDto[],
            connection: useSignalRConnection()
        }
    },
    created() {
        this.getUserFriends()
    },
    async mounted() {
        this.startSignalRConnection()
    },
    unmounted() {
        this.connection.stop();
    },
    methods: {
        handleScroll() {
            const el = this.$refs.chatBody as HTMLElement;
            if (!el) return;
            console.log(el.scrollTop)
            if (el.scrollTop < 250 && this.hasMore && !this.isLoading)
                this.getMessages(this.selectedUser.id);
        },
        handleFriendsScroll() {
            const el = this.$refs.friendsList as HTMLElement;
            if (!el) return;
            const threshold = 200;
            if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.friendsHasMore && !this.friendsIsLoading)
                this.getUserFriends();
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
        async getUserFriends() {
            if (this.friendsIsLoading) return;
            this.friendsIsLoading = true;
            try {
                var response = await this.$axios.get(`/aggregated/friends/byUser?userId=${useUserStore().getUserId}&page=${this.friendsCurrentPage}&pageSize=25`)
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
        async getMessages(userId: number) {
            if (this.isLoading) return;
            this.isLoading = true;
            try {
                var response = await this.$axios.get(`/chats/${userId}?page=${this.currentPage}&pageSize=25`)
                const messages = response.data.data.map(f => new ChatListDto(f)) as ChatListDto[]
                console.log(messages)
                this.messageHistory.push(...messages)
                console.log(response.data)
                if (!response.data.hasNext) this.hasMore = false;
                else this.currentPage++;

                return messages;
            }
            finally {
                this.isLoading = false;
            }
        },
        async selectUser(user: UserListDto) {
            this.selectedUser = user;
            this.messageHistory = [] as UserListDto[]
            this.currentPage = 1
            this.isLoading = false
            this.hasMore = true
            await this.getMessages(this.selectedUser.id)
            // this.messageHistory.push(...messages)
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