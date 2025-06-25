<template>
    <!-- Feed -->
    <div class="ig-main-layout">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>

        <main class="ig-feed-main">
            <!-- Stories -->
            <div class="ig-stories-bar">
                <div class="story-circle ig-story" v-for="(story, index) in stories" :key="index">
                    <img :src="story.image" alt="story" />
                </div>
            </div>
            <!-- Feed -->
            <div>
                <div class="post-card ig-post " v-for="(post, index) in posts" :key="index">
                    <div class="post-header ig-post-header">
                        <!-- <img :src="post.profile" alt="profile" /> -->
                        <img :src="stories[0].image" alt="profile" />
                        <strong>{{ post.user.fullname }}</strong>
                        <span class="material-icons ig-post-menu ms-auto">more_horiz</span>
                    </div>
                    <div class="post-image ig-post-image">
                        <img :src="`${post.imageUrl}`" alt="post" />
                    </div>
                    <div class="ig-post-actions p-3">
                        <span class="material-icons ig-action">favorite_border</span>
                        <span class="material-icons ig-action">chat_bubble_outline</span>
                        <span class="material-icons ig-action">send</span>
                    </div>
                    <div class="px-3 pb-2">
                        <strong>{{ post.description }}</strong>
                        <div class="text-muted small mt-1">{{ post.createDate }}</div>
                    </div>
                </div>
                <div v-show="!hasMore" class="mb-4" style="text-align: center;">Başka paylaşım bulunmadı...
                </div>
            </div>
        </main>
        <FriendSuggestions></FriendSuggestions>
    </div>

</template>

<script lang="ts">
import LeftSidebar from '../shared/left-sidebar.vue';
import FriendSuggestions from '../shared/friend-suggestions.vue';
import { PostListDto } from '@user/src/models/PostListDto';
import { PaginationModel } from '@shared/models/PaginationModel';
import { FriendListModel } from '@shared/models/friends/FriendListModel';

export default {
    components: {
        LeftSidebar,
        FriendSuggestions
    },
    name: 'InstagramClone',
    created() {
        this.getPosts();
    },
    data() {
        return {
            stories: [
                { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/2.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/3.jpg' },
                { image: 'https://randomuser.me/api/portraits/men/4.jpg' },
                { image: 'https://randomuser.me/api/portraits/women/5.jpg' }
            ],
            posts: [] as PostListDto[],
            friends: new PaginationModel<FriendListModel>(),
            currentPage: 1,
            isLoading: false,
            hasMore: true,
            gatewayUrl: import.meta.env.VITE_GatewayUrl
        }
    },
    mounted() {
        window.addEventListener('scroll', this.handleScroll);
    },
    beforeUnmount() {
        window.removeEventListener('scroll', this.handleScroll);
    },
    methods: {
        handleScroll() {
            const scrollBottom = window.innerHeight + window.scrollY;
            const threshold = document.body.offsetHeight - 500;

            if (scrollBottom >= threshold && this.hasMore) {
                this.getPosts();
            }
        },
        async getPosts() {
            if (this.isLoading) return;
            this.isLoading = true;

            try {
                var response = await this.$axios.get(`/feeds?page=${this.currentPage}&&pageSize=3`)

                if (response.data.data && !response.data.data.isError) {
                    const newPosts = response.data.data.data as PostListDto[];
                    await this.setPostImages(newPosts);
                    this.posts.push(...newPosts);

                    // Update pagination
                    if (!response.data.data.hasNext) {
                        this.hasMore = false;
                    } else {
                        this.currentPage++;
                    }

                } else {
                    console.error('Error fetching posts:', response.data.errorMessages);
                }
            } catch (error) {
                console.error('Error fetching posts:', error);
            } finally {
                this.isLoading = false;
            }
        },
        async setPostImages(postsToUpdate: PostListDto[]) {
            for (const post of postsToUpdate) {
                const imageResponse = await this.$axios.get(`/posts/image?postId=${post.id}`, {
                    responseType: 'blob',
                });

                post.imageUrl = URL.createObjectURL(imageResponse.data);
            }
        }
    },
}
</script>
