<template>
    <!-- Feed -->
    <div class="ig-main-layout">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>

        <main class="ig-feed-main">
            <!-- Stories -->
            <!-- <div class="ig-stories-bar">
                <div class="story-circle ig-story" v-for="(story, index) in stories" :key="index">
                    <img :src="story.image" alt="story" />
                </div>
            </div> -->
            <!-- Feed -->
            <div>
                <div class="post-card ig-post " v-for="(post, index) in posts" :key="index">
                    <div class="post-header ig-post-header">
                        <!-- <img :src="post.profile" alt="profile" /> -->
                        <span @click="goToProfile(post.user.id)" style="cursor: pointer;">
                            <img :src="`${gatewayUrl}users/image?userId=${post.userId}`" alt="profile" />
                            <strong>{{ post.user.fullname }}</strong>
                        </span>

                        <!-- <span class="material-icons ig-post-menu ms-auto">more_horiz</span> -->
                    </div>
                    <div class="post-image ig-post-image">
                        <img :src="`${post.imageUrl}`" alt="post" />
                    </div>
                    <div class="ig-post-actions p-3">
                        <span class="material-icons ig-action" @click="likePost(post)"
                            :style="{ color: post.likes.find(_ => _.user.id == userId) ? '#c13584' : '' }">
                            favorite_border</span>

                        <span class="material-icons ig-action"
                            @click="openCommentsModal(post)">chat_bubble_outline</span>
                        <!-- <span class="material-icons ig-action">send</span> -->
                    </div>
                    <div class="px-3 pb-2">
                        <strong @click="openLikesModal(post)" style="cursor: pointer;">{{ post.likes.length }}
                            Likes</strong><br>
                        <strong>{{ post.description }}</strong>
                        <div v-if="post.comments.length > 0">
                            <span @click="openCommentsModal(post)" class="ig-action">View all {{ post.comments.length }}
                                comments</span>
                        </div>

                        <div class="text-muted small mt-1">{{ post.createDate.toLocaleDateString('en-En') }}</div>
                    </div>
                </div>
                <div v-show="!postScrollModel.hasMore" class="mb-4 mt-5" style="text-align: center">Couldn't find
                    anymore
                    posts...
                </div>
            </div>
            <div v-if="showCommentsModal" class="comments-modal-backdrop" @click.self="closeCommentsModal">
                <div class="comments-modal">
                    <button class="close-modal-btn" @click="closeCommentsModal" aria-label="Close">&times;</button>
                    <div class="post-image">
                        <img :src="`${selectedPost.imageUrl}`" alt="post" />
                    </div>
                    <div v-if="selectedPost" class="comments-panel">
                        <div class="comments-list">
                            <div v-for="comment in selectedPost.comments" :key="comment.id" class="comment">
                                <span @click="goToProfile(comment.user.id)" style="cursor: pointer;">
                                    <img :src="`${gatewayUrl}users/image?userId=${comment.user.id}`" class="me-2"
                                        style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                                    <strong>{{ comment.user.username }}</strong> <span>{{ comment.userComment }}</span>
                                </span>
                                <span style="display: block;margin-right: auto;text-align: end;font-size: small;">
                                    {{ comment.createDate.toLocaleDateString('en-En') }}
                                </span>
                                <hr>
                            </div>
                        </div>
                        <div class="add-comment">
                            <input v-model="newComment" @keyup.enter="addComment" type="text"
                                placeholder="Add a comment..." />
                            <button @click="addComment" :disabled="!newComment.trim()">Post</button>
                        </div>
                    </div>
                </div>
            </div>

            <div v-if="showLikesModal" class="likes-modal-backdrop" @click.self="closeLikesModal">
                <div class="likes-modal">
                    <button class="close-modal-btn" @click="closeLikesModal" aria-label="Close">&times;</button>
                    <div v-if="selectedPost" class="likes-panel">
                        <div class="likes-list">
                            <div class="mb-3">
                                <strong>{{ selectedPost.likes.length }} Likes</strong>
                            </div>
                            <hr>
                            <div v-for="like in selectedPost.likes" :key="like.id" class="like mb-2">
                                <div @click="goToProfile(like.user.id)" style="cursor: pointer;">
                                    <img :src="`${gatewayUrl}users/image?userId=${like.user.id}`" class="me-2"
                                        style="border-radius: 50%;width: 25px;height: 25px;" alt="post" height="30px" />
                                    <strong>{{ like.user.username }}</strong>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </main>
        <FollowerSuggestions></FollowerSuggestions>
    </div>

