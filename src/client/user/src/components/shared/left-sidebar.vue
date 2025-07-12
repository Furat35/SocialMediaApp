<template>
        <aside class="ig-sidebar ig-sidebar-left">
                <nav class="ig-nav-menu"><a></a>
                        <router-link :to="{ name: 'main-page' }" class="ig-nav-link d-block"><span
                                        class="material-icons">home</span></router-link>
                        <router-link :to="{ name: 'explore' }" class="ig-nav-link d-block"><span
                                        class="material-icons">explore</span></router-link>
                        <router-link :to="{ name: 'message' }" class="ig-nav-link d-block"><span
                                        class="material-icons">send</span></router-link>
                        <router-link :to="{ name: 'follower-requests' }" class="ig-login-btn ig-nav-link"><span
                                        class="material-icons">group</span></router-link>
                        <router-link :to="{ name: 'create-post' }" class="ig-nav-link d-block"><span
                                        class=" material-icons">add_box</span></router-link>
                        <router-link :to="{ name: 'profile', query: { userId: userStore.getUserId } }"
                                class="ig-nav-link d-block">
                                <span class=" material-icons">account_circle</span></router-link>
                        <router-link :to="{ name: 'login' }" class="ig-nav-link d-block"><span class="material-icons"
                                        @click="logout">logout</span></router-link>

                </nav>
        </aside>
</template>

<script>
import { useUserStore } from '@user/src/helpers/store';
export default {
        computed: {
                userStore() {
                        return useUserStore();
                }
        },
        methods: {
                logout() {
                        this.userStore.logout();
                        this.$router.push({ name: 'login' });
                }
        }
}
</script>