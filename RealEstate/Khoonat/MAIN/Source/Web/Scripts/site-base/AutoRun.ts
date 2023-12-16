/// <reference path="../typings/jquery/jquery.d.ts" />

module AutoRun {

    export enum Scope { Page, Partial }

    export interface IAutoRunItem {
        onBeforeInit(root: JQuery) : void;
        onDoInit(root: JQuery) : void;
        onAfterInit(root: JQuery) : void;
    }

    var rootPageItem: IAutoRunItem = null;
    var rootPartialItem: IAutoRunItem = null;
    var pageItems: Array<IAutoRunItem> = [];
    var partialItems: Array<IAutoRunItem> = [];

    export function register(scope: Scope, id: string, item: IAutoRunItem) {
        if (id) {
            if (scope === Scope.Page)
                pageItems[id] = item;
            else {
                partialItems[id] = item;
            }
        } else {
            if (scope === Scope.Page)
                rootPageItem = item;
            else
                rootPartialItem = item;
        }
    }

    function initializePage() {
        var actionId = $("body").attr('data-action-id');
        var item = pageItems[actionId];
        var root = $(document);

        if (rootPageItem)
            rootPageItem.onBeforeInit(root);
        if (item)
            item.onBeforeInit(root);
        if (rootPageItem)
            rootPageItem.onDoInit(root);
        if (item)
            item.onDoInit(root);
        if (rootPageItem)
            rootPageItem.onAfterInit(root);
        if (item)
            item.onAfterInit(root);
    }

    export function initializePartial(partialRoot) {
        var partialIdRoots = $("[data-partial-id]", partialRoot);

        if (rootPartialItem)
            rootPartialItem.onBeforeInit(partialRoot);

        partialIdRoots.each((i, e) => {
            var item = partialItems[$(this).attr('data-partial-id')];
            if (item)
                item.onBeforeInit($(e));
        });

        if (rootPartialItem)
            rootPartialItem.onDoInit(partialRoot);

        partialIdRoots.each((i, e) => {
            var item = partialItems[$(this).attr('data-partial-id')];
            if (item)
                item.onDoInit($(e));
        });

        if (rootPartialItem)
            rootPartialItem.onAfterInit(partialRoot);

        partialIdRoots.each((i, e) => {
            var item = partialItems[$(this).attr('data-partial-id')];
            if (item)
                item.onAfterInit($(e));
        });
    }

    //
    // Run page initialization on jQuery's ready event

    $(() => {
        initializePage();
    });
}


