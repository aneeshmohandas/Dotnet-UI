<template>
  <div>
    <v-row justify="space-around">
      <v-col cols="12" md="12">
        <v-sheet class="pa-12" rounded="xl" color="grey lighten-3">
          <v-card max-width="100%" class="mx-auto">
            <v-toolbar color="primary" dark>
              <v-toolbar-title>Projects</v-toolbar-title>
              <v-spacer></v-spacer>
              <template v-slot:extension>
                <v-btn
                  fab
                  color="warning"
                  bottom
                  right
                  absolute
                  @click="onOpenNewProjectModal()"
                >
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </template>
            </v-toolbar>

            <v-divider></v-divider>
            <v-card-text>
              <div style="margin-top: 25px">
                <v-container fluid>
                  <v-row justify="center">
                    <v-expansion-panels popout focusable>
                      <v-expansion-panel
                        v-for="item in solution.projects"
                        :key="item.id"
                        hide-actions
                      >
                        <v-expansion-panel-header>
                          <v-row align="center" class="spacer" no-gutters>
                            <v-col cols="4" sm="2" md="1">
                              <v-menu>
                                <template v-slot:activator="{ on, attrs }">
                                  <v-btn
                                    bottom
                                    left
                                    icon
                                    v-bind="attrs"
                                    v-on="on"
                                  >
                                    <v-icon>mdi-dots-vertical</v-icon>
                                  </v-btn>
                                </template>

                                <v-list>
                                  <v-list-item @click="addPackageClick(item)">
                                    <v-list-item-title
                                      >Add Package</v-list-item-title
                                    >
                                  </v-list-item>
                                </v-list>
                              </v-menu>
                            </v-col>

                            <v-col class="hidden-xs-only" sm="5" md="3">
                              <strong v-html="item.name"></strong>
                            </v-col>

                            <v-col class="text-no-wrap" cols="5" sm="3">
                              <v-chip
                                color="primary"
                                class="ml-0 mr-2 black--text"
                                label
                                small
                                outlined
                              >
                                {{ item.packages.length }} Packages
                              </v-chip>
                              <strong></strong>
                            </v-col>

                            <v-col
                              class="
                                grey--text
                                text-truncate
                                hidden-sm-and-down
                              "
                            >
                              {{ item.type }}
                            </v-col>
                            <v-col>
                              <v-chip
                                color="success"
                                class="ml-0 mr-2 black--text"
                                label
                                small
                                outlined
                                title="Last change"
                              >
                                <template v-if="item.lastModifiedTime">
                                  {{ $dayjs(item.lastModifiedTime).fromNow() }}
                                </template>
                                <template v-else>
                                  {{ $dayjs(item.createdTime).fromNow() }}
                                </template>
                              </v-chip>
                              <strong></strong>
                            </v-col>
                          </v-row>
                        </v-expansion-panel-header>

                        <v-expansion-panel-content>
                          <v-divider></v-divider>
                          <v-card-text>
                            <div class="row">
                              <div class="col-md-4">
                                <label>
                                  <b>Name</b>
                                </label>
                                <div>
                                  {{ item.name }}
                                </div>
                              </div>
                              <div class="col-md-4">
                                <label>
                                  <b> Type</b>
                                </label>
                                <div>
                                  {{ item.type }}
                                </div>
                              </div>
                              <div class="col-md-4">
                                <label>
                                  <b>Created On</b>
                                </label>
                                <div>
                                  {{
                                    $dayjs(item.createdTime).format(
                                      "DD-MM-YYYY hh:mm a"
                                    )
                                  }}
                                </div>
                              </div>
                                <div class="col-md-4">
                                <label>
                                  <b>Last Modified On</b>
                                </label>
                                <div v-if="item.lastModifiedTime">
                                  {{
                                    $dayjs(item.lastModifiedTime).format(
                                      "DD-MM-YYYY hh:mm a"
                                    )
                                  }}
                                </div>
                                <div v-else>
                                  -
                                </div>
                              </div>
                             
                               
                            
                            </div>
                          </v-card-text>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </v-row>
                </v-container>
              </div>
            </v-card-text>
            <v-card-actions> </v-card-actions>
          </v-card>
        </v-sheet>
      </v-col>
    </v-row>
    <createNewProject
      :key="cmpKey"
      :showModal="showNewProjectModal"
      @saveClicked="onNewProjectAdded"
      @closeBtnClicked="onNewProjectClosed"
      :projectList="projectList"
    />

    <v-dialog
      v-model="showPackageModal"
      transition="dialog-bottom-transition"
      fullscreen
      persistent
      no-click-animation
    >
      <template>
        <v-toolbar dark color="primary">
          <v-btn icon dark @click="onPackageModalClose">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title
            ><span v-if="selectedProject"
              >Packages - {{ selectedProject.name }}</span
            ></v-toolbar-title
          >
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <!-- <v-btn dark text @click="onPackageSaveClick"> Save </v-btn> -->
          </v-toolbar-items>
        </v-toolbar>
        <v-card>
          <v-card-text v-if="selectedProject">
            <selectPackage
              :key="pId"
              ref="packageComp"
              @packageAdded="onNewPackageAddeed"
              :addedPackages.sync="selectedProject.packages"
              intialTab="AddedPackages"
            />
          </v-card-text>
          <v-card-actions class="justify-end"> </v-card-actions>
        </v-card>
      </template>
    </v-dialog>
  </div>
</template>
<script>
import createNewProject from "@/components/common/createNewProject.vue";
import selectPackage from "@/components/common/selectPackage.vue";

export default {
  components: { createNewProject, selectPackage },
  props: {
    solution: null,
    showAddProjectButton: {
      type: Boolean,
      default: false,
    },
    projectList: [],
  },

  created() {
    this.$appHub.$on("updated-project", this.UpdateProject);
    this.$appHub.$on("updated-solution", this.UpdateSolution);
  },
  mounted() {},
  data: () => ({
    showNewProjectModal: false,
    selectedProject: null,
    showPackageModal: false,
    pId: 1000,
    cmpKey: 999,
  }),
  methods: {
    onOpenNewProjectModal() {
      this.showNewProjectModal = true;
      this.cmpKey++;
    },
    onNewProjectAdded(dt) {
      this.showNewProjectModal = false;
      var data = {
        project: dt,
        solutionId: this.solution.solutionId,
      };
      this.$appHub.addProjectToSolution(data);
    },
    onNewProjectClosed() {
      this.showNewProjectModal = false;
    },
    addPackageClick(seletedProject) {
      this.selectedProject = Object.assign({}, seletedProject);
      this.showPackageModal = true;
    },
    onPackageSaveClick() {},
    onNewPackageAddeed(item) {
      var data = {
        package: item,
        projectId: this.selectedProject.id,
        solutionId: this.solution.solutionId,
      };
      this.$appHub.addPackageToProject(data);
    },
    onPackageModalClose() {
      this.showPackageModal = false;
      this.selectedProject = null;
    },
    UpdateProject(data) {
      if (this.selectedProject) this.selectedProject = data;
    },
    UpdateSolution(data) {
      this.$emit("updateSelectedSolution", data);
    },
  },
  destroyed() {
    this.$appHub.$off("updated-project", this.UpdateProject);
    this.$appHub.$off("updated-solution", this.UpdateSolution);
  },
};
</script>