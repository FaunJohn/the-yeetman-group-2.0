class NaviManager {
  temperatures = [];
  paginator = undefined;
  apiUrl = "http://localhost:8080/navi";
  pageIndex = 1;
  input = document.getElementById("navi-input");
  output = document.getElementById("navi-output");

  unavailableResponse = `
    <div class="text-warning navi-warning-output-wrapper">
        <i class="material-icons navi-warning-output-icon">
        warning
        </i>
        Server busy
    </div>
    `;

  fetchData() {
    this.input = document.getElementById("navi-input");
    let resp = fetch(this.apiUrl, {method: "POST", body: this.input.value})
      .then((resp) => {
        this.displayResponse(resp);
      });
  }

  displayResponse(resp) {
    this.output = document.getElementById("navi-output");
    if(resp.status == 200) {
      clearInterval(this.reloadInterval);
      this.setLoadingStatus(false);
      const data = resp.json();
      data.then((d) => {
        let li = `<ul class="collection">`;
        d.forEach(city => {
          li += `
                        <li class="collection-item">
                            ${city}
                        </li>
                    `;
        });
        li += "</ul>";
        this.output.innerHTML = li;
      });
    } else if(resp.status == 503) {
      this.output.innerHTML = this.unavailableResponse;
    }
  }

  reloadMapData() {
    this.output = document.getElementById("navi-output");
    this.output.innerHTML = "Loading...";
    let resp = fetch(this.apiUrl + "/reload", {method: "GET"})
      .then((resp) => {
        if(resp.status == 503) {
          this.output.innerHTML = this.unavailableResponse;
        } else if(resp.status == 200) {
          this.setLoadingStatus(true);
        }
      });
  }

  setLoadingStatus(newState) {
    this.reloadingMapDataIndicatorWrapper = document.getElementById("navi-reload");
    if(newState) {
      this.reloadingMapDataIndicatorWrapper.innerHTML = "";
    } else {
      this.reloadingMapDataIndicatorWrapper.innerHTML = "";
    }
  }
}

const naviManager = new NaviManager();
