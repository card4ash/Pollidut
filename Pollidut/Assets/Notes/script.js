$(document).ready(function(){
	/* This code is executed after the DOM has been completely loaded */

	var tmp;
	
	$('.note').each(function(){
		/* Finding the biggest z-index value of the notes */
		tmp = $(this).css('z-index');
		if(tmp>zIndex) zIndex = tmp;
	})

	/* A helper function for converting a set of elements to draggables: */
	make_draggable($('.note'));
	
	/* Configuring the fancybox plugin for the "Add a note" button: */
	
	$('#main').on('click', '#addButton', function () {
	    $.fancybox({
	        width: 800,
	        height: 600,
	        autoSize: false,
	        href: '/Dashboard/AddNotes',
	        type: 'ajax'
	    });
	});
	
	///* Listening for keyup events on fields of the "Add a note" form: */
	//$('body').on('keyup', '.pr-body,.pr-author', function (e) {
	//	if(!this.preview)
	//		this.preview=$('#fancy_ajax .note');
		
	//	/* Setting the text of the preview to the contents of the input field, and stripping all the HTML tags: */
	//	this.preview.find($(this).attr('class').replace('pr-','.')).html($(this).val().replace(/<[^>]+>/ig,''));
	//});
	
	/* Changing the color of the preview note: */
	$('html').on('click','.color',function(){
	    $('#previewNote').removeClass('yellow green blue').addClass($(this).attr('class').replace('color', ''));
	});
	
	/* The submit button: */
	$('html').on('click', '#note-submit', function (e) {
		if($('.pr-body').val().length<4)
		{
			alert("The note text is too short!")
			return false;
		}
		if($('.pr-author').val().length<1)
		{
			alert("You haven't entered your name!")
			return false;
		}
		
		//$(this).replaceWith('<img src="img/ajax_load.gif" style="margin:30px auto;display:block" />');
		
		//var data = {
		//	'zindex'	: ++zIndex,
		//	'body'		: $('.pr-body').val(),
		//	'author'		: $('.pr-author').val(),
		//	'color': $.trim($('#previewNote').attr('class').replace('note', ''))
		//};
		

		$.ajax({
		    type: "POST",
		    url: "/Dashboard/PostComplain",
		    data: JSON.stringify({ zindex: ++zIndex, body: $('.pr-body').val(), author: $('.pr-author').val(), color: $.trim($('#previewNote').attr('class').replace('note', '')) }),
		    contentType: "application/json; charset=utf-8",
		    dataType: "json",
		    success: function (data) {
		        if (data.status == "success") {
		            /* msg contains the ID of the note, assigned by MySQL's auto increment: */
		            var tmp = $('.fancybox-inner .note').clone();
		            alert(data.ID);
		            tmp.find('span.data').text(data.ID).end().css({ 'z-index': zIndex, top: 0, left: 0 });
		            tmp.appendTo($('#main'));
		            make_draggable(tmp);
		        }
		        $.fancybox.close();
		    },
		    error: function (msg) {
		        alert(msg.responseText);
		    }
		});
		e.preventDefault();
	})
	
	$('html').on('submit','.note-form',function(e){e.preventDefault();});
});

var zIndex = 0;

function make_draggable(elements)
{
	/* Elements is a jquery object: */
	
	elements.draggable({
		containment:'parent',
		start:function(e,ui){ ui.helper.css('z-index',++zIndex); },
		stop:function(e,ui){
			
			/* Sending the z-index and positon of the note to update_position.php via AJAX GET: */
			$.ajax({
			    type: "POST",
			    url: "/Dashboard/UpdatePosition",
			    data: JSON.stringify({ Id: parseInt(ui.helper.find('span.data').html()),x: ui.position.left, y: ui.position.top, z: zIndex }),
			    contentType: "application/json; charset=utf-8",
			    dataType: "json",
			    success: function (data) {
			        if (data.status == "success") {
			        }
			    },
			    error: function (msg) {
			        alert(msg.responseText);
			    }
			});
		}
	});
}