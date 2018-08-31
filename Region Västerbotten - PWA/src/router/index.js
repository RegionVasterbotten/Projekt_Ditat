import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import CameraView from '@/components/CameraView'
import DetailView from '@/components/DetailView'
import Settings from '@/components/Settings'
import Admin from '@/components/Admin'
import Auth from '@/components/Auth'
import AuthGuard from './auth-guard'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Capture Image',
      component: Home,
      beforeEnter: AuthGuard
    },
    {
      path: '/login',
      name: 'Login',
      component: Auth,

    },
    {
      path: '/settings',
      name: 'Settings',
      component: Settings,
      beforeEnter: AuthGuard
    },
    {
      path: '/admin',
      name: 'Admin',
      component: Admin,
      beforeEnter: AuthGuard
    },
    {
      path: '/camera',
      name: 'Camera',
      component: CameraView,
      beforeEnter: AuthGuard
    },
    {
      path: '/detailview',
      name: 'New Product',
      component: DetailView,
      beforeEnter: AuthGuard
    }
  ]
})
