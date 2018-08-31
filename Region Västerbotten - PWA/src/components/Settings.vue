<template>
  <v-container fluid fill-height class="centered">
    <v-layout align-center justify-center row wrap>
      <v-flex xs10>
        <v-card v-for="item in countries" :key="item.id">
          <v-card-media :src="item.image" class="mt-2 " :class="[{'active': item.isSelected}, {'inactive': !item.isSelected}]" height="160px"
            @click="switchLocale(item)">
          </v-card-media>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <v-btn color="green" class="animated fadeInLeftBig" @click="goTo(-1)" fab fixed bottom left :disabled="isAppLoading">
          <v-icon dark :class="[{'white--text':isAppLoading }]">arrow_back</v-icon>
        </v-btn>
      </v-flex>
    </v-layout>
    <v-snackbar :timeout="timeout" :color="color" :multi-line="mode === 'multi-line'" :vertical="mode === 'vertical'" v-model="snackbar">
      <div v-if="selectedCountry == 'sv'">
        Ändrade språk till Svenska
      </div>
      <div v-else-if="selectedCountry == 'en'">
        Changed language to English
      </div>
      <v-btn dark flat @click.native="snackbar = false">
        <v-icon>clear</v-icon>
      </v-btn>
    </v-snackbar>
  </v-container>
</template>

<script>
import { mapGetters, mapActions } from "vuex";

export default {
  data() {
    return {
      snackbar: false,
      color: "success",
      mode: "",
      selectedCountry: "",
      timeout: 2000,
      countries: {
        sweden: {
          image: require("../assets/Flag_of_Sweden.svg"),
          id: "sv",
          isSelected: true
        },
        uk: {
          image: require("../assets/Flag_of_the_United_Kingdom.svg"),
          id: "en",
          isSelected: false
        }
      },
      dataSelected: [],
      data: [
        {
          id: "1",
          alt: "Svenska"
        },
        {
          id: "2",
          alt: "English"
        }
      ]
    };
  },
  methods: {
    ...mapActions(["setLanguage", "isLoading"]),
    goTo(input) {
      this.$router.go(input);
    },
    openSnackbar(country) {
      this.snackbar = true;
      this.selectedCountry = country;
    },
    myVuexAction: function(value) {
      console.log(value);
    },
    switchLocale(item) {
      this.isLoading(true);
      this.snackbar = true;
      this.openSnackbar(item.id);
      this.countries.sweden.isSelected = false;
      this.countries.uk.isSelected = false;
      item.isSelected = true;
      this.setLanguage(item)
      this.$i18n.locale = item.id
      this.isLoading(false);
    }
  },
  computed: {
    displayLocale() {
      let other = "sv";
      if (this.$i18n.locale === "sv") {
        other = "en";
      }
      return other;
    },
    ...mapGetters(["isAppLoading", "languageId", "isLanguagueSwedish", "isLanguagueEnglish"])
  }
};
</script>

<style scoped>
.active {
  border: 5px solid #4caf50;
  transition: all 0.3s;
}

.inactive {
  opacity: 0.3;
  transition: all 0.3s;
}

.card__media {
  transition: all 0.3s;
}
</style>
