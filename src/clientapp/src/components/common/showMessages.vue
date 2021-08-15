  <template>
  <div>
    <v-dialog
      v-model="logModal"
      persistent
      transition="dialog-bottom-transition"
      hide-overlay
      :fullscreen="showFullScreen"
    >
      <terminal />
    </v-dialog>
  </div>
</template>
<script>
import terminal from "@/components/common/terminal.vue";

export default {
  components: { terminal },
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
  data: () => ({
    messages: [],
    showFullScreen:true
  }),
  methods: {
    onLogModalClose() {
      this.$emit("logModalClose");
    },
  },
};
</script>
