 <template>
  <v-dialog
    v-model="showModal"
    transition="dialog-bottom-transition"
    fullscreen
    persistent
    no-click-animation
  >
    <template>
      <v-toolbar dark color="primary">
        <v-btn icon dark @click="onProjectSaveModalClose">
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-toolbar-title>Project details</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-toolbar-items>
          <v-btn dark text @click="onProjectDetailSaveClick" :disabled="!valid">
            Save
          </v-btn>
        </v-toolbar-items>
      </v-toolbar>
      <v-card>
        <v-card-text>
          <v-form v-model="valid">
            <v-container>
              <v-row>
                <v-col cols="12" md="4">
                  <v-select
                    v-model="newProject.type"
                    :items="projectTemplates"
                    label="Project Template"
                    :rules="[(v) => !!v || 'Type is required']"
                    required
                  ></v-select>
                </v-col>
                <v-col cols="12" md="4">
                  <v-text-field
                    v-model="newProject.name"
                    :rules="nameRules"
                    :counter="100"
                    label="Project Name"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12" md="4">
                  <v-select
                    v-model="referProject"
                    :items="projectList"
                    label="Add Reference"
                  ></v-select>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
          <selectPackage
            :key="pId"
            ref="packageComp"
            @packageAdded="onNewPackageAddeed"
            :addedPackages.sync="newProject.packages"
          />
        </v-card-text>
        <v-card-actions class="justify-end"> </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>
<script>
import selectPackage from "@/components/common/selectPackage.vue";

export default {
  components: { selectPackage },
  props: {
    showModal: {
      type: Boolean,
      default: false,
    },
    projectList: []
  },
  data: () => ({
    valid: false,
    projectDetailsModal: false,
    projectTemplates: [],
    referProject:"",
    newProject: {
      name: "",
      type: "",
      linkProject: [],
      packages: [],
    },
    nameRules: [
      (value) => !!value || "Required.",
      (value) => (value && value.length >= 3) || "Min 3 characters",
    ],

    pId: 1000,
  }),
  mounted() {
    this.getProjectTemplate();
  },
  methods: {
    onOpenNewProjectModal() {},
    onProjectSaveModalClose() {
      this.$emit("closeBtnClicked");
    },
    onProjectDetailSaveClick() {
      var packages = this.$refs.packageComp.addedPackages;
      this.newProject.packages = packages;
      if(this.referProject){
        this.newProject.linkProject.push(this.referProject);
      }
      var proj = Object.assign({}, this.newProject);
      this.$emit("saveClicked", proj);
    },
    getProjectTemplate() {
      this.$httpClient
        .get(this.$siteName + "/api/dotnetui/home/GetTemplates")
        .then((res) => {
          // handle success
          this.projectTemplates = res.data;
        })
        .catch((error) => {
          // handle error
          console.log(error);
        });
    },
    onNewPackageAddeed(pack) {
      this.newProject.packages.push(pack);
      this.$notify({
        title: "Success",
        text: "Package  added",
        type: "Success",
      });
    },
  },
};
</script>