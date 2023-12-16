
interface JQueryCaretRange {
    start: number;
    end: number;
    length: number;
    text: string;
}

interface JQuery {
    caret(): number;
    caret(pos: number): JQuery;
    caret(text: string): JQuery;

    range(): JQueryCaretRange;
    range(startPos: number, endPos?: number): JQuery;
    range(text: string): JQuery;

    selectAll(): JQuery;
    deselectAll(): JQuery;
}
