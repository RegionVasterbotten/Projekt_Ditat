import store from '../store'

export default (to, from, next) => {
  if (store.state.isLoggedIn) {
    next()
  } else {
    next('/login')
  }
}
