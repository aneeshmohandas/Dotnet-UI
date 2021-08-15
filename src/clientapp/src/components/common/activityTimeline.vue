<template>
  <v-container class="col">
    <v-row>
      <v-col cols="9">
        <h4 class="text-normal mb-2"></h4>

        <div>
          <v-timeline dense clipped v-if="timelineData.length > 0">
            <v-slide-x-transition group>
              <template v-for="data in timelineData">
                <v-timeline-item
                  class="mb-4"
                  color="grey"
                  icon-color="grey lighten-2"
                  small
                  :key="data.id"
                >
                  <v-row justify="space-between">
                    <v-col cols="7">
                      {{ data.message }}
                      <div>
                        {{
                          $dayjs(data.createdOn).format("dddd, DD MMMM YYYY")
                        }}
                      </div>
                    </v-col>
                    {{ $dayjs(data.createdOn).format("hh:mm a") }}
                  </v-row>
                </v-timeline-item>
              </template>
            </v-slide-x-transition>
          </v-timeline>
          <template v-else>
            <v-row justify="center">
              <div style="padding: 10px">
                <v-img
                  max-height="400"
                  max-width="400"
                  src="@/assets/no_data.svg"
                ></v-img>
                <v-row justify="center" style="padding-top: 40px">
                  &nbsp;<a > No data</a>
                </v-row>
              </div>
            </v-row>
          </template>
        </div>
      </v-col>

      <v-col cols="3">
        <v-list rounded dense>
          <v-list-item-group v-model="select" color="primary">
            <v-list-item v-for="(item, i) in items" :key="i">
              <v-list-item-icon> </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title v-text="item"></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
export default {
  data: () => ({
    select: 0,
    items: ["Today", "Last 24 Hours", "Last Week"],
    timelineData: [],
  }),
  watch: {
    select(nv) {
      if (nv || nv == 0) {
        this.getTimelineData();
      }
    },
  },
  mounted() {
    this.getTimelineData();
  },
  methods: {
    getTimelineData() {
      this.$httpClient
        .get(
          this.$siteName +
            "/api/dotnetui/activity/timeline?Type=" +
            this.items[this.select]
        )
        .then((res) => {
          // handle success
          this.timelineData = res.data;
        })
        .catch((error) => {
          // handle error
          console.log(error);
        });
    },
  },
};
</script>
<style >
.d-ui-border-bottom {
  border-bottom: 1px solid lightgrey !important;
}
</style>