<template>
  <div>
    <v-container grid-list-xs fluid fill-height>
      <v-layout align-center justify-center row wrap>
        <v-flex xs12>
          <transition name="fade" mode="out-in" v-if="ProductImages.length > 1">
            <v-carousel :cycle="true" v-if="ProductImages && !isAppLoading" :lazy="true" :interval="10000">
              <v-carousel-item v-for="(item, index) in ProductImages" :key="index" :gradient="gradient">
                <div class="img-wrapper ">
                  <img v-img:ProductImages :src="urlToImage + item.Path" class="img-in-container" alt="">
                  <div class="img-overlay app-space-xxl">
                    <v-btn v-if="item.PrimaryImage == true" fab color="primary">
                      <v-icon dark>star</v-icon>
                    </v-btn>
                    <v-btn v-else fab color="grey" @click="makeTheImagePrimary(item)">
                      <v-icon dark>star_border</v-icon>
                    </v-btn>
                    <v-tooltip bottom v-if="item.PrimaryImage == true">
                      <v-btn slot="activator" fab :disabled="item.PrimaryImage" color="grey">
                        <v-icon dark>delete</v-icon>
                      </v-btn>
                      <span>{{$t('Primary images cannot be deleted')}}</span>
                    </v-tooltip>
                    <v-btn v-else fab color="red" @click="removeThisImage(index, item.Id)">
                      <v-icon dark>delete</v-icon>
                    </v-btn>
                  </div>
                </div>
              </v-carousel-item>
            </v-carousel>
          </transition>
          <transition name="fade" mode="out-in" v-else>
            <v-card>
              <v-card-media v-for="item in ProductImages" :key="item.id" :src="urlToImage + item.Path" height="350px">

                <v-layout column class="media">
                  <div class="img-wrapper ">
                    <img v-img:ProductImages :src="urlToImage + item.Path" class="img-in-container" alt="">
                    <div class="img-overlay app-space-xxl">
                      <v-btn v-if="item.PrimaryImage == true" fab color="primary">
                        <v-icon dark>star</v-icon>
                      </v-btn>
                      <v-btn v-else fab color="grey" @click="makeTheImagePrimary(item)">
                        <v-icon dark>star_border</v-icon>
                      </v-btn>
                      <v-tooltip bottom v-if="item.PrimaryImage == true">
                        <v-btn slot="activator" fab :disabled="item.PrimaryImage" color="grey">
                          <v-icon dark>delete</v-icon>
                        </v-btn>
                        <span>{{$t('Primary images cannot be deleted')}}</span>
                      </v-tooltip>
                      <v-btn v-else fab color="red" @click="removeThisImage(index, item.Id)">
                        <v-icon dark>delete</v-icon>
                      </v-btn>
                    </div>
                  </div>
                </v-layout>

              </v-card-media>
            </v-card>
          </transition>
        </v-flex>
        <v-flex xs12>
          <v-card>
            <div class="text-xs-left app-space-xxl">
              <h3 class="headline" v-if="product.Name">{{product.Name}}</h3>
              <h3 class="headline" v-else>{{$t(product.Category)}}</h3>

              <span class="grey--text" v-for="item in product.productProperties" :key="item.id">{{$t(item.Key)}}:
                <span v-if="item.Value">{{item.Value}}</span>
                <span v-else>{{$t('Could not calculate')}}</span>
                <br>
              </span>
            </div>
            <v-divider></v-divider>
          </v-card>
          <v-card class="grey--text">
            <v-container>
              <v-form v-model="valid" ref="form" lazy-validation>
                <p class="custom-margin">{{$t('Category')}}</p>
                <v-select v-model="product.Category" :items="categoriesList" item-text="Name" item-value="Name" class="mb-3" :label="$t(product.Category)" single-line auto solo :prepend-icon="product.CategoryIcon || categoriesList.Icon" hide-details></v-select>
                <p class="custom-margin">{{$t('Name')}}</p>
                <v-text-field v-model="product.Name" large class="mb-3" solo prepend-icon="label_outline" type="text" :label="$t('Name')" :v-bind="product.Name" :value="product.Name" required></v-text-field>
                <div v-for="item in product.productProperties" :key="item.id">
                  <p class="custom-margin">{{$t(item.Key)}}</p>
                  <v-text-field v-model="item.Value" class="mb-3" solo :prepend-icon="item.Icon" :type="item.Type" :v-bind="item.Value" :label="$t(item.Key)" :value="item.Value" required></v-text-field>
                </div>

                <p class="custom-margin">{{$t('Price')}}</p>
                <v-text-field v-model="product.Price" suffix="SEK" :label="$t('Price')" class="mb-4" large solo prepend-icon="toll" type="number"></v-text-field>
                <p class="custom-margin">{{$t('Pick up at Returbutiken')}}</p>
                <v-switch v-model="product.Shipping" color="success"></v-switch>
                <p class="custom-margin">{{$t('Cost of shipping')}}</p>
                <v-text-field v-model="product.Shippingcost" suffix="SEK" :label="$t('Price')" class="mb-4" large solo prepend-icon="local_shipping" type="number"></v-text-field>
                <p class="custom-margin">{{$t('Comment')}}</p>
                <v-text-field v-model="product.Comment" class="mb-4" large multi-line solo prepend-icon="" type="text"></v-text-field>
                <v-divider class="mb-3"></v-divider>
                <p class="custom-margin">{{$t('Condition')}}</p>
                <star-rating v-model="product.Condition"></star-rating>
              </v-form>
            </v-container>
            <transition name="fade" mode="out-in">
              <v-btn color="green darken-4" class="animated fadeInRightBig" fab fixed bottom right :disabled="isAppLoading">
                <v-icon dark :class="[{'white--text': isAppLoading }]">add_a_photo</v-icon>
                <input type="file" name="myImage" accept="image/jpeg" class="input-button" @change="onFileChanged" />
              </v-btn>
            </transition>
            <v-divider></v-divider>
            <div class="text-xs-center app-space-xxl ">
              <v-layout row wrap>
                <v-flex xs12>
                  <v-btn @click="submit" fab color="primary" class="mb-3">
                    <v-icon>check</v-icon>
                  </v-btn>
                </v-flex>
              </v-layout>
            </div>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <v-snackbar :timeout="timeout" :color="color" :multi-line="mode === 'multi-line'" :vertical="mode === 'vertical'" v-model="snackbar">
      {{$t('Primary image saved')}}
      <v-btn dark flat @click.native="snackbar = false">{{$t('Close')}}</v-btn>
    </v-snackbar>
    <v-snackbar :timeout="timeout" :color="color" :multi-line="mode === 'multi-line'" :vertical="mode === 'vertical'" v-model="snackbarSaved">
      {{$t('Changes saved')}}
      <v-btn dark flat @click.native="snackbarSaved = false">{{$t('Close')}}</v-btn>
    </v-snackbar>
  </div>
