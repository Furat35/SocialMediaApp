<template>
    <aside class="ig-sidebar ig-sidebar-left">
        <nav class="ig-nav-menu"><a></a>
            <router-link :to="{ name: 'main-page' }" class="ig-nav-link d-block"><span
                    class="material-icons">home</span></router-link>
            <router-link :to="{ name: 'explore' }" class="ig-nav-link d-block"><span
                    class="material-icons">explore</span></router-link>
            <router-link :to="{ name: 'message' }" class="ig-nav-link d-block"><span
                    class="material-icons">send</span></router-link>
            <router-link :to="{ name: 'setting' }" class="ig-nav-link d-block"><span
                    class=" material-icons">settings</span></router-link>
            <router-link :to="{ name: 'login' }" class="ig-nav-link d-block"><span
                    class="material-icons">logout</span></router-link>

        </nav>
    </aside>
</template>

<script>
export default {

}
</script>