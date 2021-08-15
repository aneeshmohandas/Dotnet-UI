<template>
  <v-container>
    <v-row justify="space-around">
      <v-col cols="12" md="12">
        <v-sheet class="pa-12" rounded="xl" color="grey lighten-3">
          <v-card max-width="800" max-height="600" class="mx-auto">
            <v-toolbar color="primary" dark>
              <v-toolbar-title>Packages</v-toolbar-title>
              <v-spacer></v-spacer>
              <v-text-field
                v-model="search"
                v-if="showSearchWindow"
                label="Search here"
                :loading="isLoading"
                color="warning"
                dark
                flat
                solo-inverted
                hide-details
                clearable
                clear-icon="mdi-close-circle-outline"
              ></v-text-field>
              <v-btn
                v-if="showSearchWindow"
                :disabled="isLoading || !search || search.length < 3"
                @click="getPackageData()"
                icon
              >
                <v-icon>mdi-magnify</v-icon>
              </v-btn>
              <v-btn
                v-if="!showSearchWindow"
                @click="manageShowingWindow('search')"
                icon
              >
                <v-icon>mdi-magnify</v-icon>
              </v-btn>
              <v-btn
                v-if="showSearchWindow"
                @click="manageShowingWindow('currentPackage')"
                icon
              >
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </v-toolbar>

            <v-list three-line>
              <v-list-item-group v-model="selectedItem" color="primary">
                <template v-if="showingList.length > 0">
                  <template v-for="(item, index) in showingList">
                    <v-list-item :key="item.title">
                      <template>
                        <v-list-item-avatar v-if="item.iconUrl">
                          <v-img :src="item.iconUrl"></v-img>
                        </v-list-item-avatar>
                        <v-list-item-icon v-else>
                          <v-icon v-text="'mdi-clipboard-text'"></v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                          <v-list-item-title>{{
                            item.title
                          }}</v-list-item-title>
                          <v-list-item-subtitle
                            >{{ item.description }}
                          </v-list-item-subtitle>
                        </v-list-item-content>
                        <v-list-item-action>
                          <v-list-item-action-text>
                            <div>
                              <v-chip
                                class="ma-1"
                                color="deep-purple accent-4"
                                pill
                                outlined
                                small
                              >
                                <v-icon left>mdi-wrench </v-icon>
                                {{ item.version }}
                              </v-chip>
                              <v-chip
                                class="ma-1"
                                color="deep-purple "
                                pill
                                outlined
                                small
                              >
                                <v-icon left> mdi-download </v-icon>
                                {{ item.totalDownloads }}
                              </v-chip>
                            </div>
                          </v-list-item-action-text>
                          <v-btn
                            class="mx-2"
                            outlined
                            fab
                            x-small
                            color="primary"
                            @click="downloadPackage(item)"
                            v-if="showSearchWindow && showDownloadBtn(item)"
                          >
                            <v-icon dark> mdi-download </v-icon>
                          </v-btn>
                        </v-list-item-action>
                      </template>
                    </v-list-item>

                    <v-divider :key="index" :inset="true"></v-divider>
                  </template>
                </template>
                <template v-else>
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>
                        <span v-if="!showSearchWindow"
                          >No packages added to current project</span
                        >
                        <span v-else>No data found</span>
                      </v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </template>
              </v-list-item-group>
            </v-list>
            <v-divider></v-divider>

            <v-card-actions> </v-card-actions>
          </v-card>
        </v-sheet>
      </v-col>
    </v-row>
    <v-dialog v-model="vesrionDialog" persistent max-width="390">
        <template v-if="selectedPackage">
      <v-card>
        <v-card-title class="text-h5">
          {{
            selectedPackage.title
          }}
        </v-card-title>
        <v-card-text>
        
            <v-autocomplete
              v-model="selectedPackage.version"
              :items="currentPackageVersion"
              color="primary"
              label="Select Version"
              item-text="version"
              item-value="version"
            >
            </v-autocomplete>
         
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="versionModalCancelClick">
            Cancel
          </v-btn>
          <v-btn color="green darken-1" text @click="versionModalSaveClick">
            Add
          </v-btn>
        </v-card-actions>
      </v-card>
       </template>
    </v-dialog>
  </v-container>
</template>
<script>
export default {
  props: {
    addedPackages: [],
    intialTab: {
      type: String,
      default: "search",
    },
  },
  data: () => ({
    selectedItem: "",
    showingList: [],
    showSearchWindow: false,
    search: "",
    isLoading: false,
    tab: "",
    vesrionDialog: false,
    selectedPackage: null,
    currentPackageVersion: [],
    selectedVersion: {},
  }),

  created() {},
  mounted() {
    if (this.intialTab != "search") {
      this.showSearchWindow = false;
      this.showingList = [...this.addedPackages];
    }
    // this.$notify({
    //   title: "Important message",
    //   text: "Hello user!"
    //   //position:"bottom right"
    // });
    // this.getPackageData();
  },
  methods: {
    getPackageData() {
      this.isLoading = true;
      this.$httpClient
        .get(
          "https://azuresearch-usnc.nuget.org/query?q=" +
            this.search +
            "&prerelease=false&semVerLevel=2.0.0&skip=0&take=5"
        )
        .then((res) => {
          this.isLoading = false;
          this.showingList = res.data.data;
        })
        .catch((error) => {
          this.isLoading = false;
          console.log(error);
        });
    },
    downloadPackage(item) {
      for (var packages of this.addedPackages) {
        if (item.id == packages.id) {
          this.$notify({
            title: "Warning",
            text: "Package already added",
            type: "warning",
          });
          return;
        }
      }
      this.vesrionDialog = true;
      this.selectedPackage = Object.assign({}, item);
      this.currentPackageVersion = item.versions;
      // item.isNew = true;
      // item.addedOn = this.$dayjs();
      // this.$emit("packageAdded", item);
      // this.addedPackages.push(item);
    },
    manageShowingWindow(type) {
      switch (type) {
        case "search":
          this.showSearchWindow = true;
          if (this.search) {
            this.getPackageData();
          } else {
            this.showingList = [];
          }
          break;
        case "currentPackage":
          this.showSearchWindow = false;
          this.showingList = [...this.addedPackages];
          break;
      }
    },
    showDownloadBtn(item) {
      var packagFound = false;
      for (var packages of this.addedPackages) {
        if (item.id == packages.id) {
          packagFound = true;
        }
      }
      if (packagFound) return false;
      else return true;
    },
    versionModalCancelClick() {
      this.vesrionDialog = false;
      this.selectedPackage = null;
      this.currentPackageVersion = [];
    },
    versionModalSaveClick() {
      this.selectedPackage.isNew = true;
      this.selectedPackage.addedOn = this.$dayjs();
      this.$emit("packageAdded", this.selectedPackage);
       this.vesrionDialog = false;
    },
  },
};
</script>