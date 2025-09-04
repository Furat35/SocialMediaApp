<template lang="pug">
    .comments-modal-backdrop(@click.self='closeCommentsModal')
      .comments-modal
        button.close-modal-btn(@click='closeCommentsModal' aria-label='Close') &times;
        button.post-settings-modal-btn(
            v-if="selectedPost && selectedPost.userId === useUserStore.getUserId"
            @click="showMenu = !showMenu"
            aria-label="Open menu"
        )  ...
        div.post-settings-modal(
            v-if="showMenu"
            @click.self="showMenu = false"
            class="mb-2"
            style="height: 50px"
        )
            ul.post-settings-list
                li.post-settings-item(@click="removePost(selectedPost.id); showMenu = false") Remove Post
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
          div(class='mb-2')
            span(style='font-weight:bold;cursor:pointer' class='me-2'  @click='goToProfile(selectedPost.userId)') {{ selectedPost.user.fullname }} 
            span(style='text-align:justify') {{ selectedPost.description }}
            span(class='ms-auto d-block text-end').text-muted.small.mt-1 {{ selectedPost.createDate.toLocaleDateString(&apos;en-En&apos;) }}
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
            showMenu: false
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
        async removePost(postId: number) {
            var response = await this.$axios.delete(`/posts?Id=${postId}`,
                {
                    params: {
                        Id: postId
                    }
                }
            )
            if (!response.data.isError) {
                this.$emit('postRemoved', postId);
            }
            this.showMenu = false;
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

.post-settings-modal-btn {
    position: absolute;
    top: 0px;
    right: 50px;
    background: transparent;
    border: none;
    font-size: 30px;
    color: #888;
    cursor: pointer;
    z-index: 10;
    transition: color 0.2s;
}

.post-settings-modal-btn :hover {
    color: #222;
}

.post-settings-modal {
    position: absolute;
    top: 50px;
    right: 56px;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.15);
    min-width: 120px;
    max-width: 160px;
    z-index: 20;
}

.post-settings-list {
    list-style: none;
    margin: 0;
    padding: 0;
}

.post-settings-item {
    padding: 1rem;
    cursor: pointer;
    color: #0c0403;
    font-size: 1rem;
    transition: background 0.2s;
}

.post-settings-item:hover {
    background: #f7f7f7;
}
</style>