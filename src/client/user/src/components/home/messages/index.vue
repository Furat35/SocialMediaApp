<template lang="pug">
div(style='display: flex; margin-top: 40px;')
    .ig-sidebar.ig-sidebar-left-fixed(style='width: 80px;')
        LeftSidebar
    .ig-messages-container
        aside.ig-messages-sidebar(@scroll='handleFollowerChatsScroll' ref='followerChatsList')
            .ig-messages-sidebar-header.row(style='position: relative;width: 100%;')
                span Chats
                input.form-control.ig-search(type='search' placeholder='Search' aria-label='Search' v-model='searchQuery' @focus='onFocus' @input='onSearch' autocomplete='off')
            div(v-if='showDropdown && followerSearchList.length' v-for='follower in followerSearchList' :key='follower.user.id' :class="['ig-messages-user', { active: selectedUser && selectedUser.id === follower.user.id }]" @click='selectUser(follower.user)' ref='followerSearchList')
                img.ig-messages-user-avatar(:src='`${gatewayUrl}users/image?userId=${follower.user.id}`')
                span.ig-messages-user-name {{ follower.user.fullname }}
            .mt-3.text-center(v-else-if='showDropdown && !followerSearchList.length && searchQuery') No users found
            div(v-else='' v-for='follower in followerChats' :key='follower.id' :class="['ig-messages-user', { active: selectedUser && selectedUser.id === follower.id }]" @click='selectUser(follower)')
                img.ig-messages-user-avatar(:src='`${gatewayUrl}users/image?userId=${follower.id}`')
                span.ig-messages-user-name {{ follower.fullname }}
                // <span class="ms-auto text-danger" :ref="follower.id.toString()">+1</span>
            // Chat Area
        section.ig-messages-chat(v-if='selectedUser')
            .ig-messages-chat-header
                img.ig-messages-user-avatar(:src='`${gatewayUrl}users/image?userId=${selectedUser.id}`')
                span.ig-messages-user-name(@click='goToProfile(selectedUser.id)' style='cursor: pointer;') {{ selectedUser.fullname }}
            .ig-messages-chat-body(ref='chatBody' @scroll='handleChatScroll')
                div(v-for='(msg, idx) in chatHistory' :key='idx' :class="['ig-message-bubble', msg.to == selectedUser.id ? 'self' : 'other']" style='min-width: 120px;')
                    span  {{ msg.userMessage }} 
                    div(style='text-align: end;font-size: 0.7rem;' :title="msg.sentDate.toLocaleTimeString('en-En')") {{ msg.sentDate.toLocaleDateString(&apos;en-En&apos;) }}
            form.ig-messages-chat-input(@submit.prevent='sendMessage')
                input(v-model='newMessage' type='text' placeholder='Type a message...' autocomplete='off')
                // <button type="submit" :disabled="!newMessage.trim()">Send</button>
                button(@click='sendMessage()') gonder
            // Placeholder if no user selected
        section.ig-messages-chat.ig-messages-placeholder(v-else='')
            div Select a user to start chatting

</template>

<script lang="ts">
import LeftSidebar from '@user/src/components/shared/left-sidebar.vue';
import 'https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js'
import { useSignalRConnection } from '@user/src/services/signalr'
import { useUserStore } from '@user/src/helpers/store';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ChatListDto } from '@user/src/models/chats/chatListDto';
import { ScrollModel } from '@shared/models/ScrollModel';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';

