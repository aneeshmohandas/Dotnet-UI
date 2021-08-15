import Vue from "vue";
import Vuetify from "vuetify/lib/framework";

Vue.use(Vuetify);

export default new Vuetify({
  theme: {
     dark: false ,
    themes: {
      light: {
        primary: "#512bd4",
        // primary:"#e8f0fe",
        secondary:"#1e0965"
        
      },
    },
  },
});
