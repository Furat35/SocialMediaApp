<template>
  <div class="loading-overlay" v-if="isBusy">
    <div class="spinner"></div>
  </div>
</template>

<script>
export default {
  name: 'Loading',
  props: {
    isBusy: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      type: Boolean,
      required: true,
    }
  },
}
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(255, 255, 255, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.spinner {
  width: 48px;
  height: 48px;
  border: 5px solid #ccc;
  border-top: 5px solid #3490dc;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
