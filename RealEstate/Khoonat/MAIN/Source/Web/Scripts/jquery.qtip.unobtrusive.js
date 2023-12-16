
function getQtipSettings(eventName, jqTarget) {

	var qtipSettings = {
        overwrite: false,
		content: {},
		position: { my: 'top left', at: 'bottom left', viewport: $(window), adjust: { scroll: false } },
		hide: { delay: 100, event: 'unfocus mouseleave', fixed: true },
		show: { event: 'mouseenter' },
		style: { classes: 'qtip-select', tip: { offset: 10, width: 10, height: 10 } },
		events: { render: function (event, api) { $('.qtipClose', api.elements.content).click(api.hide); } }
	};

	if (jqTarget.attr('data-qtip-show-event')) {
		qtipSettings.show.event = jqTarget.attr('data-qtip-show-event');
		if (jqTarget.attr('data-qtip-show-event') == eventName)
			qtipSettings.show.ready = true;
	} else {
		if (eventName == 'mouseenter')
			qtipSettings.show.ready = true;
	}

	if (jqTarget.attr('data-qtip-show-solo'))
		qtipSettings.show.solo = true;
	
	if (jqTarget.attr('data-qtip-show-effect')) {
		var showEffect = jqTarget.attr('data-qtip-show-effect');
		if (showEffect == 'slideDown')
			qtipSettings.show.effect = function(offset) {
				$(this).slideDown(150);
			};
	}

	if (jqTarget.attr('data-qtip-show-modal-on')) {
		qtipSettings.show.modal = { on: true };
		if (jqTarget.attr('data-qtip-show-modal-noblur'))
			qtipSettings.show.modal.blur = false;
		if (jqTarget.attr('data-qtip-show-modal-noescape'))
			qtipSettings.show.modal.escape = false;
	}

	if (jqTarget.attr('data-qtip-hide-event'))
		qtipSettings.hide.event = jqTarget.attr('data-qtip-hide-event');

	if (jqTarget.attr('data-qtip-hide-effect')) {
		var hideEffect = jqTarget.attr('data-qtip-hide-effect');
		if (hideEffect == 'slideUp')
			qtipSettings.hide.effect = function (offset) {
				$(this).slideUp(150);
			};
	}

	if (jqTarget.attr('data-qtip-nohide'))
		qtipSettings.hide = false;

	if (jqTarget.attr('data-qtip-destroy-on-hide'))
	    qtipSettings.events.hide = function(event, api) { api.destroy(); };

	if (jqTarget.attr('data-qtip-style-classes'))
		qtipSettings.style.classes = jqTarget.attr('data-qtip-style-classes');

	if (jqTarget.attr('data-qtip-style-notip'))
		qtipSettings.style.tip = false;

	if (jqTarget.attr('data-qtip-content-selector'))
		qtipSettings.content.text = $(jqTarget.attr('data-qtip-content-selector')).html();

	if (jqTarget.attr('data-qtip-content'))
		qtipSettings.content.text = jqTarget.attr('data-qtip-content');

	if (jqTarget.attr('data-qtip-content-ajax-url')) {

	    var ajaxType = 'GET';
	    if (jqTarget.attr('data-qtip-content-ajax-type'))
	        ajaxType = jqTarget.attr('data-qtip-content-ajax-type');

	    qtipSettings.content.text = function(event, api) {
	        return $.ajax({
	            url: jqTarget.attr('data-qtip-content-ajax-url'),
                type: ajaxType,
	        }).then(function(content) {
	            return content;
	        }, function(xhr, status, error) {
                api.set('content.text', status + ': ' + error);
	        });
	    };
	}

	if (jqTarget.attr('data-qtip-position-my'))
		qtipSettings.position.my = jqTarget.attr('data-qtip-position-my');

	if (jqTarget.attr('data-qtip-position-at'))
		qtipSettings.position.at = jqTarget.attr('data-qtip-position-at');

	if (jqTarget.attr('data-qtip-position-target')) {
		if (jqTarget.attr('data-qtip-position-target') == 'window')
			qtipSettings.position.target = $(window);
		if (jqTarget.attr('data-qtip-position-target') == 'document')
			qtipSettings.position.target = $(document);
	}

	if (jqTarget.attr('data-qtip-position-target-selector'))
		qtipSettings.position.target = $(jqTarget.attr('data-qtip-position-target-selector'));

    if (jqTarget.attr('data-qtip-on-hide'))
        qtipSettings.events.hide = function(event, api) { window[jqTarget.attr('data-qtip-on-hide')](); };

	return qtipSettings;
}

$(document).ready(function () {
    $(document).on('mouseenter', '[data-qtip-enabled]', function (event) {
        var jqTarget = $(this);
        var settings = getQtipSettings('mouseenter', jqTarget);
        jqTarget.qtip(settings, event);
	});
});
