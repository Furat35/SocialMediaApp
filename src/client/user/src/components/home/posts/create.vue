<template lang="pug">
   .modal.fade.show(v-if='showPostCreateModal' tabindex='-1' style='display: block; background: rgba(0,0,0,0.5);')
        .modal-dialog
            .modal-content
                .modal-header
                    h1#staticBackdropLabel.modal-title.fs-5 Create New Post
                    button.btn-close(type='button' @click='closeModal' aria-label='Close')
                .modal-body
                    div
                        input(name='id' hidden='')
                        .mb-3.row
                            label.col-sm-4.col-form-label(for='username') Description
                            .col-sm-8
                                input#description.form-control(type='text' v-model='postCreateModal.description')
                        .mb-3.row
                            label.col-sm-4.col-form-label(for='fullname') Location
                            .col-sm-8
                                input#location.form-control(type='text' v-model='postCreateModal.location')
                        .mb-3.row
                            label.col-sm-4.form-label(for='profileImage') Post image
                            .col-sm-8
                                img(v-if='previewImage' :src='previewImage' alt='Selected Image Preview' style='max-width: 100px; margin-top: 10px;')
                                input#profileImage.form-control.mt-1(type='file' @change='handleFileChange' accept='image/*')
                .modal-footer.justify-content-center
                    button.btn.btn-primary(type='button' @click='submitPost') Post

</template>

<script>
import { PostCreateDto } from '@user/src/models/posts/PostCreateDto';
import { useUserStore } from '@user/src/helpers/store';

export default {
    props: {
        showPostCreateModal: {
            type: Boolean,
            required: true
        }
    },
    data() {
        return {
            title: '',
            content: '',
            postCreateModal: new PostCreateDto(),
            userId: useUserStore().getUserId,
            previewImage: null,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
        };
    },
    methods: {
        closeModal() {
            this.title = '';
            this.content = '';
            this.$emit('closeModal');
        },
        handleFileChange(event) {
            const file = event.target.files[0];
            if (!file) {
                this.previewImage = null;
                this.postCreateModal.image = null;
                return;
            }

            this.postCreateModal.image = file;

            const reader = new FileReader();
            reader.onload = e => {
                this.previewImage = e.target.result; // base64 data URL
            };
            reader.readAsDataURL(file);
        },
        async submitPost() {
            const formData = new FormData()
            formData.append("description", this.postCreateModal.description ?? '')
            formData.append("location", this.postCreateModal.location)
            formData.append("file", this.postCreateModal.image ?? '');

            try {
                var response = await this.$axios.post(`/posts`, formData);
                if (response.data.data && !response.data.isError) toast.success("Update succeded");
                this.$emit('closeModal');
            }
            catch (err) {
                toast.error(response.data.errorMessages || 'Error occured during update');
            }

            // alert('Post created:\n' + JSON.stringify(post, null, 2));
            this.closeModal();
        }
    }
};
</script>