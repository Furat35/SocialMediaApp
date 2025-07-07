<template>
  <!-- <div class="ig-bg"> -->
  <div class="ig-bg">
    <nav class="navbar ig-navbar">
      <div class="ig-navbar-inner  align-items-center justify-content-between">
        <div class="ig-navbar-section ig-navbar-left">
          <router-link :to="{ name: 'main-page' }">
            <img class="ig-logo"
              src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Instagram_logo.svg/2560px-Instagram_logo.svg.png"
              alt="Instagram" /></router-link>

        </div>
        <div class="ig-navbar-section ig-navbar-center">
          <input class="form-control ig-search" type="search" placeholder="Search" aria-label="Search" />
        </div>
        <div class="ig-navbar-section ig-navbar-right">
          <router-link :to="{ name: 'friend-requests' }" class="ig-login-btn ig-nav-link"><span
              class="material-icons">group</span>
            Friend Requests</router-link>

        </div>
      </div>
    </nav>

    <div class="ig-content">
      <router-view></router-view>
    </div>
  </div>
</template>

<script lang="ts">
import { useUserStore } from '../helpers/store';

export default {
  name: 'InstagramClone',
  computed: {
    userStore() {
      return useUserStore();
    },
    isAuthenticated() {
      return this.userStore.getIsAuthenticated;
    }
  }
}
</script>
<style>
@import '@user/src/assets/main.css';
</style>