</template>

<script lang="ts">
import LeftSidebar from '@user/src/components/shared/left-sidebar.vue';
import FollowerSuggestions from '@user/src/components/shared/follower-suggestions.vue';
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { PaginationModel } from '@shared/models/PaginationModel';
import { FollowerListModel } from '@shared/models/followers/FollowerListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PostLikeListDto } from '@user/src/models/posts/PostLikeListDto';
import { PostCommentListDto } from '@user/src/models/posts/PostCommentListDto';
import { UserListDto } from '@shared/models/users/UserListDto';
import { ScrollModel } from '@shared/models/ScrollModel';
import { toast } from '@user/src/helpers/toast';

export default {
    components: {
        LeftSidebar,
        FollowerSuggestions
    },
    name: 'InstagramClone',
    created() {
        this.getPosts();
    },
    data() {
        return {
            // stories: [
            //     { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
            //     { image: 'https://randomuser.me/api/portraits/men/2.jpg' },
            //     { image: 'https://randomuser.me/api/portraits/women/3.jpg' },
            //     { image: 'https://randomuser.me/api/portraits/men/4.jpg' },
            //     { image: 'https://randomuser.me/api/portraits/women/5.jpg' }
            // ],
            posts: [] as PostListDto[],
            followers: new PaginationModel<FollowerListModel>(),
            postScrollModel: new ScrollModel(),
            userId: useUserStore().getUserId,
            showCommentsModal: false,
            showLikesModal: false,
            selectedPost: null as PostListDto | null,
            newComment: '',
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
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        handleScroll() {
            const scrollBottom = window.innerHeight + window.scrollY;
            const threshold = document.body.offsetHeight - 600;

            if (scrollBottom >= threshold && this.postScrollModel.hasMore) {
                this.getPosts();
            }
        },
        async addComment() {
            if (!this.newComment.trim() || !this.selectedPost) return;
            try {
                var response = await this.$axios.post(`/posts/add-comment?postId=${this.selectedPost.id}&userComment=${this.newComment}`)
                if (!response.data.isError) {
                    this.selectedPost.comments.push(new PostCommentListDto({
                        postId: this.selectedPost.id,
                        userComment: this.newComment, createDate: new Date(), user: new UserListDto({ username: useUserStore().getUsername, id: useUserStore().getUserId })
                    }))
                    this.newComment = '';
                }
            } catch (error) {
                toast.warning(error.response.data.errorMessages.join(', ') || 'Could not add comment');
            }

        },
        openCommentsModal(post: PostListDto) {
            this.selectedPost = post;
            this.showCommentsModal = true;
        },
        closeCommentsModal() {
            this.showCommentsModal = false;
            this.selectedPost = null;
        },
        openLikesModal(post: PostListDto) {
            this.selectedPost = post;
            this.showLikesModal = true;
        },
        closeLikesModal() {
            this.showLikesModal = false;
            this.selectedPost = null;
        },
        async likePost(post: PostListDto) {
            if (post.likes.find(_ => _.user.id == this.userId)) {
                this.removeLike(post);
                return;
            }
            try {
                var response = await this.$axios.post(`/posts/like?postId=${post.id}`)
                if (!response.data.isError) {
                    var like = new PostLikeListDto({ user: new UserListDto({ id: this.userId }), postId: post.id });
                    post.likes.push(like)
                }
            } catch (error) {
                toast.warning(error.response.data.errorMessages.join(', ') || 'Could not like post');
            }
        },
        async removeLike(post: PostListDto) {
            var response = await this.$axios.post(`/posts/unlike?postId=${post.id}`)
            if (!response.data.isError)
                post.likes = post.likes.filter(_ => _.user.id != this.userId)
        },
        async getPosts() {
            if (this.postScrollModel.isLoading) return;
            this.postScrollModel.isLoading = true;

            try {
                var response = await this.$axios.get(`/aggregated/feeds?page=${this.postScrollModel.currentPage}&pageSize=5`)
                if (response.data.data) {
                    const rawPosts = response.data.data as PostListDto[];
                    const newPosts = rawPosts.map(p => new PostListDto(p))
                    await this.setPostImages(newPosts);
                    this.posts.push(...newPosts);

                    if (!response.data.hasNext) this.postScrollModel.hasMore = false;
                    else this.postScrollModel.currentPage++;
                }
            } catch (error) {
                toast.warning(error.response.data.errorMessages.join(', ') || 'Could not get the posts');
            } finally {
                this.postScrollModel.isLoading = false;
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