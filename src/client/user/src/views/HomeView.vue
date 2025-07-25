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
          <ul v-if="showDropdown && userSearchList.length" class="search-dropdown" ref="userSearchList"
            @scroll="searchDropdownScroll">
            <li v-for="user in userSearchList" :key="user.id" class="search-dropdown-item"
              @click="goToProfile(user.id)">
              <img :src="`${gatewayUrl}users/image?userId=${user.id}`" alt="profile"
                style="width: 25px;height: 25px;border-radius: 50%;" class="me-2" />
              {{ user.username }} <span style="font-size:0.7rem;font-weight: 100;">(@{{ user.fullname }})</span>
            </li>
          </ul>
          <div v-if="showDropdown && !userSearchList.length && searchQuery"
            class="search-dropdown search-dropdown-empty">
            No users found
          </div>
        </div>
        <div class="ig-navbar-section ig-navbar-right">


        </div>
      </div>
    </nav>

    <div class="ig-content">
      <router-view></router-view>
    </div>
  </div>
</template>

<script lang="ts">
import { UserListDto } from '@shared/models/users/UserListDto';
import { useUserStore } from '../helpers/store';
import { ScrollModel } from '@shared/models/ScrollModel';

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
      userSearchListScroll: new ScrollModel(),
      userSearchList: [] as UserListDto[],
      searchQuery: "",
      showDropdown: false,
      gatewayUrl: import.meta.env.VITE_GatewayUrl
    }
  },
  methods: {
    closeModal() {
      this.showModal.value = false;
      this.searchQuery.value = '';
      this.userSearchList.value = [];
    },
    searchDropdownScroll() {
      const el = this.$refs.userSearchList as HTMLElement;
      if (!el) return;
      const threshold = 200;
      if (el.scrollTop + el.clientHeight >= el.scrollHeight - threshold && this.userSearchListScroll.hasMore && !this.userSearchListScroll.isLoading) {
        this.getUsers();
      }
    },
    goToProfile(userId: number) {
      this.$router.push({ name: 'profile', query: { userId: userId } });
    },
    async onSearch() {
      if (this.searchQuery) {
        try {
          var search = this.searchQuery.toLowerCase()
          const response = await this.$axios.get(`users?page=${this.userSearchListScroll.currentPage}&pageSize=10&searchkey=${search}`)
          if (response.data.data) {
            const users = response.data.data.map(u => new UserListDto(u));
            Object.assign(this.userSearchListScroll, new ScrollModel())
            this.userSearchList = users
          }
        }
        finally {
          this.userSearchListScroll.isLoading = false;
        }
        this.showDropdown = true;
      } else {
        this.userSearchList = [];
        this.showDropdown = false;
      }
    },
    async getUsers() {
      if (this.searchQuery) {
        console.log(this.searchQuery)
        if (this.userSearchListScroll.isLoading) return;
        this.userSearchListScroll.isLoading = true;
        try {
          var search = this.searchQuery.toLowerCase()
          const response = await this.$axios.get(`users?page=${this.userSearchListScroll.currentPage}&pageSize=10&searchkey=${search}`)
          if (response.data.data) {
            const users = response.data.data.map(u => new UserListDto(u));
            this.userSearchList.push(...users)
            if (!response.data.hasNext) this.userSearchListScroll.hasMore = false;
            else this.userSearchListScroll.currentPage++;
          }
        }
        finally {
          this.userSearchListScroll.isLoading = false;
        }
      }
    },
    onFocus() {
      if (this.userSearchList.length) {
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