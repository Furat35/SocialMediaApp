<template>
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
          <!-- <button class="">Login</button> -->
          <router-link :to="{ name: 'login' }" class="ig-login-btn ig-nav-link"><span
              class="material-icons">login</span>
            Login</router-link>
        </div>
      </div>
    </nav>

    <router-view></router-view>
  </div>
</template>

<script lang="ts">
export default {
  name: 'InstagramClone',
  data() {
    return {
      stories: [
        { image: 'https://randomuser.me/api/portraits/women/1.jpg' },
        { image: 'https://randomuser.me/api/portraits/men/2.jpg' },
        { image: 'https://randomuser.me/api/portraits/women/3.jpg' },
        { image: 'https://randomuser.me/api/portraits/men/4.jpg' },
        { image: 'https://randomuser.me/api/portraits/women/5.jpg' }
      ],
      posts: [
        {
          username: 'jane_doe',
          profile: 'https://randomuser.me/api/portraits/women/1.jpg',
          image: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80',
          caption: 'Enjoying the view! 🌄',
          time: '2 hours ago'
        },
        {
          username: 'john_smith',
          profile: 'https://randomuser.me/api/portraits/men/2.jpg',
          image: 'https://images.unsplash.com/photo-1465101046530-73398c7f28ca?auto=format&fit=crop&w=800&q=80',
          caption: 'Great vibes only.',
          time: '3 hours ago'
        }
      ],
      suggestedUsers: [
        {
          username: 'alex_92',
          avatar: 'https://randomuser.me/api/portraits/men/32.jpg',
          desc: 'New to Instagram'
        },
        {
          username: 'sara_w',
          avatar: 'https://randomuser.me/api/portraits/women/44.jpg',
          desc: 'Suggested for you'
        },
        {
          username: 'mike_theo',
          avatar: 'https://randomuser.me/api/portraits/men/54.jpg',
          desc: 'Popular'
        }
      ]
    };
  }
}
</script>
<style>
@import '@user/src/assets/main.css';
</style>