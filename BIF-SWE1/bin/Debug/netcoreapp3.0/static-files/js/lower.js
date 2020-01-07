class ToLowerManager {
  temperatures = [];
  paginator = undefined;
  apiUrl = "http://localhost:8080/tolower";
  pageIndex = 1;
  input = document.getElementById("lower-input");
  output = document.getElementById("output");

  fetchData() {
    let resp = fetch(this.apiUrl, {method: "POST", body: this.input.value}).then((data) => data.text())
      .then((data) => {
        this.displayText(data);
      });
  }

  displayText(text) {
    this.output = document.getElementById("output");
    this.output.innerHTML = text;
  }
}

const toLowerManager = new ToLowerManager();
