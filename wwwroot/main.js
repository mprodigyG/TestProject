//import * as httpClient from './search_directory/httpClient'
import { GetData } from "./search_directory/httpClient.js"

document.addEventListener('DOMContentLoaded', function () {
    // initionalize the app
    init();
});

function init() {
    // get directory and it's file and folder count
    const directoryPath = "";
    GetData("/test/directoryInfo", directoryPath);

    document.getElementById('txtSearchPath').value = "C:\\TestDirectory";


}

 




