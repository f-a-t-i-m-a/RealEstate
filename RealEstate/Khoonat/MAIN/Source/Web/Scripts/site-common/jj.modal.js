
(function() {
	jj.modal = {
		setup: function() {
			$(document).on('click', '[data-ajax-modal]', function() {
				$('#ajaxModalContainer .modal-dialog').html($('#ajaxModalDefaultContent').html());
				$.ajax({
					url: $(this).attr('data-ajax-modal'),
					type: 'POST'
				}).done(function (data) {
					var modalDialog = $('#ajaxModalContainer .modal-dialog');
					modalDialog.html(data);
					jj.init.initializePartial(modalDialog);
				});
				$('#ajaxModalContainer').modal();
			});
		},
		adjust: function() {
			$('#ajaxModalContainer').modal('adjustBackdrop');
		}
	};
})();
