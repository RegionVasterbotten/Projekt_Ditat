<template>
  <v-container fluid fill-height>
    <v-layout flex align-center justify-center>
      <v-flex xs12 class="text-xs-center">
          <v-btn color="primary" class="animated fadeIn" large fab :disabled="isAppLoading">
            <v-icon dark :class="[{'white--text': isAppLoading }]">camera_alt</v-icon>
            <input type="file" name="myImage" accept="image/jpeg" class="input-button" @change="onFileChanged" />
          </v-btn>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import VueCamera from "@/components/camera.vue";
  import PictureInput from "vue-picture-input";
  import axios from "axios";
  import ImageCompressor from "image-compressor.js";
  import {
    mapActions,
    mapState,
    mapMutations,
    mapGetters
  } from "vuex";

  export default {
    data() {
      return {
        image: "",
        dialog3: false
      };
    },
    components: {
      "vue-camera": VueCamera,
      "picture-input": PictureInput
    },
    computed: {
      ...mapGetters(["theProductId", "isAppLoading"])
    },
    methods: {
      ...mapActions(["createProductId", "isLoading"]),
      goTo(input) {
        this.$router.push(input);
      },
      myCounter() {
        this.i = 0;
        this.init();
      },
      onRemoved() {
        this.image = "";
        location.reload();
      },
      imLoading(value) {
        this.isLoading(value);
      },
      async onFileChanged(event) {
        this.selectedFile = event.target.files[0];
        if (this.selectedFile) {
          this.imLoading(true);
          const imageCompressor = new ImageCompressor();
          const options = {
            quality: 0.8,
            width: 1900
          };
          const res = await imageCompressor.compress(this.selectedFile, options);
          const formData = new FormData();
          formData.append("image", res, res.name);
          try {
            let response = await this.createProductId(formData);
            this.$router.push("/detailview");
            this.imLoading(false);
          } catch (error) {
            this.imLoading(false);
            this.dialog3 = true;
          }
        }
      },
      async attemptUpload(event) {
        this.selectedFile = event.target.files[0];
        if (this.image) {
          this.imLoading(true);
          const imageCompressor = new ImageCompressor();
          const options = {
            quality: 0.8,
            width: 1900
          };
          const res = await imageCompressor.compress(this.selectedFile, options);
          const formData = new FormData();
          formData.append("image", res, res.name);

          try {
            let response = await this.createProductId(formData);
            this.imLoading(false);
            this.$router.push("/detailview");
          } catch (error) {
            this.imLoading(false);
            this.dialog3 = true;
          }
        }
      }
    }
  };

</script>

<style scoped>
  .camera-modal {
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    position: absolute;
  }

  .app--dialog {
    z-index: 10002;
  }

  .centered {
    text-align: center;
  }

  .camera-stream {
    width: 100%;
    height: 100vh;
  }

  .camera-modal-container {
    position: absolute;
    bottom: 0;
    width: 100%;
    align-items: center;
    margin-bottom: 24px;
  }

  .take-picture-button {
    display: flex;
  }

  .on-top {
    z-index: 999;
  }

  .over-image {
    margin-top: -25%;
  }


  .custom-file-upload {
    border: 1px solid #ccc;
    display: inline-block;
    padding: 6px 12px;
    cursor: pointer;
  }

  .label-as-button {
    -moz-border-radius: 50px/50px;
    -webkit-border-radius: 50px 50px;
    border-radius: 50px/50px;
    border: solid 21px #f00;
    width: 50px;
    height: 50px;
    -webkit-box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2),
    0px 6px 10px 0px rgba(0, 0, 0, 0.14), 0px 1px 18px 0px rgba(0, 0, 0, 0.12);
    box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2),
    0px 6px 10px 0px rgba(0, 0, 0, 0.14), 0px 1px 18px 0px rgba(0, 0, 0, 0.12);
    background-color: #4caf50 !important;
  }

</style>
