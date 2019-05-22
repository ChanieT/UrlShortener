$(() => {
    $("#url-btn").on('click', function () {
        const completeUrl = $("#completeUrl").val();
        const userId = $(this).data('url-id');
        $.post('/home/addUrl', { completeUrl, userId }, function (urlHash) {
            $('#shortUrl').val(urlHash);
            $('#shortUrl').prop('disabled', false);
        });
        
    });
});