<template lang="pug">
    .comments-modal-backdrop(@click.self='closeCommentsModal')
      .comments-modal
        button.close-modal-btn(@click='closeCommentsModal' aria-label='Close') &times;
        .post-image
          img(:src='`${selectedPost.imageUrl}`' alt='post')
        .comments-panel(v-if='selectedPost')
          .comments-list
            .comment(v-for='comment in selectedPost.comments' :key='comment.id')
              span(@click='goToProfile(comment.user.id)' style='cursor: pointer;')
                img.me-2(:src='`${gatewayUrl}users/image?userId=${comment.user.id}`' style='border-radius: 50%;width: 25px;height: 25px;' alt='post' height='30px')
                strong(class='me-1') {{ comment.user.username }}
                span {{ comment.userComment }}
              span(style='display: block;text-align: end;font-size: small')
                | {{ comment.createDate.toLocaleDateString(&apos;en-En&apos;) }}
                span(@click='deleteComment(comment)' style='cursor:pointer' class='ms-2 text-danger' v-if='comment.user.id == useUserStore.getUserId') Delete
              hr
          .add-comment
            input(v-model='newComment' @keyup.enter='addComment' type='text' placeholder='Add a comment...')
            button(@click='addComment' :disabled='!newComment.trim()') Post
</template>

<script lang="ts">
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { useUserStore } from '@user/src/helpers/store';
import { PostCommentListDto } from '@user/src/models/posts/PostCommentListDto';
import { UserListDto } from '@shared/models/users/UserListDto';

export default {
    name: 'CommentModal',
    props: {
        selectedPost: {
            type: PostListDto,
            required: true,
        },
    },
    data() {
        return {
            newComment: '',
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
        }
    },
    methods: {
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        async addComment() {
            if (!this.newComment.trim() || !this.selectedPost) return;
            var response = await this.$axios.post(`/posts/add-comment?postId=${this.selectedPost.id}&userComment=${this.newComment}&followerId=${this.selectedPost.userId}`);
            if (!response.data.isError) {
                this.selectedPost.comments.push(new PostCommentListDto({
                    postId: this.selectedPost.id, userComment: this.newComment,
                    user: new UserListDto({ id: this.useUserStore.getUserId, username: this.useUserStore.getUsername }), createDate: new Date()
                }))
                this.newComment = '';
            }
        },
        async deleteComment(comment: PostCommentListDto) {
            var response = await this.$axios.post(`/posts/remove-comment?postId=${comment.postId}&commentId=${comment.id}&followerId=${comment.user.id}`);
            if (!response.data.isError) {
                this.selectedPost.comments = this.selectedPost.comments.filter(c => c.id != comment.id);
            }
        },
        closeCommentsModal() {
            this.$emit('close');
        },
    },
}
</script>

<style>
.follower-list {
    max-height: 400px;
    overflow-y: auto;
}

.btn-gradient-follow {
    background: linear-gradient(90deg, #f58529 0%, #dd2a7b 50%, #8134af 100%);
    color: #fff;
    border: none;
    border-radius: 24px;
    padding: 8px 24px;
    font-weight: 500;
    font-size: 1rem;
    box-shadow: 0 2px 8px rgba(221, 42, 123, 0.08);
    transition: background 0.2s, box-shadow 0.2s;
    display: inline-flex;
    align-items: center;
    cursor: pointer;
}
</style>