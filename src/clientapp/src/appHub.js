import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default {
  install(Vue) {
    const appHub = new Vue();
    Vue.prototype.$appHub = appHub;

    let connection = null;
    let startedPromise = null;
    let manuallyClosed = false;

    Vue.prototype.startSignalR = () => {
      connection = new HubConnectionBuilder()
        .withUrl(`${Vue.prototype.$siteName}/appHub`)
        .configureLogging(LogLevel.Information)
        .build();

      connection.on("ReceiveMessage", (message) => {
        appHub.$emit("message-received", message);
      });
      connection.on("BlockUI", () => {
        appHub.$emit("block-ui");
      });
      connection.on("EnableUI", () => {
        appHub.$emit("enable-ui");
      });
      connection.on("UpdatedProject", (data) => {
        appHub.$emit("updated-project", data);
      });
      connection.on("UpdatedSolution", (data) => {
        appHub.$emit("updated-solution", data);
      });
      connection.on("RefreshSolutionList", () => {
        appHub.$emit("refresh-solution-list");
      });

      function start() {
        startedPromise = connection.start().catch((err) => {
          console.error("Failed to connect with hub", err);
          return new Promise((resolve, reject) =>
            setTimeout(
              () =>
                start()
                  .then(resolve)
                  .catch(reject),
              5000
            )
          );
        });
        return startedPromise;
      }
      connection.onclose(() => {
        if (!manuallyClosed) start();
      });

      // Start everything
      manuallyClosed = false;
      start();
    };
    Vue.prototype.stopSignalR = () => {
      if (!startedPromise) return;

      manuallyClosed = true;
      return startedPromise
        .then(() => connection.stop())
        .then(() => {
          startedPromise = null;
        });
    };
    appHub.addPackageToProject = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("AddNewPackageToProject", data))
        .catch(console.error);
    };
    appHub.createNewSolution = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("ManageProjectCreationRequest", data))
        .catch(console.error);
    };
    appHub.addProjectToSolution = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("AddNewProjectToSolution", data))
        .catch(console.error);
    };
    //AdNewProjectToSolution
    //AddNewProjectToSolution
    appHub.restoreSolution = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("RestoreSolution", data))
        .catch(console.error);
    };
    appHub.cleanSolution = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("CleanSolution", data))
        .catch(console.error);
    };
    appHub.buildSolution = (data) => {
      if (!startedPromise) return;

      return startedPromise
        .then(() => connection.invoke("BuildSolution", data))
        .catch(console.error);
    };
    appHub.publishSolution= (data) => {
      if (!startedPromise) return;
      return startedPromise
        .then(() => connection.invoke("PublishSolution", data))
        .catch(console.error);
    };
  },
};
