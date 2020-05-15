var filenameToDeleteUrl = {}
var filenameToUploadedBytes = {}
var filenameToProgress = {}
//set value to true, if paused intentionally (pause with data.abort())
var filenameToIsPaused = {}
//api url that shall be invoked on the server, when upload is complete and file can be further processed on the server
//var filenameToDoneUrl = {}



//addGuid:bool if this is true, the uploader will add a guid at the beginning of the filename and separate it with guidSeparatorarator from the rest of the original filename
window.InitUploader = (addGuid, guidSeparator) => {
    $('#fileupload').fileupload({
        //options
        dataType: 'json',
        paramName: 'files',
        sequentialUploads: false,
        autoUpload: false,

        //callbacks
        add: async function (e, data) {
            //https://stackoverflow.com/a/55619755
            var newname = addGuid ? uuidv4() + guidSeparator + data.files[0].name : data.files[0].name;
            Object.defineProperty(data.files[0], 'name', {
                value: newname
            });
            $.blueimp.fileupload.prototype.options.add.call(this, e, data);

            filename = data.files[0].name;
            console.log("filename: " + filename)
            filenameToUploadedBytes[filename] = filenameToProgress[filename] = 0;
            filenameToIsPaused[filename] = false;
            data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');

            console.log(removeGuidFromFilename(filename, guidSeparator));

            if (addGuid) {
                data.context.append($('<p/>').addClass('upload_file_name').text(removeGuidFromFilename(filename, guidSeparator)));
            }
            else {
                data.context.append($('<p/>').addClass('upload_file_name').text(filename));
            }

            addUploadButton(data);
            addIndividualProgressBar(data, 0);
            addXButton(data);
        },

        progress: function (e, data) {
            filename = data.files[0].name;
            //var progress = parseInt((data.loaded / data.total) * 100, 10);
            var progress = (data.uploadedBytes / data.total) * 100
            data.context.find("progress").attr('value', progress);
            filenameToProgress[filename] = progress;
        },

        chunkdone: async function (e, data) {
            filename = data.result[0]['name'];
            filenameToDeleteUrl[filename] = data.result[0]['deleteUrl']
            filenameToUploadedBytes[filename] += data.result[0]['size']
        },

        done: async function (e, data) {
            filename = data.result[0]['name'];
            //if the filesize is smaller than the chunksize no chunkdone event is triggered
            if (filenameToUploadedBytes[filename] == 0) {
                filenameToDeleteUrl[filename] = data.result[0]['deleteUrl']
                filenameToUploadedBytes[filename] = data.result[0]['size']
            }
            data.context.find("progress").attr('value', 100);
            data.context.find('.pause_button_individual').remove();
            data.context.find(".cancel_button_individual").removeClass().addClass("delete_button_individual").text('delete from server');
            addSuccessFlag(data, filename);
            addXButton(data, filename);

            $("#button_delete_all").on("click", function () {
                data.context.find(".delete_button_individual").click();
            })

            console.log(data.result[0]['doneUrl']);
            ajaxDoneRequest(data.result[0]['doneUrl']);
        },
        fail: async function (e, data) {

            //only invoke this, if pausing was intentional
            if (filenameToIsPaused[data.files[0].name]) {
                addResumeButton(data);
            }

            //TODO: handle other reasons that could cause an interruption/abortion of the submission, this should mainly be a network error
        }
    });
}






function addUploadButton(data) {
    data.context.append($('<button/>').addClass('upload_button_individual').text('Upload')
        .click(async function () {
            data.context.find('.x_button_individual').remove();
            //if this is removed, an empty page will be shown after data.submit()
            data.context.append($('<p/>').replaceAll($(this)));
            data.submit();
            //this needs to be updated here. otherwise, this will be the name of the last added file for all buttons
            //filename = data.files[0].name;
            addCancelOrDeleteButton(data, buttonType.cancel);
            addPauseButton(data);
        })
    );

    $("#button_upload_all").on("click", function () {
        data.context.find(".upload_button_individual").click();
    })
}

function addIndividualProgressBar(data, value) {
    data.context.append($('<progress/>').attr('value', value).attr('max', 100).addClass('individual_progress_bar'));
}

