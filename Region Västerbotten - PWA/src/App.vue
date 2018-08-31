<template>
  <v-app id="inspire">
    <v-toolbar color="green" dark fixed app>
      <v-toolbar-title>{{$t(this.$route.name)}}</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-toolbar-side-icon @click.stop="drawer = !drawer" @click="openMenu" v-if="this.isLoggedIn"></v-toolbar-side-icon>
    </v-toolbar>


    <v-navigation-drawer v-model="drawer" absolute temporary>

      <v-list class="pt-0">
        <v-divider></v-divider>
        <v-list-tile @click="goTo('/')">
          <v-list-tile-action>
            <v-icon medium>camera_alt</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Ny bild</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>

        <v-divider></v-divider>

        <v-list-tile @click="goTo('/settings')">
          <v-list-tile-action>
            <v-icon medium>language</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Språk</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>

        <v-divider></v-divider>

        <v-list-tile @click="goTo('/admin')" v-if="pass == 'true'">
          <v-list-tile-action>
            <v-icon medium>settings</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Admin</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>


        <v-list-tile @click="logout">
          <v-list-tile-action>
            <v-icon medium>exit_to_app</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Logga ut</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
        <v-divider></v-divider>
      </v-list>


    </v-navigation-drawer>


    <v-content>
      <v-progress-linear v-if="isAppLoading" :indeterminate="true"></v-progress-linear>
      <transition name="page" mode="out-in" v-on:after-enter="afterEnter" appear>
        <router-view></router-view>
      </transition>
    </v-content>
  </v-app>
</template>
<script>
  import {
    mapGetters,
    mapActions
  } from "vuex";
  export default {
    data: () => ({
      drawer: null,
      pass: ''
    }),
    methods: {
      ...mapActions(["logout"]),
      afterEnter: function (el) {},
      goTo(input) {
        this.$router.push(input);
      },
      openMenu(){
        this.pass = localStorage.getItem('isAdmin')

        console.log("PASS")
        console.log(this.pass)
      }
    },
    mounted(){
      console.log(this.isAdmin)
    },
    computed: {
      ...mapGetters(["isAppLoading", "isLoggedIn", "currentLang", "isAdmin"])
    }
  };

</script>

