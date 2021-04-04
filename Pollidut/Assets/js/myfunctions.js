

//Main Form Submit Handler
function SubmitMainForm(MainForm) {

    var SubmitURL = MainForm.attr('action');
    var FormData = MainForm.serialize();
    $.ajax({
        type: "POST",
        url: SubmitURL,
        async: true,
        //If datatype is of 'FormCollection', following 3 lines are not needed-
        //====================================================
        //contentType: "application/json; charset=utf-8",
        //data: "{'agentdata':'"+ agentdata +"'}",
        //dataType: 'json',
        //====================================================
        data: FormData,
        beforeSend: function () {
            $(MainForm).find('.btn').attr('disabled', 'disabled');
            $(MainForm).find('#btn-submit-mainform').html('Working...');
        },
        success: function (result) {
            if (result == 'Success') {
                SuccessfullSaveNotification();
                ResetMainForm(MainForm);
            }
            else {
                ShowRegularError(result);
                //alert("Failed to save. " + result);
            }
        },
        error: function (request, status, error) {
            ShowRegularError("Failed to save. Error Details -  Request: " + request + ", Status: " + status + ", Error: " + error);
            //alert("Failed to save. Error Details -  Request: " + request + ", Status: " + status + ", Error: " + error);
        },
        complete: function () {
            $(MainForm).find('.btn').removeAttr("disabled");
            $(MainForm).find('#btn-submit-mainform').html('Save changes');

        }
    });
    return false;
}

//Notify user in async way and in a auto-close non-modal dialog
function SuccessfullSaveNotification() {
    $.pnotify({
        type: 'success',
        title: 'Success',
        text: 'Data successfully saved to database!',
        icon: 'picon icon16 iconic-icon-check-alt white',
        opacity: 0.95,
        history: false,
        sticker: false
    });
}

//Notify user in async way and in a auto-close non-modal dialog
function SuccessNotification(UserMessage) {
    $.pnotify({
        type: 'success',
        title: 'Success',
        text: UserMessage,
        icon: 'picon icon16 iconic-icon-check-alt white',
        opacity: 0.95,
        history: false,
        sticker: false
    });
}

//for general purpose alert
function AlertNotification(Title, UserMessage) {
    $.pnotify({
        type: 'error',
        title: Title,
        text: UserMessage,
        icon: 'picon icon24 typ-icon-cancel white',
        opacity: 0.95,
        history: false,
        sticker: false
    });
}

//error
function ErrorNotification(UserMessage) {
    $.pnotify({
        type: 'error',
        title: 'Error !',
        text: UserMessage,
        icon: 'picon icon24 typ-icon-cancel white',
        opacity: 0.95,
        history: false,
        sticker: false
    });
}

//Regular error
function ShowRegularError(error) {
    $.pnotify({
        type: 'error',
        title: 'Oh No!',
        text: error,
        icon: 'picon icon24 typ-icon-cancel white',
        opacity: 0.95,
        history: false,
        sticker: false
    });
}

//Sticky Error
function ShowStickyError(error) {
    $.pnotify({
        type: 'error',
        title: 'Oh No!',
        text: error,
        icon: 'picon icon24 typ-icon-cancel white',
        opacity: 0.95,
        hide: false,
        history: false,
        sticker: false
    });
}

//Reset main form
function ResetMainForm(MainForm) {
    MainForm.trigger('reset');
    var validator = MainForm.validate();
    validator.resetForm();

    $('select.resetIt', MainForm).each(function () {
        $(this).select2("val","");
    });
    
    $('select.emptyIt', MainForm).empty();

    $(MainForm).find('input:visible:first').focus();
}


//$('a.handle-link').click(function (e) {
//    e.preventDefault();

//    var currentLink = $(this).attr('href');
//    $.ajax({
//        url: currentLink,
//        async: true,
//        beforeSend: function () {
//            $('div.sidenav').block({ message: '<img src="../../Assets/images/loaders/circular/001.gif" />' });
//            $('.panel-body div').removeClass('scroll');
//            $("#PartialContent").fadeOut("slow");
//            $("#PartialContent").empty();

//        },
//        success: function (data) {
//            $('#PartialContent').html(data);
//            $("#PartialContent").fadeIn("slow");
//        },
//        error: function (request, status, error) {
//            alert("Error-  Request: " + request + ", Status: " + status + ", Error: " + error);
//        },
//        complete: function () {
//            $('div.sidenav').unblock();
//        }
//    });
//});


//Main form reset button click handler
$(function () {
    $(document).on('click', '#btn-reset-mainform', function (e) {
        e.preventDefault();
        var form = $(this).parents('form');
        ResetMainForm(form);
    });
})

