<template>
  <div>
    <v-row justify="space-around">
      <v-col cols="12" md="12">
        <v-card max-width="100%" class="mx-auto" elevation="0">
          <v-card-text>
            <v-autocomplete
              outlined
              rounded
              dense
              persistent-placeholder
              placeholder="Search solution"
              v-model="selectedDrpItem"
              :loading="cmpLoading"
              :items="slnDrpList"
              hide-no-data
              :search-input.sync="searchTxt"
              clearable
            >
              <template v-slot:append>
                <v-btn
                  text
                  @click="createNewSolution()"
                  rounded
                  color="primary"
                  small
                >
                  <v-icon left> mdi-plus </v-icon>
                  Create New Solution
                </v-btn>
              </template>
              <template v-slot:prepend-inner>
                <div style="padding-top: 3px">
                  <v-icon small color="primary"> mdi-magnify </v-icon>
                </div>
              </template>
              <template v-slot:append-outer>
                <v-btn text @click="getProjectList()" rounded color="primary">
                  <v-icon left primary> mdi-refresh </v-icon>
                </v-btn>
              </template>
            </v-autocomplete>

            <div style="margin-top: 0px">
              <v-row>
                <template v-if="projects.length > 0">
                  <template v-for="(item, index) in projects">
                    <v-col
                      :key="item.solutionId"
                      cols="12"
                      sm="6"
                      md="4"
                      lg="4"
                    >
                      <div  :class="getCardClass(index)">
                        <h2 style="color: #512bd4">
                          {{
                            item.solutionName
                              ? item.solutionName
                              : item.projects[0].name
                          }}
                        </h2>

                        <v-chip
                          small
                          class="ma-2"
                          color="success"
                          outlined
                          pill
                          @click="showProjectList(item)"
                        >
                          <v-icon left> mdi-eye </v-icon>
                          {{ item.projects.length }} Projects
                        </v-chip>
                        <v-chip
                          small
                          class="ma-2"
                          color="primary"
                          outlined
                          pill
                          title="Created on"
                          @click="showProjectList(item)"
                        >
                          <v-icon left> mdi-calendar </v-icon>
                          {{ $dayjs(item.createdTime).fromNow() }}
                        </v-chip>
                        <v-chip
                          v-if="item.lastModifiedTime"
                          small
                          class="ma-2"
                          color="info"
                          outlined
                          pill
                          title="Last updated on"
                          @click="showProjectList(item)"
                        >
                          <v-icon left> mdi-pencil </v-icon>
                          {{ $dayjs(item.lastModifiedTime).fromNow() }}
                        </v-chip>

                        <v-divider light></v-divider>
                        <div class="col-12" style="padding: 2px">
                          <p v-if="item.description">
                            {{ item.description }}
                          </p>
                          <p v-else>No description</p>
                        </div>
                        <v-divider light></v-divider>
                        <div>
                          <v-chip
                            small
                            class="ma-2"
                            color="success"
                            outlined
                            pill
                            @click="buildSolutionClicked(item)"
                          >
                            <v-icon left> mdi-wrench </v-icon>
                            Build
                          </v-chip>

                          <v-chip
                            small
                            class="ma-2"
                            color="primary"
                            outlined
                            pill
                            @click="restoreSolutionClicked(item)"
                          >
                            <v-icon left> mdi-update </v-icon>
                            Restore
                          </v-chip>

                          <v-chip
                            small
                            class="ma-2"
                            color="warning"
                            outlined
                            pill
                            @click="cleanSolutionClicked(item)"
                          >
                            <v-icon left> mdi-delete </v-icon>
                            Clean
                          </v-chip>

                          <v-chip
                            small
                            class="ma-2"
                            color="indigo darken-3"
                            outlined
                            pill
                            @click="publishSolutionClicked(item)"
                          >
                            <v-icon left> mdi-launch </v-icon>
                            Publish
                          </v-chip>
                        </div>
                      </div>
                    </v-col>
                  </template>
                </template>
                <template v-else>
                  <v-row justify="center">
                    <div style="padding: 10px">
                      <v-img
                        max-height="400"
                        max-width="400"
                        src="@/assets/no_data.svg"
                      ></v-img>
                      <v-row justify="center" style="padding-top: 40px">
                        &nbsp;<a @click="createNewSolution()">
                          Create new solution</a
                        >
                      </v-row>
                    </div>
                  </v-row>
                </template>
              </v-row>
            </div>
          </v-card-text>
          <v-card-actions> </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <createNewSolution
      :showModal="newSolutionDialog"
      @onCloseClicked="onNewSolutionModalClose"
      @onSolutionSave="onFinalSave"
      ref="projectCreation"
      :key="solutionCmpKey"
    />
    <v-dialog
      v-model="showProjectDetails"
      persistent
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      no-click-animation
    >
      <v-card tile>
        <v-toolbar flat dark color="primary">
          <v-btn icon dark @click="onProjectDetailsModalClose">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title
            ><span v-if="selectedSolution">{{
              selectedSolution.solutionName
            }}</span></v-toolbar-title
          >
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
          <v-container>
            <solutionProjectList
              :solution="selectedSolution"
              @updateSelectedSolution="updateSelectedSolution"
              :projectList="selectedProjects"
            />
          </v-container>
        </v-card-text>

        <div style="flex: 1 1 auto"></div>
      </v-card>
    </v-dialog>
    <publishProject
      v-if="solutionSelectedForPublish"
      @onCloseBtnClick="closePublishModal"
      :showModal="showPublishModal"
      :solution="solutionSelectedForPublish"
      :key="publishCmpKey"
      @onPublishClick="onFinalPublishSolutionClick"
    />
  </div>
