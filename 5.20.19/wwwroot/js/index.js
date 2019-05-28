$(() => {
    $("#url-btn").on('click', function () {
        const completeUrl = $("#completeUrl").val();
        const userId = $(this).data('url-id');
        $.post('/home/addUrl', { completeUrl, userId }, function (urlHash) {
            //$('#shortUrl').html(`<a href='${urlHash.shortUrl}'>${urlHash.shortUrl}</a>`);
            //$('#shortUrl').slideDown();
            $('#shortUrl').val(urlHash);
            $('#shortUrl').prop('disabled', false);
        });
        
    });
});