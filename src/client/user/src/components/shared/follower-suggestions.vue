<template lang="pug">
    <!-- Right Sidebar -->
    aside.ig-sidebar.ig-sidebar-right
        .ig-suggestions
            .ig-suggestions-title Suggestions For You
            .ig-suggestion-row(v-for='user in suggestedUsers' :key='user.username')
                img.ig-suggestion-avatar(:src='user.avatar')
                .ig-suggestion-info
                    .ig-suggestion-username {{ user.username }}
                    .ig-suggestion-desc {{ user.desc }}
                button.btn.btn-link.ig-follower-btn Follow
</template>

<script lang="ts">
export default {
    data() {
        return {
            suggestedUsers: [
                {
                    username: 'alex_92',
                    avatar: 'https://randomuser.me/api/portraits/men/32.jpg',
                    desc: 'New to Instagram'
                },
                {
                    username: 'sara_w',
                    avatar: 'https://randomuser.me/api/portraits/women/44.jpg',
                    desc: 'Suggested for you'
                },
                {
                    username: 'mike_theo',
                    avatar: 'https://randomuser.me/api/portraits/men/54.jpg',
                    desc: 'Popular'
                }
            ]
        }
    }
}
</script>


<style>
.ig-suggestions {
    padding: 0px 20px 16px 20px;
    width: 100%;
    min-width: 240px;
    max-width: 320px;
}

.ig-suggestions-title {
    font-weight: 600;
    color: #8e8e8e;
    font-size: 16px;
    margin-bottom: 18px;
    letter-spacing: 0.5px;
}

.ig-suggestion-row {
    display: flex;
    align-items: center;
    margin-bottom: 18px;
    padding: 8px 0;
    border-radius: 8px;
    transition: background 0.18s;
}

.ig-suggestion-row:hover {
    background: #f5f5f5;
}

.ig-suggestion-avatar {
    width: 44px;
    height: 44px;
    border-radius: 50%;
    object-fit: cover;
    margin-right: 14px;
    border: 2px solid #efefef;
}

.ig-suggestion-info {
    flex: 1;
    min-width: 0;
}

.ig-suggestion-username {
    font-weight: 500;
    font-size: 15px;
    color: #262626;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.ig-suggestion-desc {
    font-size: 13px;
    color: #8e8e8e;
    margin-top: 2px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.ig-follower-btn {
    font-size: 14px;
    color: #fff;
    background: linear-gradient(90deg, #fd5949 0%, #d6249f 100%);
    font-weight: 600;
    border-radius: 6px;
    padding: 5px 16px;
    border: none;
    transition: background 0.2s, color 0.2s;
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.04);
    text-decoration: none;
}

.ig-follower-btn:hover {
    background: linear-gradient(90deg, #d6249f 0%, #fd5949 100%);
    color: #fff;
    text-decoration: none;
}
</style>
