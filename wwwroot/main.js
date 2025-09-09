//import * as httpClient from './search_directory/httpClient'
import { CopyDirectoryFile, DeleteDirectoryFile, GetData } from "./search_directory/httpClient.js"

const testApi = "/test";    // default api url
let rootDirectoryPath = ""; // The to level directly selected at start up
let currentRootDirectoryPath = ""; // sub directory at or below the root directory
var listOfFolder = [];

document.addEventListener('DOMContentLoaded', function () {
    // initionalize the app
    init();
});

function init() {
    // get the initionial set for the directory data
    initDirectoryInfo();

    $("#btnSearchFileAndFolder").on("click", function () {

    });

    $('#btnUpload').click(function () {
        if ($('#uploadFile').val() === '') {
            alert('Please select a file.');
            return;
        }

        // upload the file if it is select by the users
        uploadFile();
    });

    $(".up-to-folder").on("click", function () {
        upToFolder();
    });

    $("#dialogCopyFile").dialog({
        autoOpen: false,
        minHeight: 400,
        minWidth: 600,
        modal: true
    });

    $("#dialogMoveFile").dialog({
        autoOpen: false,
        minHeight: 400,
        minWidth: 600,
        modal: true
    });

    $("#btnSaveCopy").on("click", function () {
        copyFileSave();
    });

    $("#btnSaveMove").on("click", function () {
        moveFileSave();
    });
}

async function initDirectoryInfo() {
    try {
        const directoryPath = document.getElementById('txtSearchPath').value;


        /* Return JSON data from the api called

            Array of object
                string? Name
                string? Path
                string? Type
                double? Size
            property
                FileCount
                FolderCount
                directoryPath
                DirectoryFolders
        */

        // get directory and it's file and folder count
        const directoryInfo = await GetData(`${testApi}/directoryInfo/${encodeURIComponent(directoryPath == "" ? " " : directoryPath)}/`);

        loadDirectoryInfoTable(directoryInfo);

        rootDirectoryPath = directoryInfo.directoryPath;
        // this keeps the current path from being change if the root direct get update and need to refresh
        currentRootDirectoryPath = (currentRootDirectoryPath == "" ? directoryInfo.directoryPath : currentRootDirectoryPath);
        document.getElementById('txtSearchPath').value = directoryInfo.directoryPath;
        document.getElementById('lblTotalFiles').textContent = directoryInfo.FileCount;
        document.getElementById('lblTotalFolders').textContent = directoryInfo.FolderCount;
        listOfFolder = directoryInfo.DirectoryFolders;
        $("#folders").autocomplete({
            source: listOfFolder,
            minLength: 0
        });
    } catch (error) {
        console.error(error);
    }
}

async function GetCurrentDirectoryInfo() {
    // get directory and it's file and folder count
    const directoryInfo = await GetData(`${testApi}/directoryInfo/${encodeURIComponent(currentRootDirectoryPath)}`);

    loadDirectoryInfoTable(directoryInfo);
}

async function GetFilterDirectoryInfo() {
    // get directory and it's file and folder count
    const directoryInfo = await GetData(`${testApi}/directoryInfo/${encodeURIComponent(currentRootDirectoryPath)}/${$("#txtSearchFileAndFolder").val()}`);

    loadDirectoryInfoTable(directoryInfo);
}

function loadDirectoryInfoTable(directoryInfo) {
    // First, clear existing table data
    clearTable();

    // Find a <table> element with id="myTable":
    let rowIndex = 1;
    let fileCount = 0;
    let folderCount = 0;
    let sizeKB = 0;
    var table = document.getElementById("tblDirectoryInfo");
    var row;
    var rowAction;
    var name;
    var type;
    var size;
    var path;

    if (directoryInfo.DirectoryInfo) {
        for (const directoryInfoRow of directoryInfo.DirectoryInfo) {
            // Add a row
            row = table.insertRow(rowIndex);

            // Add columns to the new row
            rowAction = row.insertCell(0);
            name = row.insertCell(1);
            type = row.insertCell(2);
            size = row.insertCell(3);
            path = row.insertCell(4);
            name.innerHTML = directoryInfoRow.Name;
            type.innerHTML = directoryInfoRow.Type;
            size.innerHTML = directoryInfoRow.Size;
            path.innerHTML = directoryInfoRow.Path;

            if (directoryInfoRow.Type === "File") {
                fileCount++;
                rowAction.innerHTML =
                    `<button data-path="${directoryInfoRow.Path}" data-name="${directoryInfoRow.Name}" class="ui-button ui-corner-all copy-file">
                        <span class="ui-icon ui-icon-copy"></span> Copy
                    </button>&nbsp;&nbsp;&nbsp;
                    <button data-path="${directoryInfoRow.Path}" data-name="${directoryInfoRow.Name}" class="ui-button ui-corner-all move-file">
                        <span class="ui-icon ui-icon-arrowthick-1-e"></span> Move
                    </button>&nbsp;&nbsp;&nbsp;
                    <button data-path="${directoryInfoRow.Path}" class="ui-button ui-corner-all delete-file">
                        <span class="ui-icon ui-icon-closethick"></span> Delete
                    </button>
                `;
                type.innerHTML = `<button data-path="${directoryInfoRow.Path}" class="ui-button ui-corner-all download-file">
                        <span class="ui-icon ui-icon-arrowthickstop-1-s"></span> ${directoryInfoRow.Type}
                    </button>`;
            }
            else {
                rowAction.innerHTML = "";
                type.innerHTML = `<button data-path="${directoryInfoRow.Path}" class="ui-button ui-corner-all down-to-folder">
                        <span class="ui-icon ui-icon-arrowreturnthick-1-e"></span> ${directoryInfoRow.Type}
                    </button>`;
                folderCount++;
            }
            sizeKB += directoryInfoRow.Size;
            rowIndex++;
        }
        $(".download-file").on("click", function () {
            var path = $(this).data("path");
            downloadFile(path);
        });
        $(".copy-file").on("click", function () {
            var path = $(this).data("path");
            var name = $(this).data("name");
            copyFile(path, name);
        });
        $(".move-file").on("click", function () {
            var path = $(this).data("path");
            var name = $(this).data("name");
            moveFile(path, name);
        });
        $(".delete-file").on("click", function () {
            var path = $(this).data("path");
            deleteFile(path);
        });
        $(".down-to-folder").on("click", function () {
            var path = $(this).data("path");
            downToFolder(path);
        });
    }
    document.getElementById('lblCurrentDisplayFiles').textContent = fileCount;
    document.getElementById('lblCurrentDisplayFolder').textContent = folderCount;
    document.getElementById('lblCurrentDisplaySizeKB').textContent = sizeKB;
}

