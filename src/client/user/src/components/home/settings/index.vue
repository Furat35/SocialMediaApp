<template lang="pug">
    .ig-main-layout(style='margin: 0;')
        aside.ig-sidebar.ig-sidebar-left-fixed
            LeftSidebar
        main.ig-feed-main.mt-5
            div
                input(name='id' hidden='')
                .mb-3.row
                    label.col-sm-4.col-form-label(for='username') Username
                    .col-sm-8
                        input#username.form-control(type='text' name='username' v-model='userUpdateModel.username')
                .mb-3.row
                    label.col-sm-4.col-form-label(for='fullname') Fullname
                    .col-sm-8
                        input#fullname.form-control(type='text' name='fullname' v-model='userUpdateModel.fullname')
                .mb-3.row
                    label.col-sm-4.col-form-label(for='bio') Bio
                    .col-sm-8
                        input#bio.form-control(type='text' name='bio' v-model='userUpdateModel.bio')
                .mb-3.row
                    label.col-sm-4.form-label(for='profileImage') Select profile image
                    .col-sm-8
                        img(v-if='previewImage' :src='previewImage' alt='Selected Image Preview' style='max-width: 100px; margin-top: 10px;')
                        img(v-else='' :src='`${gatewayUrl}users/image?userId=${userId}`' alt='Selected Image Preview' style='max-width: 100px; margin-top: 10px;')
                        input#profileImage.form-control.mt-1(type='file' name='profileImage' @change='handleFileChange' accept='image/*')
                .mb-3.row
                    label.col-sm-4.col-form-label(for='password') Password
                    .col-sm-8
                        input#password.form-control(type='password' name='password' v-model='userUpdateModel.password')
                .col-auto(style='text-align: center;')
                    button.btn.btn-primary.mb-3(type='submit' @click='updateUser') Update

</template>

<script lang="ts">
import { UserUpdateDto } from '@shared/models/users/UserUpdateDto';
import LeftSidebar from '@user/src/components/shared/left-sidebar.vue'
import { useUserStore } from '@user/src/helpers/store';
import { toast } from '@user/src/helpers/toast';

export default {
    components: {
        LeftSidebar
    },
    data() {
        return {
            userUpdateModel: new UserUpdateDto(),
            previewImage: null,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            userId: useUserStore().getUserId
        }
    },
    created() {
        this.getUser();
    },
    methods: {
        handleFileChange(event) {
            const file = event.target.files[0];
            if (!file) {
                this.previewImage = null;
                this.userUpdateModel.profileImage = null;
                return;
            }

            this.userUpdateModel.profileImage = file;

            const reader = new FileReader();
            reader.onload = e => {
                this.previewImage = e.target.result; // base64 data URL
            };
            reader.readAsDataURL(file);
        },
        async getUser() {
            var response = await this.$axios.get(`/users/${useUserStore().getUserId}`);
            this.userUpdateModel.username = response.data.data.username;
            this.userUpdateModel.fullname = response.data.data.fullname;
            this.userUpdateModel.bio = response.data.data.bio;
            this.previewImage = this.userUpdateModel.profileImage; // base64 data URL
            // if (this.userUpdateModel.profileImage) {
            //     const reader = new FileReader();
            //     reader.onload = e => {
            //         this.previewImage = e.target.result; // base64 data URL
            //     };
            //     reader.readAsDataURL(this.userUpdateModel.profileImage);
            // }
        },
        async updateUser() {
            const formData = new FormData()
            formData.append("password", this.userUpdateModel.password ?? '')
            formData.append("username", this.userUpdateModel.username)
            formData.append("fullname", this.userUpdateModel.fullname)
            formData.append("bio", this.userUpdateModel.bio)
            if (this.userUpdateModel.profileImage) {
                formData.append("profileImage", this.userUpdateModel.profileImage ?? '');
            }

            try {
                var response = await this.$axios.post(`/users/update`, formData);
                if (response.data.data && !response.data.isError) toast.success("Update succeded");
            }
            catch (err) {
                toast.error(response.data.errorMessages || 'Error occured during update');
            }

        }
    }
}
</script>