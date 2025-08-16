<template lang="pug">
.likes-modal-backdrop
    .likes-modal
        button.close-modal-btn(@click='closeLikesModal' aria-label='Close') &times;
        .likes-panel(v-if='selectedPost')
            .likes-list
                .mb-3
                    strong {{ selectedPost.likes.length }} Likes
                hr
                .like.mb-2(v-for='like in selectedPost.likes' :key='like.id')
                    div(@click='goToProfile(like.user.id)' style='cursor: pointer;')
                        img.me-2(:src='`${gatewayUrl}users/image?userId=${like.user.id}`' style='border-radius: 50%;width: 25px;height: 25px;' alt='post' height='30px')
                        strong {{ like.user.username }}

</template>

<script lang="ts">
import { PostListDto } from '@user/src/models/posts/PostListDto';
import { useUserStore } from '@user/src/helpers/store';

export default {
    name: 'LikesModal',
    props: {
        selectedPost: {
            type: PostListDto,
            required: true,
        },
    },
    data() {
        return {
            userId: useUserStore().getUserId,
            showLikesModal: false,
            gatewayUrl: import.meta.env.VITE_GatewayUrl
        }
    },
    methods: {
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        closeLikesModal() {
            this.$emit('close');
        }
    }
}
</script>