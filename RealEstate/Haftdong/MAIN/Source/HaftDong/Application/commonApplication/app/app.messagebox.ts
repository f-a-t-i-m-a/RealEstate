module JahanJooy.HaftDong {
    appModule.service("messageBoxService", [
        "$modal", "$q", 
        ($modal, $q) => {
            return {
                confirm: (message, inputMessageType) => {
                    var messageType = "btn btn-success";
                    if (inputMessageType) {
                        if (inputMessageType == "danger") {
                            messageType = "btn btn-danger";
                        } else {
                            messageType = "btn btn-success";
                        }
                    }
                return $modal.open({
                        templateUrl: "/Application/commonApplication/views/app/app.messagebox.confirm.html",
                        controller: "ConfirmModalController",
                        size: "sm",
                        resolve: {
                            message: () => message,
                            messageType: () => messageType
                }
                    }).result;
                }
            };
        }
    ]);
}
 