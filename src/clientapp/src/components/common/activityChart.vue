<template>
  <v-container class="col">
     <h4 class=" text-normal mb-2">
          30 Actions in last year
      </h4>
    <div class="d-ui-box">
      <calendar-heatmap
        :values="values"
        :endDate="'2021-08-31'"
        tooltip-unit="actions"
      />
    </div>
  </v-container>
</template>
<script>
import { CalendarHeatmap } from "vue-calendar-heatmap";

export default {
  components: {
    CalendarHeatmap,
  },
  data: () => ({
    values: [
      { date: "2021-07-22", count: 6 },
      { date: "2021-07-26", count: 4 },
    ],
  }),
};
</script>
<style >
.d-ui-box {
  border: 1px solid lightgrey;
  padding: 0.9em;
}
.text-normal {
    font-weight: 400 !important;
}
</style>