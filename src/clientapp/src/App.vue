<template>
  <v-app id="inspire">
    <v-app-bar app>
      <v-app-bar-title>Dotnet UI</v-app-bar-title>

      <v-tabs right>
        <v-tab @click="$router.push('/')">Solutions</v-tab>
        <v-tab @click="$router.push('/Activity')">Activities</v-tab>
      </v-tabs>
      <v-spacer></v-spacer>
      <!-- <v-btn icon>
        <v-icon>mdi-dots-vertical</v-icon>
      </v-btn> -->
      <!-- <template v-slot:extension>
       
      </template> -->
    </v-app-bar>

    <v-main>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-main>
    <terminal
      :logModal="logModal"
      @logModalClose="logModal = !logModal"
      :showCloseBtn="showLogModalCloseBtn"
    />
    <notifications
      :duration="5000"
      :width="500"
      animation-name="v-fade-left"
      position="bottom right"
    >
    </notifications>
  </v-app>
</template>

<script>
import terminal from "@/components/common/terminal.vue";

export default {
  components: {
    terminal,
  },
  //
  created() {
    this.startSignalR();
    window.addEventListener("beforeunload", function (e) {
      var confirmationMessage = "o/";

      (e || window.event).returnValue = confirmationMessage; //Gecko + IE
      return confirmationMessage; //Webkit, Safari, Chrome
    });
    this.$appHub.$on("block-ui", this.blockUi);
    this.$appHub.$on("enable-ui", this.enableUi);
  },
  data: () => ({
    logModal: false,
    showLogModalCloseBtn: false,
  }),
  mounted() {},
  methods: {
    blockUi() {
      this.logModal = true;
      this.showLogModalCloseBtn = false;
    },
    enableUi() {
      this.showLogModalCloseBtn = true;
    },
  },
  destroyed() {
    this.$appHub.$off("block-ui", this.blockUi);
    this.stopSignalR();
  },
};
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
.custom-template {
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  text-align: left;
  font-size: 13px;
  margin: 5px;
  margin-bottom: 0;
  align-items: center;
  justify-content: center;
  &,
  & > div {
    box-sizing: border-box;
  }
  background: #e8f9f0;
  border: 2px solid #d0f2e1;
  .custom-template-icon {
    flex: 0 1 auto;
    color: #15c371;
    font-size: 32px;
    padding: 0 10px;
  }
  .custom-template-close {
    flex: 0 1 auto;
    padding: 0 20px;
    font-size: 16px;
    opacity: 0.2;
    cursor: pointer;
    &:hover {
      opacity: 0.8;
    }
  }
  .custom-template-content {
    padding: 10px;
    flex: 1 0 auto;
    .custom-template-title {
      letter-spacing: 1px;
      text-transform: uppercase;
      font-size: 10px;
      font-weight: 600;
    }
  }
}
.v-fade-left-enter-active,
.v-fade-left-leave-active,
.v-fade-left-move {
  transition: all 0.5s;
}
.v-fade-left-enter,
.v-fade-left-leave-to {
  opacity: 0;
  transform: translateX(-500px) scale(0.2);
}
</style>
