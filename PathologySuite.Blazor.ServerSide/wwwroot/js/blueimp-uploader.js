window.InitUploader = () => {
    $('#fileupload').fileupload({
        //options
        dataType: 'json',
        paramName: 'files',
        sequentialUploads: false,
        autoUpload: false,

        //callbacks
        add: function (e, data) {
            var jqXHR = null;
            data.context = $('<div/>').addClass('upload_file_wrapper').appendTo('#UploadList');
            data.context.append('<p/>').addClass('upload_file_name').text(data.files[0].name);
            data.context.append($('<button/>').addClass('upload_button_individual').text('Upload')
                .click(function () {
                    data.context.append($('<p/>').replaceAll($(this)));
                    jqXHR = data.submit();
                    data.context.append($('<button/>').addClass('cancel_button_individual').text('Cancel').click(function () {
                        jqXHR.abort();
                        //TODO make post request to delete partly sent file
                    }));
                })
            );

            data.context.append($('<progress/>').attr('value', '0').attr('max', '100').addClass('individual_progress_bar'));

            $("#button_upload_all").on("click", function () {
                $(".upload_button_individual").click(); //click all upload buttons
            })
            $("#button_cancel_all").on("click", function () {
                $(".cancel_button_individual").click(); //click all cancel buttons
            })
        },

        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress_global').attr(
                'value',
                progress + '%'
            );
        },
        progress: function (e, data) {
            var progress = parseInt((data.loaded / data.total) * 100, 10);
            console.log(progress)
            data.context.find("progress").attr('value', progress + '%');
            //console.log(data.context);
        },

        done: function (e, data) {
            console.log(data.files[0].name)
            data.context
                .addClass("done")
                //.find("a")
                //.prop("href", data.result.url)
                ;

            //TODO make post request to server, that file has been completely uploaded and can be processed

        },
    });
}