window.InitUploader = () => {
    $('#fileupload').fileupload({
        //options
        dataType: 'json',
        paramName: 'files',
        sequentialUploads: false,
        autoUpload: false,

        //callbacks
        add: async function (e, data) {
            addFile(data, 0)
        },

        //progressall: function (e, data) {
        //    var progress = parseInt(data.loaded / data.total * 100, 10);
        //    $('#progress_global').attr('value', progress);
        //},

        progress: function (e, data) {
            //var progress = parseInt((data.loaded / data.total) * 100, 10);
            var progress = (data.uploadedBytes / data.total) * 100
            data.context.find("progress").attr('value', progress);

            
          
        },

        chunkdone: async function (e, data) {
            data.context.find('.cancel_button_individual').click(async function () {
                data.context.append($('<p/>').text('canceling').replaceAll($(this)));
                result = await ajaxDeleteRequest(data)
                if (result['success']) {
                    data.context.find('p').remove();
                    data.context.append($('<p/>').text('successfully canceled'));
                }
                else {
                    data.context.find('p').remove();
                    data.context.append($('<p/>').text('something went wrong, could not cancel'));
                    addDeleteButton(data);
                }
            }).prop('disabled', false);

            $("#button_cancel_all").on("click", function () {
                data.context.find(".cancel_button_individual").click();
            })
        },

        chunkfail: async function (e, data) {
            
        },

        done: async function (e, data) {
            data.context.find("progress").attr('value', 100);
            addDeleteButton(data);


            //TODO make post request to server, that file has been completely uploaded and can be processed

        },
    });
}


async function addFile(data, progressValue) {
    var jqXHR = null;
    data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');
    console.log(data.context[0])
    data.context.append('<p/>').addClass('upload_file_name').text(data.files[0].name);
    data.context.append($('<button/>').addClass('upload_button_individual').text('Upload')
        .click(async function () {
            data.context.append($('<p/>').replaceAll($(this)));
            jqXHR = data.submit();
            data.context.append($('<button/>').addClass('cancel_button_individual').text('Cancel').prop('disabled', true).click(async function () {
                jqXHR.abort();
            }));

            //data.context.append($('<button/>').addClass('stop_button_individual').text('stop').prop('disabled', false).click(async function () {
            //    jqXHR.abort();
            //    //console.log(data.context[0])
            //    d = data.context[0]
            //    console.log(d)

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
        })
    );

    data.context.append($('<progress/>').attr('value', progressValue).attr('max', '100').addClass('individual_progress_bar'));

    $("#button_upload_all").on("click", function () {
        $(".upload_button_individual").click(); //click all upload buttons
    })
}


async function ajaxDeleteRequest(data) {
    return await $.ajax({
        url: data.result[0]['deleteUrl'],
        type: 'GET',
        dataType: "json"
    });
}

async function addDeleteButton(data) {
    data.context.find('.cancel_button_individual').removeClass('cancel_button_individual').text('Delete').addClass('delete_button_individual').unbind().click(async function () {
        data.context.append($('<p/>').text('deleting...').replaceAll($(this)))
        result = await ajaxDeleteRequest(data)
        if (result['success']) {
            data.context.find('p').remove();
            data.context.append($('<p/>').text('successfully deleted'));
        }
        else {
            data.context.find('p').remove();
            data.context.append($('<p/>').text('something went wrong, could not delete or already deleted'));
        }
    }).prop('disabled', false);
}