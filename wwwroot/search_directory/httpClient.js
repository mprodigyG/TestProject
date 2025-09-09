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

export async function DeleteDirectoryFile(api) {
    // actual url being send to fetch the data
    const url = new URL(baseUrl + api);

    // delete the file from the server based on it's full path
    try {
        const response = await fetch(url, {
            method: 'DELETE'
        });
        if (!response.ok) {
            throw new Error(`Status: ${response.status}`);
        }
        const result = await response.text();
        return result;
    } catch (error) {
        console.error("Error getting data: ", error);
    }
}

export async function CopyDirectoryFile(api, sourceLocation, destinationLocation) {
    // actual url being send to fetch the data
    const url = new URL(baseUrl + api);
    const data = {
        sourceLocation: sourceLocation,
        destinationLocation: destinationLocation
    };

    // Copy or move move file withing the scope directory
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            throw new Error(`Status: ${response.status}`);
        }
        const result = await response.text();
        return result;
    } catch (error) {
        console.error("Error getting data: ", error);
    }
}