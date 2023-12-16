
(function () {

	//
	// Helper functions

	function setupSelec2(ctx) {
	    $('select', ctx)
	        .select2({ minimumResultsForSearch: -1 })
	        .on('select2-opening', function() {
	            $('input').blur();
	        })
	        .on('select2-open', function() {
	            $('input').blur();
	        })
	        .on('select2-focus', function() {
	            $('input').blur();
	        })
	    ;
//	    $(".select2-search, .select2-focusser").remove();
	}

	//
	// Init functions

	khoonat.common.page.beforeInit = function () {

	};

	khoonat.common.page.doInit = function () {
		setupSelec2(document);
	};

	khoonat.common.page.afterInit = function () {
		jj.tabs.setup();
		jj.modal.setup();
		$('[data-toggle="tooltip"]').tooltip({
			'placement': function(tt, trigger) {
				var $trigger = $(trigger);
				var windowWidth = $(window).width();

				var xs = $trigger.attr('data-placement-xs');
				var sm = $trigger.attr('data-placement-sm');
				var md = $trigger.attr('data-placement-md');
				var lg = $trigger.attr('data-placement-lg');
				var general = $trigger.attr('data-placement');

				return (windowWidth >= 1200 ? lg : undefined) ||
					(windowWidth >= 992 ? md : undefined) ||
					(windowWidth >= 768 ? sm : undefined) ||
					xs ||
					general ||
					"top";
			}
		});

		$(document).on('mousedown', '[data-link-url]', function (e) {
			var target = $(e.target);
			if (target.attr('data-link-ignore') || target.parents('[data-link-ignore]').length > 0)
				return true;

			return false;
		});

		$(document).on('mouseup', '[data-link-url]', function (e) {
			var target = $(e.target);
			if (target.attr('data-link-ignore') || target.parents('[data-link-ignore]').length > 0)
				return true;

			if (e.which == 1) {
				location.href = $(this).attr('data-link-url');
			} else if (e.which == 2) {
				window.open($(this).attr('data-link-url'), '_blank');
				// ISSUE: In IE, the new tab is focused (instead of remaining on the current tab)
				// ISSUE: In Firefox, the popup blocker prevents openning a new tab
			}

			return false;
		});

		$(document).on('click', '.dropdown-keepopen', function(e) {
			$(this).closest('.dropdown').addClass('dont-close');
		});

		$(document).on('hide.bs.dropdown', function (e) {
			var target = $(e.target);

			if (target.hasClass('dont-close')) {
				e.preventDefault();
				target.removeClass('dont-close');
				return false;
			}

			return true;
		});
	};

	khoonat.common.partial.doInit = function(partialRoot) {
		setupSelec2(partialRoot);
	};

	khoonat.common.partial.afterInit = function(partialRoot) {
		$.validator.unobtrusive.parse(partialRoot);
	    $('[data-toggle="tooltip"]', partialRoot).tooltip();
	};

})();

