
// Functions

function removeNumberFormat(value) {
    return value.replace(/[^0-9\.\u0660-\u0669\u06F0-\u06F9]/g, "");
}

function parseFormattedFloat(value) {
    return parseFloat(arabicToEnglishDigits(removeNumberFormat(value)));
}

function arabicToEnglishDigits(value) {
    if (!(/[\u0660-\u0669\u06F0-\u06F9]/g.test(value)))
        return value;

    return value
        .replace(/[\u0660\u06F0]/g, "0")
        .replace(/[\u0661\u06F1]/g, "1")
        .replace(/[\u0662\u06F2]/g, "2")
        .replace(/[\u0663\u06F3]/g, "3")
        .replace(/[\u0664\u06F4]/g, "4")
        .replace(/[\u0665\u06F5]/g, "5")
        .replace(/[\u0666\u06F6]/g, "6")
        .replace(/[\u0667\u06F7]/g, "7")
        .replace(/[\u0668\u06F8]/g, "8")
        .replace(/[\u0669\u06F9]/g, "9");
}

function englishToArabicDigits(value) {
    return value
        .replace(/0/g, "\u06F0")
        .replace(/1/g, "\u06F1")
        .replace(/2/g, "\u06F2")
        .replace(/3/g, "\u06F3")
        .replace(/4/g, "\u06F4")
        .replace(/5/g, "\u06F5")
        .replace(/6/g, "\u06F6")
        .replace(/7/g, "\u06F7")
        .replace(/8/g, "\u06F8")
        .replace(/9/g, "\u06F9");
}

function applyDigitGrouping() {
	// Required AutoNumeric JS library to be included
	$(".digitGrouping").autoNumeric("init", { vMax: "999999999999999", aPad: false });
}

function roundToPrecision(num, decimals) {
	return Math.round(num * Math.pow(10, decimals)) / Math.pow(10, decimals);
}

function setSelectOptions(selector, data) {
	var valuesHtml = '';
	if (data) {
		$.each(data, function (i, v) {
			valuesHtml += '<option value="' + v.ID + '">' + v.Name + '</option>';
		});
	}

	var emptyOption = $(selector + ' option[value=""]');
	if (emptyOption && emptyOption.length > 0)
		valuesHtml = '<option value="">' + emptyOption.text() + '</option>' + valuesHtml;

	var prevValue = $(selector).val();

	$(selector).html(valuesHtml);

	var selectedValue = prevValue;
	if (data) {
		if (data.length == 1) {
			selectedValue = data[0].ID;
		}
	}

	$(selector).val(selectedValue);
	if ($(selector).val() != prevValue)
		$(selector).change();

	$(selector).trigger("liszt:updated");
}

function shortNumericString(number) {
	if (!isFinite(number))
		return null;

	var format = "";

	if (number >= 1000)
	{
		number /= 1000;
		format = " هزار";
	}

	if (number >= 1000)
	{
		number /= 1000;
		format = " میلیون";
	}

	if (number >= 1000)
	{
		number /= 1000;
		format = " میلیارد";
	}

	var integralDigits = Math.floor(number).toString().length;
	number = roundToPrecision(number, Math.max(3 - integralDigits, 0));
	return englishToArabicDigits(number + format);
}

function setupChosen() {
	$(".chzn").chosen({ no_results_text: "هیچ موردی یافت نشد" });
}

function refreshPage() {
    location.reload();
}

//
// Google Maps API related functions

function getMarkerWKT(marker) {
	return "POINT (" + marker.getPosition().lng() + " " + marker.getPosition().lat() + ")";
}

function getPolygonWKT(polygon) {
	var result = "POLYGON ((";
	var path = polygon.getPath();
	path.forEach(function (latLng, num) {
		if (num > 0)
			result = result + ", ";
		result = result + latLng.lng() + " " + latLng.lat();
	});
	result = result + ", " + path.getAt(0).lng() + " " + path.getAt(0).lat();
	return result + "))";
}