</template>
<script>
import createNewSolution from "@/components/common/createNewSolution.vue";
import solutionProjectList from "@/components/common/solutionProjectList.vue";
import publishProject from "@/components/common/publishProject.vue";

export default {
  components: { createNewSolution, solutionProjectList, publishProject },
  data: () => ({
    projects: [],
    itemsPerPage: 3,
    page: 1,
    itemsPerPageArray: [3, 6],
    logModal: false,
    newSolutionDialog: false,
    showProjectDetails: false,
    selectedSolution: null,
    solutionCmpKey: 888,
    cmpLoading: false,
    slnDrpList: [],
    selectedDrpItem: "",
    searchTxt: "",
    limit: 99,
    selectedProjects: [],
    showPublishModal: false,
    solutionSelectedForPublish: null,
    publishCmpKey: 999,
  }),
  computed: {
    numberOfPages() {
      return Math.ceil(this.projects.length / this.itemsPerPage);
    },
  },
  watch: {
    searchTxt(val) {
      if (val) {
        this.getSolutionAutocompleteList(val);
      } else {
        this.getSolutionAutocompleteList("");
        this.getProjectList();
      }
    },
    selectedDrpItem() {
      this.getProjectList();
    },
  },
  created() {
    this.$appHub.$on("refresh-solution-list", this.getProjectList);
  },
  mounted() {
    this.getProjectList();
  },
  methods: {
    //#region Create new solution
    onNewSolutionModalClose() {
      this.newSolutionDialog = false;
    },
    createNewSolution() {
      this.solutionCmpKey++;
      this.newSolutionDialog = true;
    },
    onFinalSave(model) {
      if (model.projects.length == 0) {
        this.$notify({
          title: "Warning",
          text: "Add project",
          type: "warning",
        });
        return;
      }
      this.$appHub.createNewSolution(model);
      this.newSolutionDialog = false;
    },
    //#endregion
    //#region Update Solution
    showProjectList(selectedItem) {
      this.selectedSolution = selectedItem;
      this.selectedProjects = [];
      selectedItem.projects.forEach((proj) => {
        this.selectedProjects.push(proj.name);
      });
      this.showProjectDetails = true;
    },
    updateSelectedSolution(data) {
      this.selectedSolution = data;
      data.projects.forEach((proj) => {
        this.selectedProjects.push(proj.name);
      });
    },
    onProjectDetailsModalClose() {
      this.showProjectDetails = false;
      this.selectedSolution = null;
      this.selectedProjects = [];
      this.getProjectList();
    },
    //#endregion
    //#region solution action

    buildSolutionClicked(item) {
      var loc = item.workingDirectory;
      if (item.solutionName) {
        loc = loc + "\\" + item.solutionName;
      }
      this.$appHub.buildSolution(loc);
    },
    restoreSolutionClicked(item) {
      var loc = item.workingDirectory;
      if (item.solutionName) {
        loc = loc + "\\" + item.solutionName;
      }
      this.$appHub.restoreSolution(loc);
    },
    cleanSolutionClicked(item) {
      var loc = item.workingDirectory;
      if (item.solutionName) {
        loc = loc + "\\" + item.solutionName;
      }
      this.$appHub.cleanSolution(loc);
    },

    //#endregion
    //#region publish
    publishSolutionClicked(item) {
      this.publishCmpKey++;
      this.showPublishModal = true;
      this.solutionSelectedForPublish = item;
    },
    closePublishModal() {
      this.showPublishModal = false;
      this.solutionSelectedForPublish = null;
    },
    onFinalPublishSolutionClick(data) {
      this.showPublishModal = false;
      this.solutionSelectedForPublish = null;
      this.$appHub.publishSolution(data);
    },
    //#endregion
    //#region List
    getProjectList() {
      var url = "/api/dotnetui/ProjectManager/List?Limit=" + this.limit;
      if (this.selectedDrpItem) {
        url = url + "&filter=" + this.selectedDrpItem;
      }
      this.$httpClient
        .get(this.$siteName + url)
        .then((res) => {
          this.projects = res.data;
        })
        .catch((error) => {
          console.log(error);
        });
    },
    getCardClass(index) {
      var classArray = [
        "ui-card ui-card-red",
        "ui-card ui-card-cyan",
        "ui-card ui-card-blue",
        "ui-card ui-card-orange",
      ];

      if (index > 3) index = index % 4;
      return classArray[index];
    },

    getSolutionAutocompleteList(val) {
      this.$httpClient
        .get(
          this.$siteName +
            "/api/dotnetui/ProjectManager/AutoComplete?Filter=" +
            val
        )
        .then((res) => {
          this.slnDrpList = res.data;
        })
        .catch((error) => {
          console.log(error);
        });
    },
    //#endregion
  },
  destroyed() {
    this.$appHub.$off("refresh-solution-list", this.getProjectList());
  },
};
</script>
<style scoped>
.ui-card p {
  color: black;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
  /* color: #000; */
}
.ui-card {
  border-radius: 5px;
  box-shadow: 0px 1px 2px 0px;
  padding-left: 10px;
  padding-top: 20px;
  padding-right: 20px;
  margin: 0px;
}
.ui-card-red {
  /* border-top: 5px solid #fce8e6; */
  /* background: #fce8e6; */
}
.ui-card-cyan {
  /* border-top: 5px solid #e6f4ea; */
  /* background: #e6f4ea; */
}

.ui-card-blue {
  /* border-top: 5px solid #e8f0fe; */
  /* background: #e8f0fe; */
}
.ui-card-orange {
  /* border-top: 5px solid #fef7e0; */
  /* background: #fef7e0; */
}

@media (max-width: 450px) {
  .ui-card {
    height: 200px;
  }
}

@media (max-width: 950px) and (min-width: 450px) {
  .ui-card {
    text-align: center;
    height: 160px;
  }
}
</style>
