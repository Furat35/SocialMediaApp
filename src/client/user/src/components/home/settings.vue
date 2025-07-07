<template>
    <div class="ig-main-layout" style="margin: 0;">
        <aside class="ig-sidebar ig-sidebar-left-fixed">
            <LeftSidebar />
        </aside>
        <main class="ig-feed-main mt-5">
            <div>
                <input name="id" hidden>
                <div class="mb-3 row">
                    <label for="username" class="col-sm-4 col-form-label">Username</label>
                    <div class="col-sm-8">
                        <input type="text" class="form-control" id="username" name="username"
                            v-model="userUpdateModel.username">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="fullname" class="col-sm-4 col-form-label">Fullname</label>
                    <div class="col-sm-8">
                        <input type="text" class="form-control" id="fullname" name="fullname"
                            v-model="userUpdateModel.fullname">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="bio" class="col-sm-4 col-form-label">Bio</label>
                    <div class="col-sm-8">
                        <input type="text" class="form-control" id="bio" name="bio" v-model="userUpdateModel.bio">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="profileImage" class="col-sm-4 form-label">Select profile image</label>
                    <div class="col-sm-8 ">
                        <img v-if="previewImage" :src="previewImage" alt="Selected Image Preview"
                            style="max-width: 100px; margin-top: 10px;" />
                        <img v-else :src="`${gatewayUrl}users/image?userId=${userId}`" alt="Selected Image Preview"
                            style="max-width: 100px; margin-top: 10px;" />
                        <input class="form-control mt-1" type="file" id="profileImage" name="profileImage"
                            @change="handleFileChange" accept="image/*">

                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="password" class="col-sm-4 col-form-label">Password</label>
                    <div class="col-sm-8">
                        <input type="password" class="form-control" id="password" name="password"
                            v-model="userUpdateModel.password">
                    </div>
                </div>

                <div class="col-auto" style="text-align: center;">
                    <button type="submit" class="btn btn-primary mb-3" @click="updateUser">Update</button>
                </div>
            </div>
        </main>
    </div>
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