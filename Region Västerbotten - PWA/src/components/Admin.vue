<template>
  <div>
    <v-container fill-height v-if="pass == 'true'">
      <v-layout flex justify-center>
        <v-flex md6 xs12 class="text-xs-center">
          <v-card class="mt-5" v-if="!isAppLoading">
            <v-tabs light slider-color="primary" centered>
              <v-tab ripple>
                <v-badge color="primary">
                  <span slot="badge">{{statusCount(1)}}</span>
                  <span>{{$t('Review')}}</span>
                </v-badge>
              </v-tab>
              <v-tab ripple>
                {{$t('Sent')}} ({{statusCount(2)}})
              </v-tab>

              <v-tab-item>
                <v-divider></v-divider>
                <v-card light color="grey lighten-4">
                  <v-list two-line>
                    <v-list-tile v-for="item in products" v-if="item.Status == 1" :key="item.id" avatar @click.stop="openDialog(item)">
                      <v-list-tile-avatar>
                        <img :src="urlToImage + item.ProductImages[0].Path">
                      </v-list-tile-avatar>
                      <v-list-tile-content>
                        <v-list-tile-title v-html="item.Name"></v-list-tile-title>
                      </v-list-tile-content>
                      <v-list-tile-action>
                        <v-list-tile-action-text v-if="item.Price != null || item.Price != undefined">{{ item.Price }} kr</v-list-tile-action-text>
                        <v-list-tile-action-text v-else class="text-error">{{$t('No price')}}</v-list-tile-action-text>
                      </v-list-tile-action>
                    </v-list-tile>
                  </v-list>
                </v-card>
              </v-tab-item>
              <v-tab-item>
                <v-divider></v-divider>
                <v-card light color="grey lighten-4">
                  <v-list two-line>
                    <template v-for="item in products" v-if="item.Status == 2">
                      <v-list-tile :key="item.Id" avatar>
                        <v-list-tile-avatar>
                          <img :src="urlToImage + item.ProductImages[0].Path">
                        </v-list-tile-avatar>
                        <v-list-tile-content>
                          <v-list-tile-title v-html="item.Name"></v-list-tile-title>
                        </v-list-tile-content>
                        <v-list-tile-action>
                          <v-list-tile-action-text v-if="item.Price != null || item.Price != undefined">{{ item.Price }} kr</v-list-tile-action-text>
                          <v-list-tile-action-text v-else>{{$t('No price')}}</v-list-tile-action-text>
                        </v-list-tile-action>
                      </v-list-tile>
                    </template>
                  </v-list>
                </v-card>
              </v-tab-item>
            </v-tabs>

          </v-card>
          <v-dialog v-model="dialog" max-width="500px">
            <v-card>
              <transition name="fade" mode="out-in" v-if="ProductImages.length > 1">
                <v-carousel :cycle="true" v-if="ProductImages" :lazy="true" :interval="10000">
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

              <v-card-title>
                <div class="text-xs-left app-space-xxl">
                  <h3 class="headline">{{product.Name}}</h3>
                  <span class="grey--text">{{$t(product.Category)}}
                  </span>
                </div>
              </v-card-title>
              <v-divider></v-divider>
              <v-card-text>
                <v-container>
                  <v-form v-model="valid" ref="form" lazy-validation>
                    <p class="custom-margin">{{$t('Category')}}</p>
                    <v-select v-model="product.Category" :items="categoriesList" item-text="Name" item-value="Name" class="mb-3" :label="$t(product.Category)"
                      single-line auto solo :prepend-icon="product.CategoryIcon || categoriesList.Icon" hide-details></v-select>

                    <p class="custom-margin">{{$t('Name')}}</p>
                    <v-text-field v-validate="'required'" :class="{'input': true, 'is-danger': errors.has('name') }" name="name" v-model="product.Name"
                      large class="mb-3" solo prepend-icon="label_outline" type="text" :label="$t('Name')" :v-bind="product.Name"
                      :value="product.Name"></v-text-field>
                    <p v-show="errors.has('name')" class="text-error">{{ errors.first('name') }}</p>

                    <div v-for="item in product.ProductProperties" :key="item.id">
                      <p class="custom-margin">{{$t(item.Key)}}</p>
                      <v-text-field v-validate="'required'" :name="item.Key" v-model="item.Value" class="mb-3" solo :prepend-icon="item.Icon" :type="item.Type"
                        :v-bind="item.Value" :label="$t(item.Key)" :value="item.Value" required></v-text-field>
                      <p v-show="errors.has(item.Key)" class="text-error">{{ errors.first(item.Key) }}</p>
                    </div>

                    <p class="custom-margin">{{$t('Price')}}</p>
                    <v-text-field v-validate="'required|numeric'" name="price" v-model="product.Price" suffix="kr" :label="$t('Price')" class="mb-4"
                      large solo prepend-icon="toll" type="number"></v-text-field>
                    <p v-show="errors.has('price')" class="text-error">{{ errors.first('price') }}</p>

                    <p class="custom-margin">{{$t('Pick up at Returbutiken')}}</p>
                    <v-switch v-model="product.Shipping" color="success"></v-switch>

                    <p class="custom-margin">{{$t('Cost of shipping')}}</p>
                <v-text-field v-model="product.ShippingCost" suffix="SEK" :label="$t('Price')" class="mb-4" large solo prepend-icon="local_shipping" type="number"></v-text-field>

                    <p class="custom-margin">{{$t('Comment')}}</p>
                    <v-text-field v-model="product.Comment" class="mb-4" large multi-line solo prepend-icon="" type="text"></v-text-field>
                    <v-divider class="mb-3"></v-divider>

                    <p class="custom-margin">{{$t('Condition')}}</p>
                    <star-rating v-model="product.Condition"></star-rating>
                  </v-form>
                </v-container>
              </v-card-text>
              <v-card-actions>
                <v-btn color="error" flat @click.stop="dialog=false">{{$t('Close')}}</v-btn>
                <v-spacer></v-spacer>
                <v-btn color="primary" flat @click.stop="validateBeforeSubmit(1)" :disabled="errors.any()">{{$t('Save')}}</v-btn>
                <v-btn color="amber accent-4" flat @click.stop="validateBeforeSubmit(2)" :disabled="errors.any()">{{$t('Submit to Tradera')}}</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-flex>
      </v-layout>
    </v-container>
    <v-container v-else>
      <v-layout>
        <v-flex>
          Du saknar behörighet för att besöka den här sidan.
        </v-flex>
      </v-layout>
    </v-container>
  </div>

