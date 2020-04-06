var filenameToDeleteUrl = {}
var filenameToUploadedBytes = {}
var filenameToProgress = {}
//set value to true, if paused intentionally
var filenameToIsPaused = {}

window.InitUploader = () => {
    $('#fileupload').fileupload({
        //options
        dataType: 'json',
        paramName: 'files',
        sequentialUploads: false,
        autoUpload: false,

        //callbacks
        add: async function (e, data) {
            filename = data.files[0].name;
            filenameToUploadedBytes[filename] = filenameToProgress[filename] = 0;
            var jqXHR = null;
            data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');
            data.context.append($('<p/>').addClass('upload_file_name').text(filename));
            data.context.append($('<button/>').addClass('upload_button_individual').text('Upload')
                .click(async function () {
                    //if this is removed, an empty page will be shown after data.submit()
                    data.context.append($('<p/>').replaceAll($(this)));
                    jqXHR = data.submit();

                    //this needs to be updated here. otherwise, this will be the name of the last added file for all buttons
                    filename = data.files[0].name;
                    addCancelButton(jqXHR, data, filename);
                    //addPauseButton(jqXHR, data, filename);
                })
            );

            addIndividualProgressBar(data, 0);
       
            $("#button_upload_all").on("click", function () {
                data.context.find(".upload_button_individual").click();
            })
            $("#button_cancel_all").on("click", function () {
                data.context.find(".cancel_button_individual").click();
            })
        },

        //progressall: function (e, data) {
        //    var progress = parseInt(data.loaded / data.total * 100, 10);
        //    $('#progress_global').attr('value', progress);
        //},

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
            addDeleteButton(data, filename);
            addSuccessFlag(data, filename);
            addXButton(data, filename);


            //TODO make post request to server, that file has been completely uploaded and can be processed

        },
        fail: async function (e, data) {
            if (filenameToIsPaused[data.files[0].name]) {
                var jqXHR = null;
                data.context.append($('<button/>').addClass('resume_button_individual').text('resume upload')
                    .click(async function () {
                        data.context.append($('<p/>').replaceAll($(this)));
                        filename = data.files[0].name;
                        data.uploadedBytes = filenameToUploadedBytes[filename];
                        jqXHR = data.submit();
                        filenameToIsPaused[data.files[0].name] = false;
                        filename = data.files[0].name;
                        addPauseButton(jqXHR, data, filename);                       
                    }))
            }

            //TODO: handle other reasons that could cause an interruption/abortion of the submission, this should mainly be a network error
        }
    });
}



function addIndividualProgressBar(data, value) {
    data.context.append($('<progress/>').attr('value', value).attr('max', 100).addClass('individual_progress_bar'));
}


function addPauseButton(jqXHR, data, filename) {
    data.context.append($('<button/>').addClass('pause_button_individual').text('Pause upload')
        .click(async function () {
            filenameToIsPaused[data.files[0].name] = true;

            //jqXHR.abort() triggers the "fail" callback
            jqXHR.abort();
            //this prevents abort() from removing ui elements
            data.context.append($('<p/>').replaceAll($(this)));                                       
        }))
}

async function addCancelButton(jqXHR, data, filename) {
    data.context.append($('<button/>').addClass('cancel_button_individual').text('Cancel').prop('disabled', false).click(async function () {
        console.log(filenameToIsPaused[filename])
        if (!filenameToIsPaused[filename]) {
            jqXHR.abort();
        }
        result = await ajaxDeleteRequest(filename)
        console.log('ajaxDeleteRequest: ' + result)
        if (result) {
            console.log('successfully canceled ' + filename)
        }
        else {
            console.log('something went wrong, could not cancel: ' + filename);
            retryAjaxDeleteRequest(filename, 'cancel');
        }
    }));
}

async function addDeleteButton(data, filename) {
    data.context.find('.cancel_button_individual').unbind().removeClass('cancel_button_individual').text('Delete from server').addClass('delete_button_individual').click(async function () {
        data.context.remove()
        try {
            result = await ajaxDeleteRequest(filename)
            if (result) {
                console.log('successfully deleted ' + filename)
            }
            else {
                console.log('something went wrong, could not delete' + filename);
                retryAjaxDeleteRequest(filename, 'delete');
            }
        }
        catch (e) {
            console.log('Exception in addDeleteButton' + e);
        }
    }).prop('disabled', false);
}

async function addSuccessFlag(data, filename) {
    data.context.append($('<span/>').addClass('successful_upload_flag').text('upload complete'))
}

async function addXButton(data, filename) {
    data.context.append($('<button/>').addClass('x_button_individual').text('X').click(function () {
        data.context.remove();
    }))
}

async function ajaxDeleteRequest(filename) {
    if (filenameToUploadedBytes[filename] > 0) {
        result = await $.ajax({
            url: filenameToDeleteUrl[filename],
            type: 'GET',
            dataType: "json"
        });
        return result['success'];
    }
    else {
        return true;
    }
}

async function retryAjaxDeleteRequest(filename, operationName) {
    for (i = 1; i < 50; i++) {
        ms = i * 100;
        console.log('retry ' + operationName + ': ' + filename + ' in ' + ms + 'ms');
        await timeout(ms);
        result = await ajaxDeleteRequest(filename);
        if (result) {
            console.log('successfully ' + operationName + ' ' + filename)
            break;
        }
    }
}

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}





                    // TODO: add stop and resume functionality see https://github.com/blueimp/jQuery-File-Upload/issues/3200
                    // https://github.com/blueimp/jQuery-File-Upload/wiki/Chunked-file-uploads#resuming-file-uploads
                    // https://stackoverflow.com/questions/40698560/resuming-chunked-file-upload-with-blueimp-jquery-file-upload-uploads-all-at-once

                    //data.context.append($('<button/>').addClass('stop_button_individual').text('stop').prop('disabled', false).click(async function () {
                    //    ub = data.uploadedBytes
                    //    console.log(jqXHR.uploadedBytes)
                    //    //console.log(data.context[0])
                    //    jqXHR.abort();
                    //    data.submit();
                    //    //console.log(data.context[0].children)
                    //    //$('#UploadList').append(data.context[0].children[0])
                    //    console.log(jqXHR.uploadedBytess)

                    //    //data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');
                    //    //data.context.append('<p/>').addClass('upload_file_name').text(data.files[0].name);
                    //    //data.context.append($('<button/>').addClass('resume_button_individual').text('Resume')
                    //    //    .click(async function () {
                    //    //        data.context.append($('<p/>').replaceAll($(this)));
                    //    //        jqXHR = data.submit();
                    //    //    }));

                    //    ////console.log((data.uploadedBytes / data.total) * 100)
                    //    //console.log(data.uploadedBytes)
                    //    //data.context.append($('<progress/>').attr('value', (data.uploadedBytes / data.total) * 100).attr('max', '100').addClass('individual_progress_bar'));

                    //}));



                            //var children = data.context[0].children;
                            //console.log(children);
                            ////data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');
                            //var element_wrapper = document.createElement('div');
                            //element_wrapper.className = 'upload_file_wrapper';
                            //for (i = 0; i < children.length; i++) {
                            //    //console.log(children[i]);
                            //    //console.log(typeof (children[i]))
                            //    element_wrapper.appendChild(children[i]);

                            //}

                            //document.getElementById('UploadList').appendChild(element_wrapper);
                            //console.log(element_wrapper);