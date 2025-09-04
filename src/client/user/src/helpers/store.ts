import { LoginResponseModel } from '@shared/models/auth-models/LoginResponseModel'
import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    userInfo: new LoginResponseModel(),
  }),
  getters: {
    getUsername: (state) => state.userInfo.username,
    getUserId: (state) => state.userInfo.userId,
    getRefreshToken: (state) => state.userInfo.refreshToken,
    getAccessToken: (state) => state.userInfo.accessToken,
    getIsAuthenticated: (state) => !!(state.userInfo.accessToken && state.userInfo.refreshToken),
  },
  actions: {
    setUserInfo(loginResponseModel: LoginResponseModel) {
      Object.assign(this.userInfo, loginResponseModel)
    },
    setAccessToken(accessToken: string) {
      this.userInfo.accessToken = accessToken
    },
    setRefreshToken(refreshToken: string) {
      this.userInfo.refreshToken = refreshToken
    },
    logout() {
      this.$reset()
    },
  },
  persist: true,
})