</template>

<script>
  import axios from "axios";
  import {
    mapGetters,
    mapActions
  } from "vuex";
  import {
    StarRating
  } from "vue-rate-it";

  export default {
    data() {
      return {
        pass: "",
        dialog: false,
        valid: false,
        categoriesList: [],
        urlToImage: "https://xapi.hi5.se/",
        products: [],
        tabs: null,
        gradient: "to top, rgba(0,0,0,0.0), rgba(0,0,0,0.3)",
        product: {
          Id: "",
          Name: "",
          Category: "",
          CategoryIcon: "",
          Price: 0,
          Shipping: false,
          ShippingCost: 0,
          Condition: 0,
          Comment: "",
          Status: 0,
          ProductProperties: []
        },
        ProductImages: []
      };
    },
    components: {
      StarRating
    },
    methods: {
      ...mapActions([
        "getProducts",
        "getCategories",
        "isLoading",
        "updateProductById",
        "makeImagePrimary",
        "removeImage"
      ]),
      canByPass() {
        this.pass = localStorage.getItem("isAdmin");
      },
      async validateBeforeSubmit(statusNumber) {
        this.$validator.validateAll().then(result => {
          if (result) {
            this.isLoading(true);
            if (statusNumber == 1) {
              this.product.Status = 1;
            }
            if (statusNumber == 2) {
              this.product.Status = 2;
            }
            let response = this.updateProductById(this.product);
            this.loadProducts();
            this.isLoading(false);
            this.dialog = false;
            return;
          }
          alert("Är alla fält korrekt?");
        });
      },
      async removeThisImage(index, itemId) {
        this.isLoading(true);
        let res = await this.removeImage(itemId);
        if (res) {
          this.ProductImages.splice(index, 1);
        }
        this.loadProducts();
        this.isLoading(false);
      },
      async makeTheImagePrimary(theImage) {
        this.isLoading(true);
        let res = await this.makeImagePrimary(theImage);
        this.loadProductAfterImageUpload();
        this.isLoading(false);
        return;
      },
      async loadCategories() {
        let response = await this.getCategories();
        this.categoriesList = this.theCategories;
      },
      async loadProducts() {
        this.isLoading(true);
        let response = await this.getProducts();
        console.log(response)
        this.products = this.productData.data;
        this.isLoading(false);
      },
      openDialog(item) {
        this.product.Id = item.Id;
        this.product.Name = item.Name;
        this.product.Category = item.Category;
        this.product.CategoryIcon = item.CategoryIcon;
        this.product.Price = item.Price;
        this.product.Shipping = item.Shipping;
        this.product.ShippingCost = item.ShippingCost;
        this.product.Comment = item.Comment;
        this.product.Condition = item.Condition;
        this.product.ProductProperties = item.ProductProperties;
        this.ProductImages = item.ProductImages;
        console.log(this.product)
        this.dialog = true;
      },
      statusCount(statusNumber) {
        var result = this.products.filter(function (p) {
          return p.Status == statusNumber;
        });
        return result.length;
      }
    },
    computed: {
      ...mapGetters(["productData", "theCategories", "isAppLoading", "isAdmin"])
    },
    mounted() {
      this.canByPass();
      this.loadProducts();
      this.loadCategories();
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
