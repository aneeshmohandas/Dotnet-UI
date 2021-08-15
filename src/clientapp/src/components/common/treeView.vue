<template>
  <v-dialog
    v-model="showModal"
    transition="dialog-bottom-transition"
    persistent
    no-click-animation
    width="500"
    max-width="500"
    scrollable
  >
    <template>
      <v-card tile>
        <v-toolbar dark color="primary">
          <v-btn icon dark @click="onModalCloseClick">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>
            {{ tittle }}
          </v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn dark text @click="onDirectorySelectClick"> Save </v-btn>
          </v-toolbar-items>
        </v-toolbar>

        <v-card-text>
          <div style="height: 400px">
            <v-treeview
              :key="key"
              :items="directoryList"
              :active.sync="active"
              rounded
              hoverable
              activatable
              :open.sync="initiallyOpen"
              :load-children="loadChildren"
            >
              <template v-slot:prepend="{ item, open }">
                <v-icon v-if="item.hasChildren">
                  {{ open ? "mdi-folder-open" : "mdi-folder" }}
                </v-icon>
              </template>
            </v-treeview>
          </div>
        </v-card-text>
        <v-card-actions class="justify-end"> </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>
<script>
export default {
  props: {
    tittle: {
      type: String,
      default: "Select Directory",
    },
    showModal: {
      type: Boolean,
      default: false,
    },
  },
  data: () => ({
    directoryList: [],
    initiallyOpen: [],
    search: null,
    caseSensitive: false,
    active: [],
    key: 1090,
  }),
  computed: {},
  created() {},
  mounted() {
    this.getTreeData();
  },
  methods: {
    getTreeData() {
      this.$httpClient
        .get(this.$siteName + "/api/dotnetui/home/getdata")
        .then((res) => {
          // handle success
          this.directoryList = res.data;
        })
        .catch((error) => {
          // handle error
          console.log(error);
        });
    },
    async loadChildren(item) {
      this.$httpClient
        .get(
          this.$siteName +
            "/api/dotnetui/home/directoryList?path=" +
            item.path +
            "&id=" +
            item.id
        )
        .then((res) => {
          // handle success
          var data = res.data;
          item.children.push(...data);
          this.key = this.key + 1;
          this.initiallyOpen.push(item.id);
          //   console.log(this.active);
        })
        .catch((error) => {
          // handle error
          console.log(error);
        });
    },
    onModalCloseClick() {
      this.$emit("modalCloseClicked");
    },
    onDirectorySelectClick() {
      if (this.active.length > 0) {
        var id = this.active[0];
        var searchedItem = this.getDirectory(this.directoryList, id);
        this.$emit("onSetDirectory", searchedItem.path);
      } else {
        this.$notify({
          title: "Warning",
          text: "Select a directory",
          type: "warning",
        });
        return;
      }
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