</template>

<script>
import {
  mapGetters,
  mapActions
} from "vuex";
import Api from '../api.js'
import ImageCompressor from "image-compressor.js";
import {
  StarRating
} from "vue-rate-it";

export default {
  data() {
    return {
      valid: false,
      row: null,
      hasProductyProperty: false,
      gradient: "to top, rgba(0,0,0,0.0), rgba(0,0,0,0.3)",
      selectedFile: null,
      selectedFileUrl: null,
      ProductImages: [],
      snackbar: false,
      snackbarSaved: false,
      color: "success",
      index: null,
      mode: "",
      timeout: 3000,
      categoriesList: [],
      swiperOption: {
        pagination: {
          el: ".swiper-pagination",
          dynamicBullets: true
        }
      },
      product: {
        Id: "",
        Name: "",
        Category: "",
        CategoryIcon: "",
        Price: 0,
        Shipping: false,
        Shippingcost: 0,
        Condition: 0,
        Comment: "",
        Status: 1,
        productProperties: []
      },
      urlToImage: "https://xapi.hi5.se/"
    };
  },
  components: {
    StarRating
  },
  methods: {
    ...mapActions([
      "isLoading",
      "updateProductById",
      "addImageToProductById",
      "makeImagePrimary",
      "getCategories",
      "removeImage"
    ]),
    goTo(input) {
      this.$router.go(input);
    },
    async removeThisImage(index, itemId) {
      this.imLoading(true);
      let res = await this.removeImage(itemId);
      if (res) {
        this.ProductImages.splice(index, 1);
      }
      this.loadProduct();
      this.imLoading(false);
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
          let response = await this.addImageToProductById(formData);
          this.loadProduct();
          this.isAppMounted = false;
          this.imLoading(false);
        } catch (error) {
          this.imLoading(false);
          this.dialog3 = true;
        }
      }
    },
    onUpload() {
      console.log(this.selectedFile);
    },
    async submit() {
      if (this.$refs.form.validate()) {
        this.isLoading(true);
        console.log(this.product)
        let response = await this.updateProductById(this.product);
        console.log("Saved")
        console.log(this.product)
        this.snackbarSaved = true;
        this.isLoading(false);
        this.$router.replace('/')
      }
    },
    async makeTheImagePrimary(theImage) {
      let res = await this.makeImagePrimary(theImage);
      this.loadProductAfterImageUpload();
      this.imLoading(false);
      this.snackbar = true;
      return;
    },
    clear(item) {
      console.log(item);
    },
    imLoading(value) {
      this.isLoading(value);
    },
    async loadProductAfterImageUpload() {
      this.imLoading(true);
      const latestProductId = localStorage.getItem("latestProductId");
      try {
        let result = await Api().get(
          `/products/${latestProductId}`,
        );
        this.loadProduct()
      } catch (error) {
        console.log(error);
      }
    },
    async loadCategories() {
      let response = await this.getCategories();
      this.categoriesList = this.theCategories;
    },
    async loadProduct() {
      this.imLoading(true);
      const latestProductId = localStorage.getItem("latestProductId");
      try {
        let result = await Api().get(
          `/products/${latestProductId}`
        );
        this.ProductImages = result.data.ProductImages;
        this.product.Category = result.data.Category;
        this.product.CategoryIcon = result.data.CategoryIcon;
        this.product.Comment = result.data.Comment;
        this.product.Price = result.data.Price;
        this.product.Shipping = result.data.Shipping;
        this.product.Shippingcost = result.data.Shippingcost;
        this.product.Condition = result.data.Condition;
        this.product.Id = result.data.Id;
        this.product.Name = result.data.Name;
        if (this.product.productProperties.length > 0) {
          this.product.productProperties = [];
        }
        for (var key in result.data.ProductProperties) {
          var output = {};
          output = result.data.ProductProperties[key];
          this.product.productProperties.push(output);
        }
        console.log(this.currentLang)
        console.log(this.product)
        this.imLoading(false);
      } catch (error) {
        this.imLoading(false);
        console.log(error);
      }
    }
  },
  mounted() {
    this.loadProduct();
    this.loadCategories();

  },

  computed: {
    ...mapGetters(["theProductId", "isAppLoading", "theCategories", "currentLang"]),

  }
};

