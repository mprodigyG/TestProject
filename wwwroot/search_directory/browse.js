function internalFunction() {
    console.log("Internal to myModule.");

    const txtSearchPath2 = document.getElementById("lblTotalFiles");
    console.log(txtSearchPath2.textContent);
    txtSearchPath2.textContent = "9999";
    console.log(txtSearchPath2.textContent);
}

export function publicFunction() {
    console.log("Public from myModule.");
    internalFunction();
}