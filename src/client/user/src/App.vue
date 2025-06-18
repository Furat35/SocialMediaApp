<template>
  <RouterView />
  <Loading :isBusy="isBusy" />
</template>

<script lang="ts">
import Loading from '@shared/components/Loading.vue';
export default {
  components: {
    Loading
  },
  data() {
    return {
      isBusy: false,
      refreshTokenInterval: null as ReturnType<typeof setInterval> | null
    }
  },
  mounted() {
    this.$bus.on('isBusy', this.handleEvent);

    // this.refreshTokenInterval = setInterval(() => {


    //   this.$axios.post('/auth/refresh-token')
    //     .then(response => {
    //       if (response.data.isError) {
    //         this.$bus.emit('isBusy', false);
    //         this.$router.push({ name: 'login' });
    //       } else {
    //         this.$bus.emit('isBusy', false);
    //       }
    //     })
    //     .catch(() => {
    //       this.$bus.emit('isBusy', false);
    //       this.$router.push({ name: 'login' });
    //     });
    // }, 5 * 1000);
  },
  beforeUnmount() {
    this.$bus.off('isBusy');
    if (this.refreshTokenInterval) {
      clearInterval(this.refreshTokenInterval);
      this.refreshTokenInterval = null;
    }
  },
  methods: {
    handleEvent(isBusy: boolean) {
      this.isBusy = isBusy;
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
