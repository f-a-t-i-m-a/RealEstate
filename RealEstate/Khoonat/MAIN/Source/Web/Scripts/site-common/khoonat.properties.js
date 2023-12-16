
(function() {
	khoonat.partial.searchPropertyListings = {
		doInit: function(popupRoot) {

			applyDigitGrouping();

			function setSearchFieldVisibilities() {
				var intentionOfOwner = $("#frmSearch select.intentionOfOwner", popupRoot).val();
				var propertyType = $("#frmSearch select.propertyType", popupRoot).val();

				var forRent = intentionOfOwner == "ForRent";
				var forSale = intentionOfOwner == "ForSale";
				var forEstate = propertyType == "Land" || propertyType == "Garden" || propertyType == "House" || propertyType == "Villa";
				var forUnit = propertyType == "House" || propertyType == "Villa" || propertyType == "Apartment" || propertyType == "Penthouse";

				if (forRent) $("#frmSearch .displayForRent").show();
				else $("#frmSearch .displayForRent").hide();
				if (forSale) $("#frmSearch .displayForSale").show();
				else $("#frmSearch .displayForSale").hide();
				if (forEstate) $("#frmSearch .displayForEstate").show();
				else $("#frmSearch .displayForEstate").hide();
				if (forUnit) $("#frmSearch .displayForUnit").show();
				else $("#frmSearch .displayForUnit").hide();
				if (forSale && forUnit) $("#frmSearch .displayForUnitSale").show();
				else $("#frmSearch .displayForUnitSale").hide();
				if (forSale && forEstate) $("#frmSearch .displayForEstateSale").show();
				else $("#frmSearch .displayForEstateSale").hide();

				if (!forSale && !forRent) $("#frmSearch .displayForNoIntention").show();
				else $("#frmSearch .displayForNoIntention").hide();
				if (!forEstate && !forUnit) $("#frmSearch .displayForNoPropertyType").show();
				else $("#frmSearch .displayForNoPropertyType").hide();

				jj.modal.adjust();
			}

			$("#frmSearch .intentionOfOwner").change(setSearchFieldVisibilities);
			$("#frmSearch .propertyType").change(setSearchFieldVisibilities);
			setSearchFieldVisibilities();

			$("#frmSearch .submit").click(function () {
				$("#frmSearch").submit();
			});
		}
	};
    khoonat.page.mainpropertiessavesearch = {
        doInit: function() {
            function rebuildSampleSmsText() {
                var parts = [];
                if ($("#SmsPartListingCode")[0].checked) parts.push("کد 123456");
                if ($("#SmsPartPropertyType")[0].checked) parts.push("آپارتمان مسکونی");
                if ($("#SmsPartNumberOfRooms")[0].checked) parts.push("دو خوابه");
                if ($("#SmsPartShortGeographicRegion")[0].checked && !($("#SmsPartLongGeographicRegion")[0].checked)) parts.push("زعفرانیه");
                if ($("#SmsPartLongGeographicRegion")[0].checked) parts.push("زعفرانیه تهران، منطقه یک");
                if ($("#SmsPartUserEnteredAddress")[0].checked) parts.push("خیابان مقدس اردبیلی، کوچه اول، پلاک 1");
                if ($("#SmsPartArea")[0].checked) parts.push("120 متر");
                if ($("#SmsPartPriceOrRent")[0].checked) parts.push("564 میلیون");
                if ($("#SmsPartPerAreaPrice")[0].checked) parts.push("متری 4.7 میلیون");
                if ($("#SmsPartContactName")[0].checked) parts.push("علی محمد زاده");
                if ($("#SmsPartContactPhone")[0].checked) parts.push("09122222222");
                if ($("#SmsPartContactSecondPhone")[0].checked) parts.push("02122222222");
                if ($("#SmsPartListingUrl")[0].checked) parts.push("http://khoonat.com/p/123456");

                var textResult = parts.join("، ");
                if (textResult.length < 1) textResult = "...";
                if ($("#SmsPartSiteName")[0].checked) textResult = "خونه ت: " + textResult;
                $("#sampleSmsText").text(textResult);
            }

            $("#SmsPartSiteName").change(rebuildSampleSmsText);
            $("#SmsPartListingUrl").change(rebuildSampleSmsText);
            $("#SmsPartListingCode").change(rebuildSampleSmsText);
            $("#SmsPartPropertyType").change(rebuildSampleSmsText);
            $("#SmsPartNumberOfRooms").change(rebuildSampleSmsText);
            $("#SmsPartShortGeographicRegion").change(rebuildSampleSmsText);
            $("#SmsPartLongGeographicRegion").change(rebuildSampleSmsText);
            $("#SmsPartUserEnteredAddress").change(rebuildSampleSmsText);
            $("#SmsPartArea").change(rebuildSampleSmsText);
            $("#SmsPartPriceOrRent").change(rebuildSampleSmsText);
            $("#SmsPartPerAreaPrice").change(rebuildSampleSmsText);
            $("#SmsPartContactName").change(rebuildSampleSmsText);
            $("#SmsPartContactPhone").change(rebuildSampleSmsText);
            $("#SmsPartContactSecondPhone").change(rebuildSampleSmsText);

            rebuildSampleSmsText();
        }
    };

	khoonat.page.mainpropertyviewdetails = {
		doInit: function() {
			setupFavoriteToggles();
		}
	};

	khoonat.page.mainpropertiesbrowse = {
		doInit: function () {
			setupFavoriteToggles();
		}
	};

	function setupFavoriteToggles() {
		$(document).on("click", ".fav-toggle[data-property-id]", function (e) {
			e.preventDefault();
			e.stopPropagation();
			var propertyId = $(this).attr('data-property-id');
			$(this).toggleClass("favorited");
			$.ajax({
				url: '/Property/ToggleFavorite',
				type: 'POST',
				data: { propertyId: propertyId },
				dataType: 'json'
			}).done(function (data) {
				if (data) {
					$(".favCount[data-property-id='" + propertyId + "']").text(data.TimesMarkedAsFavorite);
				}
			});

			return false;
		});
	}

})();
