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
          <input class="form-control ig-search" type="search" placeholder="Search" aria-label="Search"
            v-model="searchQuery" @focus="onFocus" @input="onSearch" @blur="onBlur" autocomplete="off" />
          <ul v-if="showDropdown && searchResults.length" class="search-dropdown">
            <li v-for="user in searchResults" :key="user.id" class="search-dropdown-item">
              {{ user.username }}
            </li>
          </ul>
          <div v-if="showDropdown && !searchResults.length && searchQuery"
            class="search-dropdown search-dropdown-empty">
            No users found.
          </div>
        </div>
        <div class="ig-navbar-section ig-navbar-right">
          <router-link :to="{ name: 'follower-requests' }" class="ig-login-btn ig-nav-link"><span
              class="material-icons">group</span>
            Follow Requests</router-link>

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
  },
  data() {
    return {
      searchQuery: "",
      searchResults: [],
      showDropdown: false
    }
  },
  methods: {
    closeModal() {
      this.showModal.value = false;
      this.searchQuery.value = '';
      this.searchResults.value = [];
    },
    async onSearch() {
      if (this.searchQuery) {
        // Replace this with your actual API call
        this.searchResults = [
          { id: 1, username: 'alice' },
          { id: 2, username: 'bob' },
          { id: 3, username: 'charlie' }
        ].filter(u => u.username.includes(this.searchQuery.toLowerCase()));
        this.showDropdown = true;
      } else {
        this.searchResults = [];
        this.showDropdown = false;
      }
    },
    onFocus() {
      if (this.searchResults.length) {
        this.showDropdown = true;
      }
    },
    onBlur() {
      setTimeout(() => {
        this.showDropdown = false;
      }, 150);
    }
  }
}
</script>
<style>
@import '@user/src/assets/main.css';
</style>