module JahanJooy.ShishDong {
    appModule.factory("errorsHttpInterceptor", [
        "$q", "$rootScope", "$injector",
        ($q: ng.IQService, $rootScope: IAppRootScopeService, $injector) => {
            return {
                'responseError': rejection => {
                    var serverErrorResponse = new ServerErrorResponse(rejection);
                    var toastr = $injector.get('toastr');
                    serverErrorResponse.errors.forEach(e => {
                        toastr.error(e.getLocalizedMessage());
                    });
                    return $q.reject(rejection);
                }
            };
        }
    ]);

    export class ApiValidationError implements RealEstateAgency.Util.Models.Shared.ApiValidationError {
        constructor(propertyPath: string, errorKey: string, errorParameters: string[]) {
            this.PropertyPath = propertyPath;
            this.ErrorKey = errorKey;
            this.ErrorParameters = errorParameters;
        }

        // ReSharper disable InconsistentNaming
        PropertyPath: string;
        ErrorKey: string;
        ErrorParameters: string[];
        // ReSharper restore InconsistentNaming

        getResourceKey(): string {
            return this.PropertyPath ? this.PropertyPath.replace(/\./g, "_") + "_" + this.ErrorKey : this.ErrorKey;
        }

        getLocalizedMessage(): string {
            var message = Common.localization.translateError(null, this.getResourceKey(), this.ErrorKey);
            if (this.ErrorParameters) {
                message = String.format(message, this.ErrorParameters);
            }

            return message;
        }
    }

    export class ServerErrorResponse {
        url: string;
        method: string;
        data: string;

        status: number;
        statusText: string;
        errors: ApiValidationError[];
        
        constructor(rejection) {
            if (rejection.config) {
                this.url = rejection.config.url;
                this.method = rejection.config.method;
                this.data = rejection.config.data ? JSON.stringify(rejection.config.data, null, 4) : '<null>';
            }

            this.status = rejection.status;
            this.statusText = rejection.statusText;
            this.errors = <any>[];

            if (rejection.data && rejection.data.Errors) {
                rejection.data.Errors.forEach(e => {
                    this.errors.push(new ApiValidationError(e.PropertyPath, e.ErrorKey, e.ErrorParameters));
                });
            }

            if (rejection.data && rejection.data.error) {
                this.errors.push(new ApiValidationError(null, rejection.data.error, [<string>rejection.data.error_description]));
            }

            if (rejection.data && rejection.data.ModelState) {
                var modelState = rejection.data.ModelState;
                for (var path in modelState) {
                    modelState[path].forEach(err => { this.errors.push(new ApiValidationError(path, err, null)); });
                }
            }

            if (!this.errors.length) {
                if (this.status === 0) {
                    this.errors = [new ApiValidationError(null, "CanNotConnectToServer", null)];
                } else if (this.status === 500) {
                    this.errors = [new ApiValidationError(null, "UnknownServerError", null)];
                } else if (this.status === 404) {
                    this.errors = [new ApiValidationError(null, "ServerResourceNotFound", null)];
                } else if (this.status === 401) {
                    this.errors = [new ApiValidationError(null, "AccessDenied", null)];
                } else if (this.status === 400) {
                    this.errors = [new ApiValidationError(null, "UnknownValidationError", null)];
                } else {
                    this.errors = [new ApiValidationError(null, "UnknownError", null)];
                }
            }
        }

    }
}