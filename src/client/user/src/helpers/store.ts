import { LoginResponseModel } from '@shared/models/auth-models/LoginResponseModel'
import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    userInfo: new LoginResponseModel(),
  }),
  getters: {
    getUsername: (state) => state.userInfo.username,
    getRefreshToken: (state) => state.userInfo.refreshToken,
    getAccessToken: (state) => state.userInfo.accessToken,
    getIsAuthenticated: (state) => !!(state.userInfo.accessToken && state.userInfo.refreshToken),
  },
  actions: {
    setUserInfo(loginResponseModel: LoginResponseModel) {
      Object.assign(this.userInfo, loginResponseModel)
    },
    logout() {
      this.$reset()
    },
  },
  persist: true,
})