</script>

<style scoped>
.carousel-height {
  height: 250px;
}

.app-container {
  justify-content: center;
  align-items: center;
}

.slider-app-active {
  display: block !important;
}

.img-in-container {
  width: 100%;
}

.slider-app {
  display: block;
}

.preview-img-list {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
}

.preview-img-item {
  margin: 5px;
  max-width: 100px;
  max-height: 100px;
}

.app-space {
  margin-bottom: 8px;
}

#create .speed-dial {
  position: absolute;
}

#create .btn--floating {
  position: relative;
}

.app-image {
  width: 100%;
  height: 350px;
  background-position: center;
  background-size: cover;
}

.img-wrapper {
  position: relative;
  width: 100%;
}

.img-overlay {
  position: absolute;
  top: 0;
  right: 0;
  text-align: center;
}

.img-overlay:before {
  content: " ";
  display: block;
  /* adjust 'height' to position overlay content vertically */
  height: 33%;
}

.app-space-xl {
  padding: 10px;
}

.input-button {
  height: 100%;
  z-index: 99999;
  width: 100%;
  opacity: 0 !important;
}

.app-space-xxl {
  padding: 25px;
}

.app-space-xs {
  margin-bottom: 5px;
}

.text-container {
  margin: 0 35px 0 35px;
}

.custom-margin {
  margin-bottom: 8px;
}

.app-item {
  align-self: center;

  margin-top: 46%;
}

.carousel__controls {
  background: rgba(0, 0, 0, 0.3);
  -webkit-box-align: center;
  -ms-flex-align: center;
  align-items: center;
  bottom: 0;
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-pack: center;
  -ms-flex-pack: center;
  justify-content: center;
  left: 0;
  position: absolute;
  height: 50px;
  list-style-type: none;
  width: 100%;
  z-index: 1;
}

.opacity-6 {
  opacity: 0.6;
}
</style>
