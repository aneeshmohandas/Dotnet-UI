<template>
  <v-dialog
    v-model="logModal"
    persistent
    transition="dialog-bottom-transition"
    hide-overlay
    fullscreen
  >
    <div class="terminal space shadow">
      <div class="top">
        <div class="btns">
          <v-icon @click="onLogModalClose" v-if="showCloseBtn">
            mdi-close
          </v-icon>
        </div>
        <div class="title">Output</div>
      </div>
      <pre class="terminalbody" id="typewriter"></pre>
    </div>
  </v-dialog>
</template>

<script>
export default {
  props: {
    logModal: {
      type: Boolean,
      default: false,
    },
    showCloseBtn: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      term: "",
      socket: "",
      messages: [],
    };
  },
  watch: {
    logModal(val) {
      if (val) {
        var t = document.getElementById("typewriter");
        if (t) t.innerHTML = "";
      }
    },
  },
  created() {
    this.$appHub.$on("message-received", this.getMessage);
  },
  mounted() {},
  methods: {
    getMessage: function (msg) {
      this.setupTypewriter(msg);
    },
    onLogModalClose() {
      this.$emit("logModalClose");
    },
    setupTypewriter(content) {
      var HTML = content;
      var t = document.getElementById("typewriter");
      t.innerHTML += HTML;
    },
  },
  destroyed() {
    this.stopSignalR();
    this.$appHub.$off("message-received", this.getMessage);
  },
};
</script>

<style  scoped>
* {
  margin: 0;
  padding: 0;
}
.terminal {
  border-radius: 5px 5px 0 0;
  position: relative;
  width: 95%;
}
.terminal .top {
  background: #e8e6e8;
  color: black;
  padding: 5px;
  border-radius: 5px 5px 0 0;
}
.terminal .btns {
  position: absolute;
  top: 7px;
  left: 5px;
}
.terminal .circle {
  width: 12px;
  height: 12px;
  display: inline-block;
  border-radius: 15px;
  margin-left: 2px;
  border-width: 1px;
  border-style: solid;
}
.title {
  text-align: center;
}

.clear {
  clear: both;
}
.terminal .terminalbody {
  background: black;
  color: #7afb4c;
  padding: 8px;
  overflow: auto;
  height: 80vh;
}
.space {
  margin: 25px;
}
.shadow {
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.4);
}
</style>
