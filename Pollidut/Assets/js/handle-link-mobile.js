
$('html').on('click', '.handle-link', function (e) { 
    e.preventDefault();

    var currentLink = $(this).attr('href');
    $.ajax({
        url: currentLink,
        async: true,
        beforeSend: function () {
            $("#PartialContent").fadeOut("slow").empty();
        },
        success: function (data) {
            $('#PartialContent').hide().html(data).fadeIn("slow");
           
        },
        error: function (request, status, error) {
            alert("Error-  Request: " + request + ", Status: " + status + ", Error: " + error);
        },
        complete: function () {
           
        }
    });
});