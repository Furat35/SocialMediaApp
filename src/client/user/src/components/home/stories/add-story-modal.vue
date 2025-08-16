<template lang="pug">
    div.story-modal
        div.story-create-modal-content
            button.close-btn(@click="closeAddStoryModal" style="color:black") ×
            div.text-center.mt-5
                h1.modal-title.fs-5.mb-3 Create New Story
                div
                    .mb-3.row
                        label.col-sm-4.form-label(for='profileImage') Story image
                        .col-sm-6.me-2
                            img(v-if='previewImage' :src='previewImage' alt='Selected Image Preview' style='max-width: 100px; margin-top: 10px;')
                            input#profileImage.form-control.mt-1(type='file' @change='handleFileChange' accept='image/*')
                button.btn.btn-primary(type='button' @click='createStory') Post
</template>

<script lang="ts">
import { StoryCreateModel } from '@shared/models/stories/StoryCreateModel';
import { useUserStore } from '@user/src/helpers/store';
import { toast } from '@user/src/helpers/toast';

export default {
    name: 'AddStoryModal',
    data() {
        return {
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
            previewImage: null,
            storyCreateModal: new StoryCreateModel(),
        }
    },
    async created() {
    },
    methods: {
        async getSelectedUserInfo(userId: number) {
            var response = await this.$axios.get(`/users/${userId}`);
            Object.assign(this.selectedUser, response.data.data);
        },
        handleFileChange(event) {
            const file = event.target.files[0];
            if (!file) {
                this.previewImage = null;
                this.storyCreateModal.image = null;
                return;
            }

            this.storyCreateModal.image = file;

            const reader = new FileReader();
            reader.onload = e => {
                this.previewImage = e.target.result; // base64 data URL
            };
            reader.readAsDataURL(file);
        },
        async getUserInfo(userId: number) {
            var response = await this.$axios.get(`/users/${userId}`);
            return response.data.data;
        },
        async createStory() {
            const formData = new FormData()
            formData.append("file", this.storyCreateModal.image ?? '');

            try {
                var response = await this.$axios.post(`/stories`, formData);
                response.data.data.user = await this.getUserInfo(this.useUserStore.getUserId);
                this.$emit('storyCreated', response.data.data);
                toast.success("Story added successfully");
            }
            catch (err) {
                toast.error(response.data.errorMessages || 'Error occured during adding story');
            }
        },
        closeAddStoryModal() {
            this.$emit('close')
        }
    }
}
</script>

<style scoped>
.story-modal {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
}

.story-create-modal-content {
    background: #fff;
    border-radius: 18px;
    width: 400px;
    height: 60vh;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.25);
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.close-btn {
    position: absolute;
    top: 5px;
    right: 16px;
    background: none;
    border: none;
    font-size: 2rem;
    color: #888;
    cursor: pointer;
    transition: color 0.2s;
}

.close-btn:hover {
    color: #222;
}

.story-username {
    font-size: 0.8rem;
    font-weight: 600;
    color: #444444c5;
}

.user-info {
    display: flex;
    align-items: center;
    position: absolute;
    max-width: 270px;
    top: 10px;
    left: 10px;
    margin: 0 0 0.5rem 0;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.18);
    background: rgba(255, 255, 255, 0.396);
    border-radius: 8px;
    padding: 4px 10px;
}
</style>