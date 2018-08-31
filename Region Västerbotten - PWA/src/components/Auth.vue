<template >
  <v-container grid-list-md fluid fill-height>
    <v-layout align-center justify-center row wrap text-xs-center>
      <form @submit="SignIn" text-xs-center>
        <v-text-field :disabled="isAppLoading" solo label="Användarnamn" type="text" v-model="username" :error-messages="errors.collect('name')" v-validate="'required|max:50'" data-vv-name="name" required class="mb-3"></v-text-field>
        <v-text-field :disabled="isAppLoading" solo label="Lösenord" type="password" v-model="password" :error-messages="errors.collect('password')" v-validate="'required|max:50'" data-vv-name="password" required></v-text-field>
        <v-btn type="submit" v-show="!isAppLoading"  color="green" class="  mt-3" fab :disabled="errors.any() || !isCompleted">
          <v-icon dark :class="[{'white--text': isAppLoading }]">check</v-icon>
          <v-icon dark :class="[{'white--text': isAppLoading }]">close</v-icon>
        </v-btn>
      </form>
      <v-snackbar :timeout="timeout" :color="color" :multi-line="mode === 'multi-line'" :vertical="mode === 'vertical'" v-model="responseError" text-xs-center>
        {{ text }}
        <v-btn dark flat @click.native="responseError = false">
          <v-icon>clear</v-icon>
        </v-btn>
      </v-snackbar>
    </v-layout>
  </v-container>
</template>

<script>
import Api from '../api.js'
import { mapGetters, mapActions } from "vuex";
import qs from "qs";

export default {
  $_veeValidate: {
    validator: "new"
  },
  data() {
    return {
      grant_type: "password",
      username: "",
      password: "",
      hidden: false,
      responseError: false,
      color: "error",
      mode: "mode === 'vertical'",
      timeout: 6000,
      text: "Fel användarnamn eller lösenord",
      tabs: null,
      dictionary: {
        custom: {
          name: {
            required: () => "Name can not be empty",
            max: "The name field may not be greater than 50 characters"
          },
          password: {
            required: () => "Password can not be empty",
            max: "The password field may not be greater than 50 characters"
          }
        }
      }
    };
  },
  mounted() {
    this.$validator.localize("en", this.dictionary);
  },
  computed: {
    isCompleted() {
      return this.username && this.password;
    },
    ...mapGetters(["isAppLoading", "isLoggedIn"])
  },
  methods: {
    ...mapActions(["isLoading", "login"]),
    handleErrors(response) {
      if (!response.ok) throw new Error(response.statusText);
      return response;
    },
    clear() {
      this.name = "";
      this.email = "";
      this.select = null;
      this.checkbox = null;
      this.$validator.reset();
    },
    async SignIn() {
      if (this.$validator.validateAll()) {
        try {
          this.isLoading(true);
          let response = await fetch("https://xapi.hi5.se/token", {
            method: "POST",
            headers: {
              "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8"
            },
            body: qs.stringify({
              grant_type: this.grant_type,
              username: this.username,
              password: this.password
            })
          });
          let data = await response.json();
          console.log(data)

          if (!data.access_token) {
            this.responseError = true;
            this.isLoading(false);
          } else {
            let res = await this.login({ data, router: this.$router })
          }
        } catch (error) {
          console.log(error);
        }
      }
    }
  }
};
</script>

<style scoped>
</style>
