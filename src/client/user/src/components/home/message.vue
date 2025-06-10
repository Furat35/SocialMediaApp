<template>
    <div style="display: flex;">
        <div style="width: 80px;">
            <LeftSidebar></LeftSidebar>
        </div>

        <div class="ig-messages-container">
            <!-- Sidebar: User List -->
            <aside class="ig-messages-sidebar">
                <div class="ig-messages-sidebar-header">Chats</div>
                <div v-for="user in users" :key="user.id"
                    :class="['ig-messages-user', { active: selectedUser && selectedUser.id === user.id }]"
                    @click="selectUser(user)">
                    <img :src="user.avatar" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ user.name }}</span>
                </div>
            </aside>

            <!-- Chat Area -->
            <section class="ig-messages-chat" v-if="selectedUser">
                <div class="ig-messages-chat-header">
                    <img :src="selectedUser.avatar" class="ig-messages-user-avatar" />
                    <span class="ig-messages-user-name">{{ selectedUser.name }}</span>
                </div>
                <div class="ig-messages-chat-body" ref="chatBody">
                    <div v-for="(msg, idx) in messages[selectedUser.id]" :key="idx"
                        :class="['ig-message-bubble', msg.fromSelf ? 'self' : 'other']">
                        {{ msg.text }}
                    </div>
                </div>
                <form class="ig-messages-chat-input" @submit.prevent="sendMessage">
                    <input v-model="newMessage" type="text" placeholder="Type a message..." autocomplete="off" />
                    <button type="submit" :disabled="!newMessage.trim()">Send</button>
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

export default {
    components: {
        LeftSidebar
    },
    data() {
        return {
            name: 'MessagePage',
            users: [
                {
                    id: 1,
                    name: 'Jane Doe',
                    avatar: 'https://randomuser.me/api/portraits/women/1.jpg',
                },
                {
                    id: 2,
                    name: 'John Smith',
                    avatar: 'https://randomuser.me/api/portraits/men/2.jpg',
                },
                {
                    id: 3,
                    name: 'Sara White',
                    avatar: 'https://randomuser.me/api/portraits/women/44.jpg',
                },
            ],
            selectedUser: null,
            newMessage: '',
            messages: {
                1: [
                    { text: 'Hi Jane!', fromSelf: true },
                    { text: 'Hello! How are you?', fromSelf: false },
                ],
                2: [
                    { text: 'Hey John!', fromSelf: true },
                    { text: 'Hey! What\'s up?', fromSelf: false },
                ],
                3: [
                    { text: 'Hi Sara!', fromSelf: true },
                    { text: 'Hi there!', fromSelf: false },
                ],
            }
        }
    },
    created() {
        this.selectedUser = this.users[0];
    },
    methods: {
        selectUser(user) {
            this.selectedUser = user;
            this.$nextTick(() => {
                this.scrollToBottom();
            });
        },
        sendMessage() {
            if (!this.selectedUser || !this.newMessage.trim()) return;
            this.messages[this.selectedUser.id].push({
                text: this.newMessage,
                fromSelf: true,
            });
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

<style scoped>
.ig-messages-container {
    display: flex;
    height: calc(100vh - 60px);
    background: #fafafa;
    overflow: hidden;
    width: 100%;
    margin: 10 auto;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.04);
}

.ig-messages-sidebar {
    width: 230px;
    background: #fff;
    border-right: 1px solid #dbdbdb;
    display: flex;
    flex-direction: column;
    padding: 0;
}

.ig-messages-sidebar-header {
    font-weight: 600;
    font-size: 18px;
    padding: 20px 18px 14px 18px;
    border-bottom: 1px solid #dbdbdb;
    color: #262626;
}

.ig-messages-user {
    display: flex;
    align-items: center;
    padding: 14px 18px;
    cursor: pointer;
    transition: background 0.18s;
    border-bottom: 1px solid #f5f5f5;
}

.ig-messages-user:last-child {
    border-bottom: none;
}

.ig-messages-user.active,
.ig-messages-user:hover {
    background: #f5f5f5;
}

.ig-messages-user-avatar {
    width: 42px;
    height: 42px;
    border-radius: 50%;
    object-fit: cover;
    margin-right: 14px;
    border: 2px solid #efefef;
}

.ig-messages-user-name {
    font-weight: 500;
    font-size: 15px;
    color: #262626;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.ig-messages-chat {
    flex: 1;
    display: flex;
    flex-direction: column;
    background: #fafafa;
    min-width: 0;
}

.ig-messages-chat-header {
    display: flex;
    align-items: center;
    padding: 20px 18px 14px 18px;
    border-bottom: 1px solid #dbdbdb;
    background: #fff;
    font-weight: 600;
    font-size: 18px;
}

.ig-messages-chat-header .ig-messages-user-avatar {
    width: 27px;
    height: 27px;
    margin-right: 12px;
    border: 1px solid #dbdbdb;
}

.ig-messages-chat-body {
    flex: 1;
    padding: 24px 22px;
    overflow-y: auto;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.ig-message-bubble {
    max-width: 60%;
    padding: 10px 16px;
    border-radius: 18px;
    font-size: 15px;
    line-height: 1.5;
    word-break: break-word;
    margin-bottom: 2px;
    display: inline-block;
}

.ig-message-bubble.self {
    align-self: flex-end;
    background: linear-gradient(90deg, #fd5949 0%, #d6249f 100%);
    color: #fff;
    border-bottom-right-radius: 4px;
}

.ig-message-bubble.other {
    align-self: flex-start;
    background: #fff;
    color: #262626;
    border-bottom-left-radius: 4px;
    border: 1px solid #dbdbdb;
}

.ig-messages-chat-input {
    display: flex;
    padding: 16px 22px;
    border-top: 1px solid #dbdbdb;
    background: #fff;
    gap: 10px;
}

.ig-messages-chat-input input {
    flex: 1;
    border: 1px solid #dbdbdb;
    border-radius: 20px;
    padding: 10px 16px;
    font-size: 15px;
    outline: none;
    background: #fafafa;
    transition: border 0.2s;
}

.ig-messages-chat-input input:focus {
    border: 1.5px solid #a29bfe;
    background: #fff;
}

.ig-messages-chat-input button {
    background: linear-gradient(90deg, #fd5949 0%, #d6249f 100%);
    color: #fff;
    border: none;
    border-radius: 20px;
    padding: 0 22px;
    font-weight: 600;
    font-size: 15px;
    cursor: pointer;
    transition: background 0.2s;
}

.ig-messages-chat-input button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.ig-messages-placeholder {
    align-items: center;
    justify-content: center;
    color: #8e8e8e;
    font-size: 18px;
    display: flex;
}
</style>