$('a.handle-link').click(function (e) {
    e.preventDefault();

    var currentLink = $(this).attr('href');
    $.ajax({
        url: currentLink,
        async: true,
        beforeSend: function () {
            $('div.sidenav').block({ message: '<img src="../../Assets/images/loaders/circular/001.gif" />' });
            $('.panel-body div').removeClass('scroll');
            $("#PartialContent").fadeOut("slow").empty();

        },
        success: function (data) {
            $('#PartialContent').hide().html(data).fadeIn("slow");
        },
        error: function (request, status, error) {
            alert("Error-  Request: " + request + ", Status: " + status + ", Error: " + error);
        },
        complete: function () {
            $('div.sidenav').unblock();
        }
    });
});