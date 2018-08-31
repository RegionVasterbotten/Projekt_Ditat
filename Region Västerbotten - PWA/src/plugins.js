import Vue from 'vue'
import VueI18n from 'vue-i18n'
Vue.use(VueI18n)

const messages = {
  en: {
    'Capture Image' : 'Capture Image',
    'Login': 'Login',
    'Settings': 'Settings',
    'Admin': 'Admin',
    'New Product': 'New Product',
    'Author': 'Author',
    'Category': 'Category',
    'Year': 'Year',
    'Name': 'Name',
    'Price': 'Price',
    'Shipping': 'Shipping',
    'Cost of shipping': 'Cost of shipping',
    'Comment': 'Comment',
    'Condition': 'Condition',
    'Could not calculate': 'Could not calculate',
    'Book': 'Book',
    'Pot': 'Pot',
    'CD': 'CD',
    'DVD': 'DVD',
    'Primary images cannot be deleted': 'Primary images cannot be deleted',
    'Primary image saved': 'Primary image saved',
    'Changes saved': 'Changes saved',
    'Close': 'Close',
    'Review': 'Review',
    'Sent': 'Sent',
    'Save': 'Save',
    'Submit to Tradera': 'Submit to Tradera',
    'No price': 'No price',
    'Width': 'Width',
    'Height': 'Height',
    'Bok': 'Book',
    'Avhämtning från Returbutiken' : 'Pick-up at Returbutiken'
  },
  sv: {
    'Capture Image' : 'Ta bild',
    'Login': 'Logga in',
    'Settings': 'Inställningar',
    'Admin': 'Adminvy',
    'New Product': 'Ny produkt',
    'Author': 'Författare',
    'Category': 'Kategori',
    'Year': 'Utgivningsår',
    'Name': 'Namn',
    'Price': 'Pris',
    'Shipping': 'Kan fraktas',
    'Cost of shipping': 'Fraktkostnad',
    'Comment': 'Kommentar',
    'Condition': 'Skick',
    'Could not calculate': 'Kunde inte räkna ut',
    'Book': 'Bok',
    'Pot': 'Kruka',
    'CD': 'CD',
    'DVD': 'DVD',
    'Primary images cannot be deleted': 'Primära bilder går inte att ta bort',
    'Primary image saved': 'Primär bild sparad',
    'Changes saved': 'Ändringar sparade',
    'Close': 'Stäng',
    'Review': 'Granska',
    'Sent': 'Skickade',
    'Save': 'Spara',
    'Submit to Tradera': 'Skicka till Tradera',
    'No price': 'Inget pris',
    'Width': 'Bredd',
    'Height': 'Höjd',
    'Pick up at Returbutiken' : 'Avhämtning från Returbutiken'
  }
}
export const i18n = new VueI18n({
  silentTranslationWarn: true,
  locale: 'sv', // set locale
  messages // set locale messages
})
