import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import router from './router/index'

Vue.use(Vuex)

import Api from './api'

const LOGIN_SUCCESS = "LOGIN_SUCCESS";
const LOGOUT = "LOGOUT";

export default new Vuex.Store({
  state: {
    productId: "",
    categories: [],
    productData: [],
    isLoading: false,
    isLoggedIn: !!localStorage.getItem('token'),
    languageSwedish: false,
    languageEnglish: false
  },
  mutations: {
    updateProductId(state, productId) {
      state.productId = productId
    },
    setProductIdData(state, payload) {
      state.productId = payload.data;
    },
    setCategories(state, payload) {
      state.categories = payload.data;
    },
    setProductData(state, payload) {
      state.productData = payload
    },
    setLoadingTrue(state, payload) {
      state.isLoading = payload
    },
    setLoadingFalse(state, payload) {
      state.isLoading = payload
    },
    [LOGIN_SUCCESS](state) {
      state.isLoading = false;
      state.isLoggedIn = true
      router.replace('/')
    },
    [LOGOUT](state) {
      state.isLoggedIn = false
      router.replace('/login')
    },
    setLanguageSwedish(state, payload) {
      state.languageSwedish = payload
    },
    setLanguageEnglish(state, payload) {
      state.languageEnglish = payload
    }
  },
  getters: {
    isLanguagueSwedish: state => {
      return state.languageSwedish
    },
    isLanguagueEnglish: state => {
      return state.languageEnglish
    },
    currentLang: state => {
      return state.language
    },
    productId: state => {
      return state.productId
    },
    productData: state => {
      return state.productData
    },
    isAdmin: _ => {
      return localStorage.getItem('isAdmin')
    },
    theCategories: state => {
      return state.categories
    },
    isAppLoading: state => {
      return state.isLoading
    },
    isLoggedIn: state => {
      return state.isLoggedIn
    },
    language: state => {
      return state.language
    }
  },
  actions: {
    isLoading({
      commit
    }, payload) {
      commit('setLoadingTrue', payload)
    },
    setLanguage({
      commit
    }, payload) {
      if (payload.id == 'sv') {
        commit('setLanguageEnglish', false)
        commit('setLanguageSwedish', true)
        return;
      } else {
        commit('setLanguageSwedish', false)
        commit('setLanguageEnglish', true)
      }
    },
    logout({ commit }) {
      localStorage.removeItem('token')
      localStorage.removeItem('isAdmin');
      commit(LOGOUT)
    },
    login({
      commit
    }, {data, router}) {
      if (data.access_token) {
        localStorage.removeItem('token');
        localStorage.removeItem('isAdmin');
        localStorage.setItem('token', data.access_token);
        localStorage.setItem('isAdmin', data.isAdmin);
        commit(LOGIN_SUCCESS);
        this.state.isLoggedIn = true
        router.push('/')
      } else {
        console.log("no token!")
      }
    },
    async addImageToProductById({
      commit
    }, payload) {
      const config = {
        headers: {
          'content-type': 'multipart/form-data'
        }
      }
      try {
        let id = localStorage.getItem('latestProductId')
        await Api().post(`/productImages/?productId=${id}&primaryImage=false`, payload)
      } catch (error) {
        console.log(error)
      }
    },
    async removeImage({
      commit
    }, payload) {
      try {
        await Api().delete(`/productimages/${payload}`)
        return
      } catch (error) {
        console.log(error)
      }
    },
    async makeImagePrimary({
      commit
    }, payload) {
      try {
        commit('setLoadingTrue', true)
        let formData = {
          Id: payload.Id,
          PrimaryImage: true,
          SelectedForExport: true
        }
        if (formData) {
          await Api().put(`/productImages/${payload.Id}`, formData)
          commit('setLoadingFalse', false)
        }
      } catch (error) {
        commit('setLoadingFalse', false)
        console.log(error)
      }
    },
    async getCategories({
      commit
    }, payload) {
      try {
        commit('setLoadingTrue', true)
        let result = await Api().get('/categories')
        commit('setCategories', result)
        commit('setLoadingFalse', false)
      } catch (error) {
        console.log(error)
      }
    },
    async updateProductById({
      commit,
      state
    }, payload) {
      try {
        commit('setLoadingTrue', true)
        let id = payload.Id
        console.log(Api)
        const response = await Api().put(`/products/${id}`, payload)
        commit('setLoadingFalse', false)
        return response
      }
      catch (error) {
        console.error('Error:', error)
        commit('setLoadingFalse', false)
      }
    },
    async getProducts({
      commit
    }, payload) {
      try {
        let result = await Api().get('products')
        commit('setProductData', result)
        return result
      } catch (error) {
        console.log(error)
      }
    },
    async createProductId({
      commit
    }, payload) {
      try {
        let result = await Api().post('/products', {})
        commit('setProductIdData', result)
        const id = result.data
        localStorage.setItem('latestProductId', id)

        await Api().post(`/productImages/?productId=${id}&primaryImage=true`, payload)
      } catch (error) {
        console.log(error)
        alert("Error: Något gick fel!")
        commit('setLoadingFalse', false)
      }
    },
  }
})
