
//
// Setup namespaces

jj = {
	init: {}
};

khoonat = {
	common: {
		page: {},
		partial: {},
	},
	page: {},
	partial: {},
};

//
// Populate init functions

jj.init.exec = function(ns, id, fn, param) {
	if (id && ns[id] && ns[id][fn] && typeof ns[id][fn] == 'function') {
		ns[id][fn](param);
	}
};

jj.init.initializePage = function () {
	var actionId = $("body").attr('data-action-id');

	jj.init.exec(khoonat.common, 'page', 'beforeInit');
	jj.init.exec(khoonat.page, actionId, 'beforeInit');
	jj.init.exec(khoonat.common, 'page', 'doInit');
	jj.init.exec(khoonat.page, actionId, 'doInit');
	jj.init.exec(khoonat.common, 'page', 'afterInit');
	jj.init.exec(khoonat.page, actionId, 'afterInit');
};

jj.init.initializePartial = function (partialRoot) {
	var partialIdRoots = $("[data-partial-id]", partialRoot);

	jj.init.exec(khoonat.common, 'partial', 'beforeInit', partialRoot);
	partialIdRoots.each(function() {
		jj.init.exec(khoonat.partial, $(this).attr('data-partial-id'), 'beforeInit', partialRoot);
	});

	jj.init.exec(khoonat.common, 'partial', 'doInit', partialRoot);
	partialIdRoots.each(function () {
		jj.init.exec(khoonat.partial, $(this).attr('data-partial-id'), 'doInit', partialRoot);
	});

	jj.init.exec(khoonat.common, 'partial', 'afterInit', partialRoot);
	partialIdRoots.each(function () {
		jj.init.exec(khoonat.partial, $(this).attr('data-partial-id'), 'afterInit', partialRoot);
	});
};

//
// Run page initialization on jQuery's ready event

$(function() {
	jj.init.initializePage();
});