export default {
    components: {
        LeftSidebar
    },
    data() {
        return {
            name: 'MessagePage',
            followerChats: [] as UserListDto[],
            selectedUser: null as UserListDto,
            chatScrollModel: new ScrollModel(),
            followerChatScrollModel: new ScrollModel(),
            followerScrollModel: new ScrollModel(),
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            newMessage: '',
            chatHistory: [] as ChatListDto[],
            followerSearchScroll: new ScrollModel(),
            followerSearchList: [] as FollowerListModel[],
            searchQuery: "",
            showDropdown: false,
            connection: useSignalRConnection()
        }
    },
    created() {
        this.getFollowerChats()
    },
    async mounted() {
        this.startSignalRConnection()
    },
    unmounted() {
        this.connection.stop();
    },
    methods: {
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        handleChatScroll() {
            const el = this.$refs.chatBody as HTMLElement;
            if (!el) return;
            if (el.scrollTop < 250 && this.chatScrollModel.hasMore && !this.chatScrollModel.isLoading)
                this.getChatHistory(this.selectedUser.id);
        },
        handleFollowerChatsScroll() {
            if (this.showDropdown) {
                const el = this.$refs.followerChatsList as HTMLElement;
                if (!el) return;
                const threshold = 200;
                console.log(el.scrollTop)
                if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerSearchScroll.hasMore && !this.followerSearchScroll.isLoading) {
                    this.getFollowers();
                }
            }
            else {
                const el = this.$refs.followerChatsList as HTMLElement;
                if (!el) return;
                const threshold = 200;
                if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerChatScrollModel.hasMore && !this.followerChatScrollModel.isLoading)
                    this.getFollowerChats();
            }

        },
        async startSignalRConnection() {
            this.connection.on("ReceiveMessage", (receivedMessage) => {
                const el = this.$refs[receivedMessage.from][0] as HTMLElement;
                if (el) {
                    this.chatHistory.push({
                        to: receivedMessage.to,
                        from: receivedMessage.from,
                        userMessage: receivedMessage.userMessage,
                        sentDate: new Date(receivedMessage.sentDate),
                    })
                }
                else this.getFollowerChats()

                console.log("Message from server:", receivedMessage.from, receivedMessage);
            });
            try {
                await this.connection.start();
                console.log("SignalR connected");
            } catch (err) {
                console.error("SignalR connection error:", err);
            }
        },
        async getFollowerChats() {
            if (this.followerChatScrollModel.isLoading) return;
            this.followerChatScrollModel.isLoading = true;
            try {
                var response = await this.$axios.get(`/aggregated/chats?page=${this.followerChatScrollModel.currentPage}&pageSize=25`)
                const newfollowerChats = response.data.data.map(u => new UserListDto(u))
                this.followerChats.push(...newfollowerChats);
                // this.getUnreadMessages(newfollowerChats.map(f => f.id));
                if (!response.data.hasNext) this.followerChatScrollModel.hasMore = false;
                else this.followerChatScrollModel.currentPage++;
            }
            finally {
                this.followerChatScrollModel.isLoading = false;
            }
        },
        async getChatHistory(userId: number) {
            if (this.chatScrollModel.isLoading) return;
            this.chatScrollModel.isLoading = true;
            try {
                var response = await this.$axios.get(`/chats/${userId}?page=${this.chatScrollModel.currentPage}&pageSize=25`)
                const messages = response.data.data.map(f => new ChatListDto(f)) as ChatListDto[]
                this.chatHistory.unshift(...messages.reverse());
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
            this.chatHistory = [] as UserListDto[]
            Object.assign(this.chatScrollModel, new ScrollModel())
            await this.getChatHistory(this.selectedUser.id)
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        sendMessage() {
            if (!this.selectedUser || !this.newMessage.trim()) return;
            this.chatHistory.push({
                to: this.selectedUser.id,
                from: useUserStore().getUserId,
                userMessage: this.newMessage,
                sentDate: new Date()
            })
            this.connection.invoke("SendMessage", this.selectedUser.id, this.newMessage);
            this.newMessage = '';
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        async getFollowers() {
            if (this.searchQuery) {
                if (this.followerSearchScroll.isLoading) return;
                this.followerSearchScroll.isLoading = true;
                try {
                    var search = this.searchQuery.toLowerCase()
                    var response = await this.$axios.get(`/aggregated/followers/search?searchkey=${search}&page=${this.followerSearchScroll.currentPage}&pageSize=25`)
                    const followers = response.data.data.map(u => new FollowerListModel(u));
                    this.followerSearchList.push(...followers)
                    // this.getUnreadMessages(followers.map(f => f.user.id));
                    if (!response.data.hasNext) this.followerSearchScroll.hasMore = false;
                    else this.followerSearchScroll.currentPage++;
                }
                finally {
                    this.followerSearchScroll.isLoading = false;
                }
            }
        },
        // async getUnreadMessages(userIds: number[]) {
        //     var response = await this.$axios.post(`/chats/get-unread-messages`, userIds)
        //     // console.log("Unread messages response:", response.data);
        // },
        closeModal() {
            this.showModal.value = false;
            this.searchQuery.value = '';
            this.followerSearchList.value = [];
        },
        async onSearch() {
            if (this.searchQuery) {
                if (this.followerSearchScroll.isLoading) return;
                Object.assign(this.followerSearchScroll, new ScrollModel())
                this.followerSearchScroll.isLoading = true;
                this.followerSearchList = [];
                var response = await this.$axios.get(`/aggregated/followers/search?searchkey=${this.searchQuery.toLowerCase()}&page=${this.followerSearchScroll.currentPage}&pageSize=20`)
                const users = response.data.data.map(u => new UserListDto(u));
                this.followerSearchList.push(...users)
                if (!response.data.hasNext) this.followerSearchScroll.hasMore = false;
                else this.followerSearchScroll.currentPage++;
                this.followerSearchScroll.isLoading = false;
                this.showDropdown = true;
            } else {
                this.followerSearchList = [];
                this.showDropdown = false;
            }
        },
        onFocus() {
            if (this.followerSearchList.length) {
                this.showDropdown = true;
            }
        },
        onBlur() {
            setTimeout(() => {
                this.showDropdown = false;
            }, 150);
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