function addPauseButton(data) {
    data.context.append($('<button/>').addClass('pause_button_individual').text('Pause upload')
        .click(async function () {
            filenameToIsPaused[data.files[0].name] = true;
            //abort() triggers the "fail" callback
            data.abort();
            //this prevents abort() from removing ui elements
            data.context.append($('<p/>').replaceAll($(this)));
        }))

    $("#button_pause_all").on("click", function () {
        data.context.find(".pause_button_individual").click();
    })
}

function addResumeButton(data) {
    data.context.append($('<button/>').addClass('resume_button_individual').text('resume upload')
        .click(async function () {
            data.context.append($('<p/>').replaceAll($(this)));
            filename = data.files[0].name;
            data.uploadedBytes = filenameToUploadedBytes[filename];
            data.submit();
            filenameToIsPaused[data.files[0].name] = false;
            filename = data.files[0].name;
            addPauseButton(data);
        }))
    $("#button_resume_all").on("click", function () {
        data.context.find(".resume_button_individual").click();
    })
}


const buttonType = {
    cancel: 1,
    delete: 2
};

//cancel and delete do almost the same thing. only difference is, cancel should call data.abort(), if submission is not paused.
async function addCancelOrDeleteButton(data, buttonType) {
    cssClassName = null;
    buttonText = null;
    if (buttonType == 1) {
        cssClassName = 'cancel_button_individual';
        buttonText = 'Cancel';

        $("#button_cancel_all").on("click", function () {
            data.context.find(".cancel_button_individual").click();
        })
    }
    else if (buttonType == 2) {
        cssClassName = 'delete_button_individual';
        buttonText = 'Delete from server';
    }
    else {
        console.log('addCancelOrDeleteButton: no sufficient buttonType: ' + buttonType);
    }

    data.context.append($('<button/>').addClass(cssClassName).text(buttonText).click(async function () {
        data.context.remove();
        filename = data.files[0].name;
        try {
            if (!filenameToIsPaused[filename]) {
                data.abort();
            }
            result = await ajaxDeleteRequest(filename)
            if (result) {
                console.log('successfully deleted from the server ' + filename)
            }
            else if (!result) {
                console.log('something went wrong, could not delete from the server: ' + filename);
                retryAjaxDeleteRequest(filename);
            }
            else {
                console.log('ajaxDeleteRequest: no response from server')
            }
        }

        catch (e) {
            console.log('Exception: ' + e);
            console.log('filename ' + filename);
        }
    }));
}

async function addSuccessFlag(data) {
    data.context.append($('<span/>').addClass('successful_upload_flag').text('upload complete'))
}

async function addXButton(data) {
    data.context.append($('<button/>').addClass('x_button_individual').text('X').click(function () {
        data.context.remove();
    }))

    $("#x_all").on("click", function () {
        data.context.find(".x_button_individual").click();
    })
}


//Let the server know, when the upload is complete and the file can be further processed.
async function ajaxDoneRequest(url) {
    try {
        console.log('done request')
        console.log(url)

        result = await $.ajax({
            url: url,
            type: 'GET',
            dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.status)
                console.log(textStatus);
                console.log(errorThrown);
                //ajaxDoneRequest(url);
            }
        });

        console.log(result);
        return result['success'];
    }
    catch (e) {
        console.error(e, e.stack)
    }

}

async function ajaxDeleteRequest(filename) {

    console.log("delete request to: ")
    console.log(filenameToDeleteUrl[filename]);
    console.log(filenameToUploadedBytes[filename])

    if (filenameToUploadedBytes[filename] > 0) {
        result = await $.ajax({
            url: filenameToDeleteUrl[filename],
            type: 'GET',
            dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.status)
                console.log(textStatus);
                console.log(errorThrown);
            }
        });
        return result['success'];
    }
    else {
        return true;
    }
}

async function retryAjaxDeleteRequest(filename) {
    for (i = 1; i < 50; i++) {
        ms = i * 100;
        console.log('retry deletion' + ': ' + filename + ' in ' + ms + 'ms');
        await timeout(ms);
        result = await ajaxDeleteRequest(filename);
        if (result) {
            console.log('successfully deleted ' + filename)
            break;
        }
    }
}

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}


//https://stackoverflow.com/a/2117523
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

function removeGuidFromFilename(filename, guidSeparator) {
    return filename.split(guidSeparator)[1];
}

function getGuidFromFilename(filename, guidSeparator) {
    return filename.split(guidSeparator)[0];
}