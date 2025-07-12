import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import MainPageComponent from '../components/home/main-page/index.vue'
import SettingsComponent from '../components/home/settings/index.vue'
import MessageComponent from '../components/home/messages/index.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ExploreComponent from '../components/home/explore/index.vue'
import ProfileComponent from '../components/home/profiles/index.vue'
import FollowerRequestComponent from '../components/home/follower-requests/index.vue'
import CreatePostComponent from '../components/home/posts/create.vue'

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
          name: 'follower-requests',
          path: '/follower-requests',
          component: FollowerRequestComponent,
        },
      ],
    },
    {
      name: 'create-post',
      path: '/create-post',
      component: CreatePostComponent,
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
