module JahanJooy.Common {
    export class DownloadUtils {
        static downloadBlob(blob: Blob, fileName: string): void {
            if (navigator.msSaveBlob) {
                navigator.msSaveBlob(blob, fileName);
                return;
            }

            var d = document;
            var a = d.createElement("a");
            var url = URL.createObjectURL(blob);

            a.setAttribute("download", fileName);
            a.href = url;
            a.setAttribute("style", "display: none;");

            d.body.appendChild(a);
            setTimeout(() => {
                if (a.click) {
                    a.click();
                } else if (d.createEvent) {
                    var eventObj = d.createEvent("MouseEvents");
                    eventObj.initEvent("click", true, true);
                    a.dispatchEvent(eventObj);
                }
                d.body.removeChild(a);
            }, 100);
        }
    }
}
