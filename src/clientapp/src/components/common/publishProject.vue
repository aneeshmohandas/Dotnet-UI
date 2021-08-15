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
          <v-btn icon dark @click="onPublishProfileModalClose">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>{{ solution.solutionName }} </v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items> </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <v-row justify="space-around" style="padding-top: 25px">
            <v-col cols="12" md="12">
              <v-sheet class="pa-12" rounded="xl" color="grey lighten-3">
                <v-card max-width="100%" class="mx-auto">
                  <v-toolbar color="primary" dark>
                    <v-toolbar-title>Publish Configurations</v-toolbar-title>
                    <v-spacer></v-spacer>
                    <template v-slot:extension>
                      <v-btn
                        fab
                        color="warning"
                        bottom
                        right
                        absolute
                        @click="showNewProfileModal = true"
                      >
                        <v-icon>mdi-plus</v-icon>
                      </v-btn>
                    </template>
                  </v-toolbar>

                  <v-divider></v-divider>
                  <v-card-text>
                    <div style="margin-top: 25px">
                      <template  v-if="solution.publishProfiles.length > 0">
                      <v-container
                        fluid
                      >
                        <v-row justify="center">
                          <v-expansion-panels popout focusable>
                            <v-expansion-panel
                              v-for="item in solution.publishProfiles"
                              :key="item.id"
                              hide-actions
                            >
                              <v-expansion-panel-header>
                                <v-row align="center" class="spacer" no-gutters>
                                  <v-col cols="4" sm="2" md="1">
                                    <v-menu>
                                      <template
                                        v-slot:activator="{ on, attrs }"
                                      >
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
                                        <v-list-item
                                          @click="onPublishBtnClick(item)"
                                        >
                                          <v-list-item-title
                                            >Publish</v-list-item-title
                                          >
                                        </v-list-item>
                                      </v-list>
                                    </v-menu>
                                  </v-col>

                                  <v-col class="hidden-xs-only" sm="5" md="3">
                                    <strong v-html="item.profileName"></strong>
                                  </v-col>

                                  <v-col class="text-no-wrap" cols="5" sm="3">
                                    <v-chip
                                      color="primary"
                                      class="ml-0 mr-2 black--text"
                                      label
                                      small
                                      outlined
                                    >
                                      Folder
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
                                    {{ item.configuration }}
                                  </v-col>
                                  <v-col>
                                    <v-chip
                                      color="success"
                                      class="ml-0 mr-2 black--text"
                                      label
                                      small
                                      title="Last change"
                                      outlined
                                    >
                                      <template v-if="item.lastModifiedTime">
                                        {{
                                          $dayjs(
                                            item.lastModifiedTime
                                          ).fromNow()
                                        }}
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
                                        <b>Profile Name</b>
                                      </label>
                                      <div>
                                        {{ item.profileName }}
                                      </div>
                                    </div>
                                    <div class="col-md-4">
                                      <label>
                                        <b> Folder Location</b>
                                      </label>
                                      <div>
                                        {{ item.outputDirectory }}
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
                                        <b>Configuration</b>
                                      </label>
                                      <div>
                                        {{ item.configuration }}
                                      </div>
                                    </div>
                                    <div class="col-md-4">
                                      <label>
                                        <b>Self Contained</b>
                                      </label>
                                      <div>
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
                      </template>
                      <template v-else>
                          <v-row justify="center">
                    <div style="padding: 10px">
                      <v-img
                        max-height="250"
                        max-width="250"
                        src="@/assets/no_data.svg"
                      ></v-img>
                      <v-row justify="center" style="padding-top: 40px">
                        &nbsp;<a @click="showNewProfileModal = true">
                          Create new profile</a
                        >
                      </v-row>
                    </div>
                  </v-row>
                      </template>
                    </div>
                  </v-card-text>
                  <v-card-actions> </v-card-actions>
                </v-card>
              </v-sheet>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-dialog>
    <v-dialog
      v-model="showNewProfileModal"
      persistent
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      no-click-animation
    >
      <v-card tile>
        <v-toolbar flat dark color="primary">
          <v-btn icon dark @click="showNewProfileModal = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>New Publish Configuration </v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn
              dark
              @click="onPublishProfileSaveClick"
              :disabled="!valid"
              text
            >
              Publish
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <v-form v-model="valid">
            <v-row>
              <v-col cols="12" md="3">
                <v-checkbox
                  v-model="publishModel.saveAsProfile"
                  label="Save as profile"
                ></v-checkbox>
              </v-col>
              <v-col cols="12" md="3">
                <v-text-field
                  v-model="publishModel.profileName"
                  :counter="100"
                  label="Profile Name"
                  :disabled="!publishModel.saveAsProfile"
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="3">
                <v-select
                  v-model="publishModel.configuration"
                  :items="configList"
                  label="Configuration"
                  :rules="[(v) => !!v || 'Type is required']"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" md="3">
                <div style="margin-top: 10px">
                  <v-chip
                    class="ma-2"
                    color="deep-purple accent-4"
                    @click="DirectoryModal = true"
                    outlined
                  >
                    <v-icon left> mdi-folder </v-icon>
                    <span v-if="publishModel.outputDirectory">
                      {{ publishModel.outputDirectory }}
                    </span>
                    <span v-else> Select output directory </span>
                  </v-chip>
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
    <treeview
      ref="directoryTree"
      :showModal="DirectoryModal"
      @modalCloseClicked="DirectoryModal = false"
      @onSetDirectory="onOutputDirectorySaveClick"
    />
  </v-container>
</template>
<script>
import treeview from "@/components/common/treeView.vue";

export default {
  components: { treeview },
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    solution: {},
  },
  data: () => ({
    valid: false,
    configList: ["Debug", "Release"],
    publishModel: {
      profileName: "",
      configuration: "Release",
      outputDirectory: "",
      solutionId: "",
      saveAsProfile: false,
    },
    DirectoryModal: false,
    showNewProfileModal: false,
    pId: 1000,
  }),
  mounted() {
    this.publishModel.solutionId = this.solution.solutionId;
  },
  methods: {
    onPublishProfileModalClose() {
      this.$emit("onCloseBtnClick");
    },
    onPublishProfileSaveClick() {
      if (!this.publishModel.outputDirectory) {
        this.$notify({
          title: "Warning",
          text: "Select a directory",
          type: "warning",
        });
        return;
      } else {
        this.showNewProfileModal = false;
        this.$emit("onPublishClick", this.publishModel);
      }
    },
    onOutputDirectorySaveClick(direc) {
      this.publishModel.outputDirectory = direc;
      this.DirectoryModal = false;
    },
    onPublishBtnClick(item) {
      item.solutionId = this.publishModel.solutionId;
      this.$emit("onPublishClick", item);
    },
  },
};
</script>