<style>
  .drawer-ontop {
    z-index: 9999 !important;
  }

  .text-center {
    text-align: center;
  }

  .vue-select-image__thumbnail--selected {
    background: #4baf4f;
  }

  .jumbotron__image {
    width: 100%;
  }

  .input-button {
    height: 100%;
    z-index: 99999;
    width: 100%;
    opacity: 0 !important;
  }

  .error {
    background-color: #ff5252 !important;
    border-color: #ff5252 !important;
  }

  .progress-linear {
    background: transparent;
    margin: 0rem 0 !important;
    overflow: hidden;
    width: 100%;
    position: relative;
  }

  .text-error {
    color: #f44336;
  }

  .custom-loader {
    animation: loader 1s infinite;
    display: flex;
  }

  @-moz-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }

  @-webkit-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }

  @-o-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }

  @keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }

  @-webkit-keyframes animateThis {
    0% {
      opacity: 0;
    }
    100% {
      opacity: 1;
    }
  }

  .theme--light .input-group.input-group--solo,
  .application .theme--light.input-group.input-group--solo {
    background: #efefef !important;
  }

  .dummy-loading-large {
    width: 200px;
    background-color: #dcdcdc;
    height: 10px;
    border-radius: 50px;
    margin-bottom: 5px;
    -webkit-animation: animateThis 1s ease-in;
    -webkit-animation-fill-mode: forwards;
  }

  .carousel-loading {
    height: 150px !important;
  }

  .dummy-loading {
    width: 150px;
    background-color: #f1f1f1;
    height: 10px;
    border-radius: 50px;
    margin-bottom: 5px;
    -webkit-animation: animateThis 1s ease-in;
    -webkit-animation-fill-mode: forwards;
  }

  .dummy-loading-light {
    width: 100px;
    background-color: #dcdcdc;
    height: 10px;
    border-radius: 50px;
    margin-bottom: 5px;
    -webkit-animation: animateThis 1s ease-in;
    -webkit-animation-fill-mode: forwards;
  }

  .theme--dark .btn.btn--disabled:not(.btn--icon):not(.btn--flat),
  .application .theme--dark.btn.btn--disabled:not(.btn--icon):not(.btn--flat) {
    background-color: rgba(0, 0, 0, 0.12) !important;
  }

  /* Enter and leave animations can use different */

  /* durations and timing functions.              */

  .slide-fade-enter-active {
    transition: all 0.3s ease;
  }

  .slide-fade-leave-active {
    transition: all 0.8s cubic-bezier(1, 0.5, 0.8, 1);
  }

  .slide-fade-enter,
  .slide-fade-leave-to {
    transform: translateX(10px);
    opacity: 0;
  }

  .info {
    background-color: #2196f3 !important;
    border-color: #2196f3 !important;
  }

  .carousel-3d-slide.current {
    opacity: 1 !important;
    visibility: visible !important;
    transform: none !important;
    width: 205px !important;
    height: 260px !important;
    margin: 0 0 0 79px;
    z-index: 999;
    box-shadow: 0px 1px 15px rgba(0, 0, 0, 0.3);
  }

  .container.grid-list-xs .layout .flex {
    padding: 0px !important;
  }

  .carousel__controls {
    background: rgba(0, 0, 0, 0.1) !important;
  }

  .next,
  .prev {
    color: #fff !important;
  }

  .carousel-3d-slide {
    transition: all 0.3s !important;
    border-radius: 0px !important;
    border-color: #000 !important;
    border-color: rgba(0, 0, 0, 0) !important;
  }

  .container.grid-list-xs {
    padding: 0px !important;
  }

  .vue-select-image__thumbnail--selected {
    background: #fff;
    border: 2px solid #4caf50;
  }

  .vue-select-image__item {
    margin: 6px;
    float: left;
  }

  .vue-select-image__wrapper {
    overflow: auto;
    list-style-image: none;
    list-style-position: outside;
    list-style-type: none;
    padding: 0;
    width: 170px;
    margin: auto;
  }

  .page-enter-active,
  .page-leave-active {
    transition: opacity 0.5s, transform 0.3s;
  }

  .page-enter,
  .page-leave-to {
    opacity: 0;
    transform: translateX(-30%);
  }

  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 0.5s;
  }

  .fade-enter,
  .fade-leave-to {
    opacity: 0;
  }

  body {
    margin: 0;
  }

  .picture-preview {
    background-color: rgba(200, 200, 200, 0) !important;
  }

  #app {
    font-family: "Avenir", Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #2c3e50;
  }

  .carousel {
    height: 350px !important;
  }

  main {
    margin-top: 40px;
    padding: 16px 0px 0px !important;
  }

  .loading-overlay {
    z-index: 10002;
  }

  .loading-overlay .loading-background {
    background: #000;
  }

  .loading-overlay .loading-icon:after {
    border-top-color: #fff;
  }

  header {
    margin: 0;
    height: 56px;
    padding: 0 16px 0 24px;
    background-color: #35495e;
    color: #ffffff;
  }

  header span {
    display: block;
    position: relative;
    font-size: 20px;
    line-height: 1;
    letter-spacing: 0.02em;
    font-weight: 400;
    box-sizing: border-box;
    padding-top: 16px;
  }

  .text-center {
    text-align: center;
  }

  .slide-fade-enter-active {
    transition: all 0.3s ease;
  }

  .slide-fade-leave-active {
    transition: all 0.8s cubic-bezier(1, 0.5, 0.8, 1);
  }

  .slide-fade-enter,
  .slide-fade-leave-to {
    transform: translateX(10px);
    opacity: 0;
  }

</style>

<style lang="stylus">
  @import '~vuetify/src/stylus/main';

</style>
