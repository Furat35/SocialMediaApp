<template lang="pug">
aside.ig-sidebar.ig-sidebar-left
        nav.ig-nav-menu
                router-link.ig-nav-link.d-block(:to="{ name: 'main-page' }")
                        span.material-icons home
                router-link.ig-nav-link.d-block(:to="{ name: 'explore' }")
                        span.material-icons explore
                router-link.ig-nav-link.d-block(:to="{ name: 'message' }")
                        span.material-icons send
                router-link.ig-login-btn.ig-nav-link(:to="{ name: 'follower-requests' }")
                        span.material-icons group
                .ig-nav-link.d-block(@click='showPostCreateModal = true' style='cursor: pointer;')
                        span.material-icons add_box
                router-link.ig-nav-link.d-block(:to="{ name: 'profile', query: { userId: userStore.getUserId } }")
                        span.material-icons account_circle
                router-link.ig-nav-link.d-block(:to="{ name: 'login' }")
                        span.material-icons(@click='logout') logout
CreatePostComponent(:showPostCreateModal='showPostCreateModal' @closeModal='closeModal')

</template>

<script>
import { useUserStore } from '@user/src/helpers/store';
import CreatePostComponent from '@user/src/components/home/posts/create.vue'

export default {
        components: {
                CreatePostComponent
        },
        computed: {
                userStore() {
                        return useUserStore();
                }
        },
        data() {
                return {
                        showPostCreateModal: false
                }
        },
        methods: {
                logout() {
                        this.userStore.logout();
                        this.$router.push({ name: 'login' });
                },
                closeModal() {
                        this.showPostCreateModal = false;
                        console.log(this.showPostCreateModal);
                }
        }
}
</script>