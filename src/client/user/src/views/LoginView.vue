<template>
  <div class="login-container">
    <div class="login-card">
      <img class="login-logo"
        src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Instagram_logo.svg/2560px-Instagram_logo.svg.png"
        alt="Instagram" />
      <h2 class="login-title">Login</h2>
      <form @submit.prevent="handleLogin">
        <input v-model="loginModel.username" type="text" class="login-input px-2" placeholder="Username" required />
        <input v-model="loginModel.password" type="password" class="login-input px-2" placeholder="Password" required />
        <button type="submit" class="login-btn">Login</button>
      </form>
      <div class="login-footer">
        <span>Need an accound?</span>
        <router-link :to="{ name: 'register' }" class="signup-link">Register</router-link>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { LoginRequestModel } from '@shared/models/auth-models/LoginRequestModel';
import { toast } from '../helpers/toast';
import { nextTick } from 'vue';
import { LoginResponseModel } from '@shared/models/auth-models/LoginResponseModel';
import { useUserStore } from '../helpers/store';

export default {
  name: 'LoginView',
  data() {
    return {
      loginModel: new LoginRequestModel(),
    }
  },
  methods: {
    handleLogin() {
      new LoginResponseModel({})
      this.$bus.emit('isBusy', true);
      this.$axios.post('/auth/login', this.loginModel)
        .then((response) => {
          if (response.data.data && !response.data.isError) {
            toast.success("Login succeded");
            useUserStore().setUserInfo(response.data.data as LoginResponseModel)
            this.$router.push({ name: 'main-page' });
          } else {
            toast.error(response.data.errorMessages || 'Login failed');
          }
          nextTick(() => {
            this.$bus.emit('isBusy', false);
          });
        })
        .catch(error => {
          this.$bus.emit('isBusy', false);
          toast.error(error.response?.data?.errorMessages[0] || 'Error occured during login');
        });
    }
  },
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  background: #fafafa;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-card {
  background: #fff;
  border: 1px solid #dbdbdb;
  border-radius: 12px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.04);
  padding: 40px 32px 32px 32px;
  width: 350px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.login-logo {
  width: 180px;
  margin-bottom: 24px;
}

.login-title {
  font-size: 22px;
  font-weight: 600;
  margin-bottom: 24px;
  color: #262626;
}

.login-input {
  width: 100%;
  padding: 0;
  padding: 10px 0;
  margin-bottom: 14px;
  border: 1px solid #dbdbdb;
  border-radius: 6px;
  background: #fafafa;
  font-size: 15px;
  outline: none;
  transition: border 0.2s;
}

.login-input:focus {
  border: 1.5px solid #a29bfe;
  background: #fff;
}

.login-btn {
  width: 100%;
  padding: 10px 0;
  background: linear-gradient(90deg, #fd5949 0%, #d6249f 100%);
  color: #fff;
  font-weight: 600;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  cursor: pointer;
  margin-bottom: 18px;
  transition: background 0.2s;
}

.login-btn:hover {
  background: linear-gradient(90deg, #d6249f 0%, #fd5949 100%);
}

.login-footer {
  font-size: 14px;
  color: #8e8e8e;
  margin-top: 10px;
  text-align: center;
}

.signup-link {
  color: #0095f6;
  text-decoration: none;
  margin-left: 6px;
  font-weight: 500;
}

.signup-link:hover {
  text-decoration: underline;
}
</style>