<template>
  <v-container fluid>
    <v-dialog
      v-model="showModal"
      persistent
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      no-click-animation
    >
      <v-card tile>
        <v-toolbar flat dark color="primary">
          <v-btn icon dark @click="onNewSoltiuonClose">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>Create Solution</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn dark @click="onNewSoltiuonSave" :disabled="!valid" text>
              Save
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <v-form v-model="valid">
            <v-row>
              <v-col cols="12" md="3">
                <div style="margin-top: 10px">
                  <v-chip
                    class="ma-2"
                    color="deep-purple accent-4"
                    @click="DirectoryModal = true"
                    outlined
                    :disabled="disableDirSelection"
                  >
                    <v-icon left> mdi-folder </v-icon>
                    <span v-if="this.reqModel.workingDirectory">
                      {{ this.reqModel.workingDirectory }}
                    </span>
                    <span v-else> Select directory </span>
                  </v-chip>
                </div>
              </v-col>
              <v-col cols="12" md="3">
                <v-checkbox
                  disabled
                  v-model="reqModel.createSolution"
                  label="Add solution file"
                ></v-checkbox>
              </v-col>
              <v-col cols="12" md="3">
                <v-text-field
                  :disabled="!reqModel.createSolution"
                  required
                  :rules="[(v) => !!v || 'Name is required']"
                  v-model="reqModel.solutionName"
                  label="Solution Name"
                ></v-text-field>
              </v-col>

              <v-col cols="12" md="3">
                <v-checkbox
                  v-model="reqModel.createGitIgnoreFile"
                  label="Add dotnet gitignore file"
                ></v-checkbox>
              </v-col>
              <v-col cols="12" md="12">
                <v-textarea
                  label="Description"
                  auto-grow
                  outlined
                  rows="3"
                  v-model="reqModel.description"
                  row-height="25"
                  shaped
                ></v-textarea>
              </v-col>
            </v-row>
          </v-form>
          <v-data-iterator
            :items="reqModel.projects"
            :items-per-page.sync="itemsPerPage"
            hide-default-footer
          >
            <template v-slot:header>
              <v-toolbar class="mb-2" color="indigo darken-5" dark flat>
                <v-toolbar-title>Projects</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn
                  @click="onOpenNewProjectModal()"
                  class="mx-2"
                  rounded
                  color="white"
                  dark
                  outlined
                >
                  Add New Project
                </v-btn>
              </v-toolbar>
            </template>

            <template v-slot:default="props">
              <v-row>
                <v-col
                  v-for="item in props.items"
                  :key="item.name"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                >
                  <v-card>
                    <v-card-title class="subheading font-weight-bold">
                      {{ item.name }}
                    </v-card-title>

                    <v-divider></v-divider>

                    <v-list>
                      <v-list-item>
                        <v-list-item-content>Type:</v-list-item-content>
                        <v-list-item-content class="align-end">
                          {{ item.type }}
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item>
                        <v-list-item-content>Packages:</v-list-item-content>
                        <v-list-item-content class="align-end">
                          {{ item.packages.length }}
                        </v-list-item-content>
                      </v-list-item>
                    </v-list>
                  </v-card>
                </v-col>
              </v-row>
            </template>
          </v-data-iterator>
        </v-card-text>

        <div style="flex: 1 1 auto"></div>
      </v-card>
    </v-dialog>

    <!--Modal Project Details-->
    <v-col cols="auto">
      <createNewProject
        :key="pId"
        :showModal="projectDetailsModal"
        @saveClicked="onNewProjectAdded"
        @closeBtnClicked="onProjectSaveModalClose"
        :projectList="projectList"
      />
    </v-col>
    <!--Modal DirectoryModal -->

    <treeview
      ref="directoryTree"
      :showModal="DirectoryModal"
      @modalCloseClicked="onDirectoryModalCloseClick"
      @onSetDirectory="onWorkDirectorySaveClick"
    />

    <!-- List of projects -->
  </v-container>
</template>
<script>
import treeview from "@/components/common/treeView.vue";
import createNewProject from "@/components/common/createNewProject.vue";

export default {
  components: { treeview, createNewProject },
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
  },
  data: () => ({
    valid: false,
    projectDetailsModal: false,
    reqModel: {
      projects: [],
      createSolution: true,
      createGitIgnoreFile: true,
      workingDirectory: "",
      solutionName: "",
    },
    projectList: [],

    itemsPerPage: 3,
    items: [],
    nameRules: [
      (value) => !!value || "Required.",
      (value) => (value && value.length >= 3) || "Min 3 characters",
    ],
    DirectoryModal: false,
    pId: 1000,
    disableDirSelection: false,
  }),
  mounted() {
    this.getDefaultDirectory();
  },
  methods: {
    onOpenNewProjectModal() {
      this.projectDetailsModal = true;
      this.pId++;
    },
    onProjectSaveModalClose() {
      this.projectDetailsModal = false;
    },

    onWorkDirectorySaveClick(direc) {
      this.reqModel.workingDirectory = direc;
      this.DirectoryModal = false;
    },

    onNewProjectAdded(dt) {
      this.reqModel.projects.push(dt);
      this.projectList.push(dt.name);
      this.projectDetailsModal = false;
    },
    onNewSoltiuonClose() {
      this.$emit("onCloseClicked");
    },
    onNewSoltiuonSave() {
      if (!this.reqModel.workingDirectory) {
        this.$notify({
          title: "Warning",
          text: "Select a directory",
          type: "warning",
        });
        return;
      } else if (this.reqModel.projects.length == 0) {
        this.$notify({
          title: "Warning",
          text: "Add project",
          type: "warning",
        });
        return;
      } else {
        this.$emit("onSolutionSave", this.reqModel);
      }
    },
    onDirectoryModalCloseClick() {
      this.DirectoryModal = false;
    },
    getDefaultDirectory() {
      this.$httpClient
        .get(this.$siteName + "/api/dotnetui/AppConfig/DefaultWorkingDirectory")
        .then((res) => {
          // handle success
          //  console.log(res.data.data)
          this.reqModel.workingDirectory = res.data.data.directory;
          this.disableDirSelection = res.data.data.disableSelection;
        })
        .catch((error) => {
          // handle error
          console.log(error);
        });
    },
  },
};
</script>