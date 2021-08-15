<template>
    <solutionList />
    <!-- -->
</template>

 <script>
// import treeview from "../components/common/treeView.vue";
import solutionList from "../components/common/solutionList.vue";

export default {
  name: "Home",
  components: {  solutionList },
  data: () => ({
    valid: false,
    dialog: false,
    workingDirectory: "",
    logModal: false,
    termKey: 999,
  }),
  mounted() {},
  methods: {
    onWorkDirectorySaveClick() {
      var id = this.$refs.directoryTree.active[0];
      var directoryList = this.$refs.directoryTree.directoryList;
      var searchedItem = this.getDirectory(directoryList, id);
      this.workingDirectory = searchedItem.path;
      this.dialog = false;
    },
    getDirectory(directoryList, id) {
      if (directoryList) {
        for (var i = 0; i < directoryList.length; i++) {
          if (directoryList[i].id == id) {
            return directoryList[i];
          }
          var found = this.getDirectory(directoryList[i].children, id);
          if (found) return found;
        }
      }
    },
   
  },
};
</script>

