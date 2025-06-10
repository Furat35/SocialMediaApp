<template>
    <div class="login-container">
        <div class="login-card">
            <img class="login-logo"
                src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Instagram_logo.svg/2560px-Instagram_logo.svg.png"
                alt="Instagram" />
            <h2 class="login-title">Üye Ol</h2>
            <form @submit.prevent="handleLogin">
                <input v-model="username" type="text" class="login-input" placeholder="  Kullanıcı Adı" required />
                <input v-model="password" type="password" class="login-input" placeholder="  Şifre" required />
                <button type="submit" class="login-btn">Üye Ol</button>
            </form>
            <div class="login-footer">
                <span>Hesabın var mı?</span>
                <router-link :to="{ name: 'login' }" class="signup-link">Giriş</router-link>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
export default {
    name: 'LoginView',
    data() {
        return {
            username: '',
            password: ''
        }
    },
    methods: {
        handleLogin() {
            // Here you would typically handle the login logic, e.g., API call
            console.log('Logging in with:', this.username, this.password);
            // Reset fields after login attempt
            this.username = '';
            this.password = '';
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