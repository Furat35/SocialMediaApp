import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import MainPageComponent from '../components/home/main-page.vue'
import SettingsComponent from '../components/home/settings.vue'
import MessageComponent from '../components/home/message.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ExploreComponent from '../components/home/explore.vue'
import ProfileComponent from '../components/home/profile.vue'
import FriendRequests from '../components/home/friend-requests.vue'

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      children: [
        {
          name: 'main-page',
          path: '/',
          component: MainPageComponent,
        },
        {
          name: 'setting',
          path: '/settings',
          component: SettingsComponent,
        },
        {
          name: 'message',
          path: '/messages',
          component: MessageComponent,
        },
        {
          name: 'explore',
          path: '/explore',
          component: ExploreComponent,
        },
        {
          name: 'profile',
          path: '/profile',
          component: ProfileComponent,
        },
        {
          name: 'friend-requests',
          path: '/friend-requests',
          component: FriendRequests,
        },
      ],
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
    },
  ],
})

export default router
