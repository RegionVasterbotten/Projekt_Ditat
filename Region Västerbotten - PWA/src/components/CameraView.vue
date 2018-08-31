<template>
  <div>
    <v-container fluid>
      <v-layout row wrap>
        <v-flex xs12>
          <picture-input type="file" id="file" ref="pictureInput" @change="onChanged" @remove="onRemoved" :width="500" :hideChangeButton="true"
            :removable="false" :height="500" buttonClass="btn info" removeButtonClass="btn btn--floating error" :customStrings="{
            upload: '<h1>Upload it!</h1>',
            tap: `<a class='btn btn--floating btn--large btn--router theme--dark green'><div class='btn__content'><i aria-hidden='true' class='icon material-icons'>camera_alt</i> </div></a>`,
            change: 'Ändra',
            remove: 'X'
            }">
          </picture-input>
        </v-flex>
      </v-layout>
      <v-layout row wrap class="mt-5">
        <v-flex xs12>
          <v-btn large fab @click="onRemoved" color="red" :disabled="!image">
            <v-icon dark>delete</v-icon>
          </v-btn>
          <v-btn large fab @click="compressAndUpload" color="primary" :disabled="!image">
            <v-icon dark>check</v-icon>
          </v-btn>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script>
  import VueCamera from "@/components/camera.vue";
  import PictureInput from "vue-picture-input";
  import Api from '../api.js'
  import ImageCompressor from "image-compressor.js";
  import {
    mapActions
  } from "vuex";

  import {
    mapState,
    mapMutations
  } from "vuex";

  export default {
    data() {
      return {
        image: ""
      };
    },
    components: {
      "vue-camera": VueCamera,
      "picture-input": PictureInput
    },
    computed: {
      ...mapState(["product.productId"])
    },
    methods: {
      ...mapActions(["createProductId"]),
      onChanged() {
        console.log("New picture loaded");
        if (this.$refs.pictureInput.file) {
          this.image = this.$refs.pictureInput.file;
        } else {
          console.log("Old browser. No support for Filereader API");
        }
      },
      onRemoved() {
        this.image = "";
        location.reload();
      },
      compressAndUpload() {
        if (this.image) {
          this.createProductId().then(response => {
            new ImageCompressor(this.image, {
              quality: 0.6,
              success(image) {
                const formData = new FormData();
                formData.append(image, {});

                Api().post('', formData).then(() => {
                  console.log("Upload success");
                  this.$router.push("/detailview");
                });
              },
              error(e) {
                console.log(e.message);
              }
            });
          });
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

  input[type="file"] {
    display: none;
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
