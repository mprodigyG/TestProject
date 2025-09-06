const baseUrl = window.location.origin;

export async function GetData(api) {
    // actual url being send to fetch the data
    const url = new URL(baseUrl + api);

    // get the data from the web server and return it in JSON format
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Status: ${response.status}`);
        }
        const directoryInfo = await response.json();

        // the return data in javascript form
        return directoryInfo;
    } catch (error) {
        console.error("Error getting data: ", error);
    }
}