function clearTable() {
    var table = document.getElementById("tblDirectoryInfo");
    var rowCount = table.rows.length;
    // starting from 1 to skip the header row
    for (var i = rowCount - 1; i > 0; i--) {
        table.deleteRow(i);
    }
}

function uploadFile() {
    var formData = new FormData();
    var file = $('#uploadFile')[0].files[0];
    formData.append('file', file);

    $.ajax({
        url: `${testApi}/uploadfile/${encodeURIComponent(currentRootDirectoryPath)}`,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $('#status').text('File uploaded successfully: ' + response);
            setTimeout(function () {
                $('#status').text("");
            }, 3000);

            // reload the table data
            GetCurrentDirectoryInfo()
        },
        error: function () {
            $('#status').text('File upload failed.');
        }
    });
}

function downloadFile(filePath) {
    // the file to down load in the browser
    window.location.href = `${testApi}/downloadfile/${encodeURIComponent(filePath)}`;
}

function copyFile(filePath, fileName) {
    const newFileName = filePath;
    $("#dialogCopyFile").dialog("open");
    document.getElementById('lblFilePathCopy').textContent = currentRootDirectoryPath;
    document.getElementById('lblFileNameCopy').textContent = fileName;
    document.getElementById('txtNewFileNameCopy').value = "Copy of " + fileName;
}

async function copyFileSave() {
    const filePath = document.getElementById('lblFilePathCopy').textContent;
    const fileName = document.getElementById('lblFileNameCopy').textContent;
    const newFileName = document.getElementById('txtNewFileNameCopy').value;
    const result = await CopyDirectoryFile(`${testApi}/copyfile`, `${filePath}\\${fileName}`, `${filePath}\\${newFileName}`);
    $("#dialogCopyFile").dialog("close");
}

function moveFile(filePath, fileName) {
    const newFileName = filePath;
    $("#dialogMoveFile").dialog("open");
    document.getElementById('lblFilePathMove').textContent = currentRootDirectoryPath;
    document.getElementById('lblFileNameMove').textContent = fileName;
    $("#folders").val("");
}

async function moveFileSave() {
    if (listOfFolder.findIndex(item => item === $("#folders").val()) < 0) {
        alert("Select a folder from the list")
    }
    else {
        const filePath = document.getElementById('lblFilePathMove').textContent;
        const fileName = document.getElementById('lblFileNameMove').textContent;
        const newFilePath = $("#folders").val();
        const result = await CopyDirectoryFile(`${testApi}/movefile`, `${filePath}\\${fileName}`, `${newFilePath}\\${fileName}`);
        $("#dialogCopyFile").dialog("close");
    }
}

async function deleteFile(filePath) {
    const result = await DeleteDirectoryFile(`${testApi}/deletefile/${encodeURIComponent(filePath)}`);
    console.log(result);
}

function upToFolder() {
    // set the current directory to the one the user click
    const index = currentRootDirectoryPath.lastIndexOf("\\");
    if (index !== -1) {
        currentRootDirectoryPath = currentRootDirectoryPath.substring(0, index);
    }

    console.log("root", rootDirectoryPath);
    console.log("cur", currentRootDirectoryPath);
    GetCurrentDirectoryInfo();
    if (currentRootDirectoryPath.length < rootDirectoryPath.length) {
        $(".up-to-folder").show();
    } else {
        $(".up-to-folder").hide();
    }
}

function downToFolder(filePath) {
    // set the current directory to the one the user click
    currentRootDirectoryPath = filePath;
    GetCurrentDirectoryInfo();
    $(".up-to-folder").show();
}
 




