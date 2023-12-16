

(function () {
	jj.tabs = {
		setup: function () {
			$(document).on('show.bs.tab', '[data-ajax-content-url]', function (e) {

				var trigger = $(e.target);
				var contentWrapperSelector = trigger.attr('href');
				var contentWrapper = $(contentWrapperSelector);

				contentWrapper.html($('#ajaxTabDefaultContent').html());
				$.ajax({
					url: $(this).attr('data-ajax-content-url'),
					type: 'POST'
				}).done(function (data) {
					contentWrapper.html(data);
					jj.init.initializePartial(contentWrapper);
					// TODO: The following line doesn't work
					// scrollToTabContents(contentWrapper, trigger);
				});
			});
		}
	};

	function scrollToTabContents(contentWrapper, trigger) {
		var currentScrollValue = $(window).scrollTop();
		var topOfView = currentScrollValue;
		var bottomOfView = topOfView + $(window).height();
		var topOfContent = contentWrapper.offset().top + currentScrollValue;
		var bottomOfContent = topOfContent + contentWrapper.height();

		if (bottomOfContent > bottomOfView || topOfView > topOfContent) {
			var targetScrollTop = trigger.offset().top + currentScrollValue;
			$("html, body").animate({ scrollTop: targetScrollTop - 10 }, 500);
		}
	}

})();
