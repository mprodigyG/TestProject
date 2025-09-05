//
//console.log(baseUrl);
console.log("httclient");
    const baseUrl = window.location.origin;
export function GetData(api, directoryPath) {

    //const baseUrl = baseUrl + api;
    const params = {
        search: 'directoryPath',
        filter: directoryPath
    };

    const url = new URL(baseUrl + api);
    url.search = new URLSearchParams(params).toString();

    console.log(url);
    //fetch(url)
    //    .then(response => response.json())
    //    .then(data => {
    //        // Handle the fetched data
    //    })
    //    .catch(error => {
    //        // Handle errors
    //    });
}