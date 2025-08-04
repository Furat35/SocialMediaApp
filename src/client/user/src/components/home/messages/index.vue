<template lang="pug">
div(style='display: flex; margin-top: 40px;')
    .ig-sidebar.ig-sidebar-left-fixed(style='width: 80px;')
        LeftSidebar
    .ig-messages-container
        aside.ig-messages-sidebar(@scroll='handleChatsScroll' ref='followerChatsList')
            .ig-messages-sidebar-header.row.mx-0(style='position: relative;width: 100%;')
                span Chats
                input.form-control.ig-search(type='search' placeholder='Search' aria-label='Search' v-model='searchQuery' @focus='onFocus' @input='onSearch' autocomplete='off')
            div(v-if='showDropdown && chatsSearchList.length' v-for='chat in chatsSearchList' :key='chat.user.id' 
                :class="['ig-messages-user', { active: selectedUser && selectedUser.id === chat.user.id }]" @click='selectUser(chat.user)' ref='chatsSearchList')
                img.ig-messages-user-avatar(:src='`${gatewayUrl}users/image?userId=${chat.user.id}`')
                span.ig-messages-user-name {{ chat.user.fullname }}
                span.ms-auto.text-danger(v-if='!chat.isRead') +1
            .mt-3.text-center(v-else-if='showDropdown && !chatsSearchList.length && searchQuery') No users found
            div(v-else v-for='chat in chats' :key='chat.user.id' :class="['ig-messages-user', { active: selectedUser && selectedUser.id === chat.user.id }]" @click='selectUser(chat.user)')
                img.ig-messages-user-avatar(:src='`${gatewayUrl}users/image?userId=${chat.user.id}`')
                span.ig-messages-user-name {{ chat.user.fullname }}
                //- span.ms-auto.text-danger(:ref='chat.user.id.toString()' v-if='hasUnreadMessages(chat.user.id)') +1
                span.ms-auto.text-danger(v-if='!chat.isRead') +1

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
            selectedUser: null as UserListDto,
            chatHistory: [] as ChatListDto[],
            chatHistoryScrollModel: new ScrollModel(),
            chats: [] as ChatListDto[],
            chatsScrollModel: new ScrollModel(),
            followerSearchScroll: new ScrollModel(),
            chatsSearchList: [] as ChatListDto[],
            searchQuery: "",
            newMessage: '',
            showDropdown: false,
            connection: useSignalRConnection(),
            userId: useUserStore().getUserId,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            lastLoadTime: 0
        }
    },
    created() {
        this.getChats()
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
            const now = performance.now();
            if (now - this.lastLoadTime > 500 && el.scrollTop < 250 && this.chatHistoryScrollModel.hasMore && !this.chatHistoryScrollModel.isLoading) {
                this.lastLoadTime = now;
                const prevHeight = el.scrollHeight;
                const prevTop = el.scrollTop;
                this.getChatHistory(this.selectedUser.id).then(() => {
                    this.$nextTick(() => {
                        const newHeight = el.scrollHeight;
                        const added = newHeight - prevHeight;
                        el.scrollTop = prevTop + added;
                    });
                });
            }
        },
        handleChatsScroll() {
            const el = this.$refs.followerChatsList as HTMLElement;
            if (!el) return;
            const threshold = 200;
            if (this.showDropdown) {
                if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.followerSearchScroll.hasMore && !this.followerSearchScroll.isLoading) {
                    this.searchFollower();
                }
            }
            else {
                if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.chatsScrollModel.hasMore && !this.chatsScrollModel.isLoading)
                    this.getChats();
            }
        },
        async startSignalRConnection() {
            this.connection.on("ReceiveMessage", (receivedMessage) => {
                // console.log(receivedMessage)
                // const el = this.$refs[receivedMessage.from][0] as HTMLElement;
                console.log(receivedMessage);
                if (this.selectedUser && this.selectedUser.id == receivedMessage.from) {
                    this.chatHistory.push({
                        to: receivedMessage.to,
                        from: receivedMessage.from,
                        userMessage: receivedMessage.userMessage,
                        sentDate: new Date(receivedMessage.sentDate),
                        isRead: true
                    })
                }
                else if (this.chats.find(c => c.user.id === receivedMessage.from)) {
                    const chat = this.chats.find(c => c.user.id === receivedMessage.from);
                    if (chat) {
                        // chat.lastMessage = receivedMessage.userMessage;
                        // chat.sentDate = new Date(receivedMessage.sentDate);
                        chat.isRead = false;
                    }
                }
                else this.getChats()

                console.log("Message from server:", receivedMessage.from, receivedMessage);
            });
            try {
                await this.connection.start();
                console.log("SignalR connected");
            } catch (err) {
                console.error("SignalR connection error:", err);
            }
        },
        async getChats() {
            if (this.chatsScrollModel.isLoading) return;
            this.chatsScrollModel.isLoading = true;
            try {
                var response = await this.$axios.get(`/aggregated/chats?page=${this.chatsScrollModel.currentPage}&pageSize=25`)
                const newChats = response.data.data.map(u => new ChatListDto(u))
                this.chats.push(...newChats);
                if (!response.data.hasNext) this.chatsScrollModel.hasMore = false;
                else this.chatsScrollModel.currentPage++;
            }
            finally {
                this.chatsScrollModel.isLoading = false;
            }
        },
        async getChatHistory(userId: number) {
            console.log("tets")
            console.log(this.chatHistoryScrollModel);
            if (this.chatHistoryScrollModel.isLoading) return;
            this.chatHistoryScrollModel.isLoading = true;
            let messages = [] as ChatListDto[];
            try {
                var response = await this.$axios.get(`/chats/${userId}?page=${this.chatHistoryScrollModel.currentPage}&pageSize=25`)
                messages = response.data.data.map(f => new ChatListDto(f)) as ChatListDto[]
                this.chatHistory.unshift(...messages.reverse());
                if (!response.data.hasNext) this.chatHistoryScrollModel.hasMore = false;
                else this.chatHistoryScrollModel.currentPage++;
            }
            finally {
                this.chatHistoryScrollModel.isLoading = false;
            }
            return messages;
        },
        async selectUser(user: UserListDto) {
            this.selectedUser = user;
            this.chatHistory = [] as UserListDto[]
            Object.assign(this.chatHistoryScrollModel, new ScrollModel())
            await this.setMessagesAsRead(this.selectedUser.id);
            await this.getChatHistory(this.selectedUser.id)
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        async setMessagesAsRead(userId: number) {
            await this.$axios.post(`/chats/set-read?userId=${userId}`)
            this.chats.find(c => c.user.id == userId).isRead = true;
            if (this.chatsSearchList.length > 0)
                this.chatsSearchList.find(c => c.user.id == userId).isRead = true;
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
        async searchFollower() {
            if (this.searchQuery) {
                if (this.followerSearchScroll.isLoading) return;
                this.followerSearchScroll.isLoading = true;
                try {
                    var search = this.searchQuery.toLowerCase()
                    var response = await this.$axios.post(`/aggregated/chats/lastChats-byUserIds?searchkey=${search}&page=${this.followerSearchScroll.currentPage}&pageSize=25`)
                    const followers = response.data.data.map(u => new ChatListDto(u));
                    this.chatsSearchList.push(...followers)
                    if (!response.data.hasNext) this.followerSearchScroll.hasMore = false;
                    else this.followerSearchScroll.currentPage++;
                }
                finally {
                    this.followerSearchScroll.isLoading = false;
                }
            }
        },
        closeModal() {
            this.showModal.value = false;
            this.searchQuery.value = '';
            this.chatsSearchList.value = [];
        },
        async onSearch() {
            if (this.searchQuery) {
                if (this.followerSearchScroll.isLoading) return;
                Object.assign(this.followerSearchScroll, new ScrollModel())
                this.followerSearchScroll.isLoading = true;
                var response = await this.$axios.post(`/aggregated/chats/lastChats-byUserIds?searchkey=${this.searchQuery.toLowerCase()}&page=${this.followerSearchScroll.currentPage}&pageSize=20`)
                this.chatsSearchList = [];
                const users = response.data.data.map(u => new ChatListDto(u));
                this.chatsSearchList.push(...users)
                if (!response.data.hasNext) this.followerSearchScroll.hasMore = false;
                else this.followerSearchScroll.currentPage++;
                this.followerSearchScroll.isLoading = false;
                this.showDropdown = true;
            } else {
                this.chatsSearchList = [];
                this.showDropdown = false;
            }
        },
        onFocus() {
            if (this.chatsSearchList.length) {
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