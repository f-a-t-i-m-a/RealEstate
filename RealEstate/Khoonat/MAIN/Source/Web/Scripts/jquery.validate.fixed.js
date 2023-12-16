
// Fixes range validation issues in JQuery Validation plugin
// The plugin does not recognize numeric ranges when formatted numbers are used (with thousand separators)
// This file replaces three functions in the plugin to avoid modifying the plugin source code.
// This file should be referenced AFTER JQuery Validate plugin

(function() {
    $.validator.methods.min = function (value, element, param) {
        var convertedValue = isNaN(param) ? value : parseFormattedFloat(value);
        return this.optional(element) || convertedValue >= param;
    };

    $.validator.methods.max = function (value, element, param) {
        var convertedValue = isNaN(param) ? value : parseFormattedFloat(value);
        return this.optional(element) || convertedValue <= param;
    };

    $.validator.methods.range = function (value, element, param) {
        var convertedValue = isNaN(param[0]) ? value : parseFormattedFloat(value);
        return this.optional(element) || (convertedValue >= param[0] && convertedValue <= param[1]);
    };

    //var originalNumber = $.validator.methods.number;
    $.validator.methods.number = function (value, element) {
        var cleanedValue = arabicToEnglishDigits(removeNumberFormat(value));
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(cleanedValue);
    };
})();


