<template>
    <div class="register-container">
        <div class="register-card">
            <img class="register-logo"
                src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Instagram_logo.svg/2560px-Instagram_logo.svg.png"
                alt="Instagram" />
            <h2 class="register-title">Register</h2>
            <form @submit.prevent="handleRegister">
                <input v-model="registerModel.username" type="text" class="register-input px-2"
                    placeholder="Kullanıcı Adı" required />
                <input v-model="registerModel.fullname" type="text" class="register-input px-2"
                    placeholder="İsim Soyisim" required />
                <input v-model="registerModel.email" type="email" class="register-input px-2" placeholder="Email"
                    required />
                <input v-model="registerModel.password" type="password" class="register-input px-2"
                    placeholder="Password" required />
                <button type="submit" class="register-btn">Register</button>
            </form>
            <div class="register-footer">
                <span>Have an account?</span>
                <router-link :to="{ name: 'login' }" class="login-link">Login</router-link>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { RegisterRequestModel } from '@shared/models/auth-models/RegisterRequestModel';
import { toast } from '../helpers/toast';
import { nextTick } from 'vue';
import { RegisterResponseModel } from '@shared/models/auth-models/RegisterResponseModel';

export default {
    name: 'RegisterView',
    data() {
        return {
            registerModel: new RegisterRequestModel(),
            registerResponseModel: new RegisterResponseModel(),
        }
    },
    methods: {
        handleRegister() {
            this.$bus.emit('isBusy', true);
            this.$axios.post('auth/register', this.registerModel)
                .then((response) => {
                    if (response.data.data) {
                        toast.success("Register succeded");
                        this.$router.push({ name: 'login' });
                        Object.assign(response.data.data, this.registerModel);
                    } else {
                        toast.error(response.data.errorMessages || 'Register failed');
                    }
                    nextTick(() => {
                        this.$bus.emit('isBusy', false);
                    });
                })
                .catch(error => {
                    toast.error(error.response?.data?.errorMessages || 'Error occured during register');
                    this.$bus.emit('isBusy', false);
                });
        }
    },
}
</script>

<style scoped>
.register-container {
    min-height: 100vh;
    background: #fafafa;
    display: flex;
    align-items: center;
    justify-content: center;
}

.register-card {
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

.register-logo {
    width: 180px;
    margin-bottom: 24px;
}

.register-title {
    font-size: 22px;
    font-weight: 600;
    margin-bottom: 24px;
    color: #262626;
}

.register-input {
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

.register-input:focus {
    border: 1.5px solid #a29bfe;
    background: #fff;
}

.register-btn {
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

.register-btn:hover {
    background: linear-gradient(90deg, #d6249f 0%, #fd5949 100%);
}

.register-footer {
    font-size: 14px;
    color: #8e8e8e;
    margin-top: 10px;
    text-align: center;
}

.login-link {
    color: #0095f6;
    text-decoration: none;
    margin-left: 6px;
    font-weight: 500;
}

.login-link:hover {
    text-decoration: underline;
}
</style>