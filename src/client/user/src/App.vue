<template>
  <RouterView />
  <Loading :isBusy="isBusy" />
</template>

<script lang="ts">
import Loading from '@shared/components/Loading.vue';
import { RefreshTokenRequestModel } from '@shared/models/auth-models/RefreshTokenRequestModel';
import { useUserStore } from './helpers/store';
export default {
  components: {
    Loading
  },
  data() {
    return {
      isBusy: false,
      refreshTokenInterval: null as ReturnType<typeof setInterval> | null,
      refreshTokenErrorCount: 0
    }
  },
  mounted() {
    this.$bus.on('isBusy', this.handleEvent);
    this.$bus.on('login-succeded', this.loginSucceded);
  },
  beforeUnmount() {
    this.$bus.off('isBusy');
    if (this.refreshTokenInterval) {
      clearInterval(this.refreshTokenInterval)
    }
  },
  methods: {
    handleEvent(isBusy: boolean) {
      this.isBusy = isBusy;
    },
    loginSucceded() {
      this.refreshTokenErrorCount = 0;
      this.refreshTokenInterval = setInterval(() => {
        if (this.refreshTokenErrorCount > 4) {
          this.$router.push({ name: 'login' });
          clearInterval(this.refreshTokenInterval)
          return;
        }
        this.$axios.post('/auth/refresh-token',
          new RefreshTokenRequestModel(
            {
              accessToken: useUserStore().getAccessToken,
              refreshToken: useUserStore().getRefreshToken
            }))
          .then(response => {
            if (response.data.isError) {
              this.$router.push({ name: 'login' });
              this.refreshTokenErrorCount++;
            }
            else {
              useUserStore().setAccessToken(response.data.data.accessToken)
              useUserStore().setRefreshToken(response.data.data.refreshToken)
            }
          })
          .catch(() => {
            this.$router.push({ name: 'login' });
            this.refreshTokenErrorCount++;
          });
      }, 5 * 60 * 1000);
    }
  }
}
</script>

<style scoped>
header {
  line-height: 1.5;
  max-height: 100vh;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

nav {
  width: 100%;
  font-size: 12px;
  text-align: center;
  margin-top: 2rem;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

  nav {
    text-align: left;
    margin-left: -1rem;
    font-size: 1rem;

    padding: 1rem 0;
    margin-top: 1rem;
  }
}
</style>
