module JahanJooy.HaftDong.Components {
    import MapOptions = google.maps.MapOptions;

    interface IGoogleMapScope extends ng.IScope {
        supply: any;
        boundary: any;
        property: any;
        vicinity: any;
        visibility: any;
        displayClass: any;
        editMode: any;
        properties: any;
        clickOnPropertyMarker(input: any): Function;
        clickOnVicinityMarker(input: any): Function;
    }

    class GoogleMapDirectiveInstance {
        private element: JQuery;

        constructor(private $http, private $modal, private $compile) {
        }

        link(scope: IGoogleMapScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) {
            var self = this;
            var existingPropertyMarkers = [];
            var existingVicinityMarkers = [];

            this.element = $("<div></div>");
            this.element.appendTo(element);

            var defaultLat = 35.6892;
            var defaultLong = 51.3890;
            var center = new google.maps.LatLng(defaultLat, defaultLong);
            var saleMarker: { url: string; size: google.maps.Size; origin: google.maps.Point; anchor: google.maps.Point; scaledSize: google.maps.Size };
            var rentMarker: { url: string; size: google.maps.Size; origin: google.maps.Point; anchor: google.maps.Point; scaledSize: google.maps.Size };
            var dailyRentMarker: { url: string; size: google.maps.Size; origin: google.maps.Point; anchor: google.maps.Point; scaledSize: google.maps.Size };
            var fullMortgageMarker: { url: string; size: google.maps.Size; origin: google.maps.Point; anchor: google.maps.Point; scaledSize: google.maps.Size };
            var circleMarker: { url: string; size: google.maps.Size; origin: google.maps.Point; anchor: google.maps.Point; scaledSize: google.maps.Size };
            var infoWindow = new google.maps.InfoWindow();

            var mapProp: MapOptions = {
                center: center,
                zoom: 11,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                scaleControl: true,
                draggable: true,
                overviewMapControl: false,
                styles: [
                    {
                        "featureType": "poi",
                        "stylers": [
                            { "visibility": "off" }
                        ]
                    }
                ]
            };

            this.element.addClass(attrs["displayClass"]);
            var map = new google.maps.Map(this.element[0], mapProp);

            var cursorMarker: google.maps.Marker;
            var userPointMarker = new google.maps.Marker({
                map: map,
                clickable: false
            });

            scope.$watch("property", (p: any) => {
                if (p != null && p.GeographicLocation != null
                    && p.GeographicLocation.X != null
                    && p.GeographicLocation.Y != null) {
                    defaultLat = p.GeographicLocation.Y;
                    defaultLong = p.GeographicLocation.X;
                    var latlong = new google.maps.LatLng(defaultLat, defaultLong);
                    map.setCenter(latlong);
                    map.setZoom(13);
                    userPointMarker.setPosition(latlong);
                    userPointMarker.setMap(map);
                }
            });

            scope.$watch("visibility", v => {
                if (v == null) {
                    this.element.show();
                } else if (v) {
                    this.element.show();
                    this.element.addClass(attrs["displayClass"]);
                    var zoom = map.getZoom();
                    var center = map.getCenter();
                    google.maps.event.trigger(map, "resize");
                    map.setZoom(zoom);
                    map.setCenter(center);
                } else {
                    this.element.hide();
                }
                this.element.appendTo(element);
            });

            scope.$watch("vicinity", (v: any) => {
                if (v != null && v.CenterPoint != null) {
                    defaultLat = v.CenterPoint.Y;
                    defaultLong = v.CenterPoint.X;
                    var latlong = new google.maps.LatLng(defaultLat, defaultLong);
                    map.setCenter(latlong);
                    map.setZoom(13);
                }
            });

            function onSupplyDetailClick(supplyId) {
                self.$http.get("/api/web/supplies/get/" + supplyId, null).success((data: any) => {
                    scope.supply = data;
                    self.$modal.open({
                        templateUrl: "Application/userApplication/views/Supplies/supplies.detail.modal.html",
                        controller: "SupplyDetailModalController",
                        scope: scope,
                        size: "lg"
                    });
                });
            };

            function getPropertyInfoWindowContent(supply) {
                var area = "";
                var price = "";

                if (supply.Property.UnitArea !== 0 && supply.Property.UnitArea != null) {
                    area = "مساحت: " + supply.Property.UnitArea.toString() + " متر بنا";

                } else if (supply.Property.EstateArea !== 0 && supply.Property.EstateArea != null)
                    area = "مساحت: " + supply.Property.EstateArea.toString() + " متر زیر بنا";

                if (supply.TotalPrice != null && supply.TotalPrice !== 0)
                    price = "قیمت: " + supply.TotalPrice.toString() + " تومان";

                var content = "<span id=\"content\">" + Common.localization.translateEnum("PropertyType", supply.Property.PropertyType.toString()) + " - " +
                    Common.localization.translateEnum("UsageType", supply.Property.UsageType.toString()) + "</br>" +
                    "محل: " + supply.Property.Address + "</br> " +
                    area + "</br> " + price + " <a ng-click=\"alert(1)\">جزییات...</a></span>";
//                    area + "</br> " + price + " <a ng-click=\"onSupplyDetailClick('" + supply.ID + "')\">جزییات...</a></span>");
                var el = self.$compile(content)(scope);
                return el[0].innerHTML;

                //                    return '<span id="content">' + Common.localization.translateEnum("PropertyType", supply.Property.PropertyType.toString()) + '-' +
                //                        Common.localization.translateEnum("UsageType", supply.Property.UsageType.toString()) + '</br>' +
                //                        'محل: ' + supply.Property.Address + '</br> ' +
                //                        area + '</br> ' + price + '<a onClick="alert(\'' + supply.ID + '\')">جزییات</a></span>';
            }

            function getVicinityInfoWindowContent(vicinity) {
                return "<span id=\"content\">" + "محله: " + vicinity.Name + "</br>" + "تعداد املاک ثبت شده: " + vicinity.NumberOfPropertyListings + "</br>" +
                //                        ' <a class="bold" target="_blank" href="' + hrefVicinityTemplate.replace('__id__', vicinity.VicinityID) + '">جزییات</a> ' +
                    "</span>";
            }

            function initializeMarkerIcons() {
                rentMarker = {
                    url: "../Content/markers/property-for-rent.png",
                    size: new google.maps.Size(22, 22),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(11, 11),
                    scaledSize: new google.maps.Size(22, 22)
                };

                saleMarker = {
                    url: "../Content/markers/property-for-sale.png",
                    size: new google.maps.Size(22, 22),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(11, 11),
                    scaledSize: new google.maps.Size(22, 22)
                };

                dailyRentMarker = {
                    url: "../Content/markers/property-for-daily-rent.png",
                    size: new google.maps.Size(22, 22),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(11, 11),
                    scaledSize: new google.maps.Size(22, 22)
                };

                fullMortgageMarker = {
                    url: "../Content/markers/property-for-full-mortgage.png",
                    size: new google.maps.Size(22, 22),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(11, 11),
                    scaledSize: new google.maps.Size(22, 22)
                };

                circleMarker = {
                    url: "../Content/markers/item-group.png",
                    size: new google.maps.Size(32, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(16, 16),
                    scaledSize: new google.maps.Size(32, 32)
                };
            }

            scope.$watchCollection("properties", (properties: any) => {
                initializeMarkerIcons();

                var oldPropertyMarkers = existingPropertyMarkers;
                existingPropertyMarkers = [];

                if (properties != null && properties.SupplySummaries != null
                    && properties.SupplySummaries.length !== 0) {
                    properties.SupplySummaries.forEach(p => {
                        if (!(p.Property.GeographicLocation) || (p.Property.GeographicLocation.X === 0 && p.Property.GeographicLocation.Y === 0)) {
                            return;
                        }
                        var title = Common.localization.translateEnum("PropertyType", p.Property.PropertyType.toString());
                        if (p.Property.UsageType != null) {
                            title += " - " + Common.localization.translateEnum("UsageType", p.Property.UsageType.toString());
                        }

                        var marker = new google.maps.Marker({
                            title: title.toString(),
                            position: new google.maps.LatLng(p.Property.GeographicLocation.Y, p.Property.GeographicLocation.X),
                            icon: rentMarker
                        });
                        switch (p.IntentionOfOwner) {
                        case RealEstateAgency.Domain.Enums.Property.IntentionOfOwner.ForSale:
                            marker.setIcon(saleMarker);
                            marker.setMap(map);
                            break;
                        case RealEstateAgency.Domain.Enums.Property.IntentionOfOwner.ForRent:
                            marker.setIcon(rentMarker);
                            marker.setMap(map);
                            break;
                        case RealEstateAgency.Domain.Enums.Property.IntentionOfOwner.ForDailyRent:
                            marker.setIcon(dailyRentMarker);
                            marker.setMap(map);
                            break;
                        case RealEstateAgency.Domain.Enums.Property.IntentionOfOwner.ForFullMortgage:
                            marker.setIcon(fullMortgageMarker);
                            marker.setMap(map);
                            break;

                        }

                        existingPropertyMarkers.push(marker);

//                        google.maps.event.addListener(marker, "click", () => {
//                            infoWindow.close();
//                            this.$http.get("/api/web/supplies/get/" + p.ID, null)
//                                .success((data: any) => {
//                                    if (data != null) {
//                                        infoWindow.setContent(getPropertyInfoWindowContent(data));
//                                        infoWindow.open(map, marker);
//                                    }
//                                });
//                        });

                        google.maps.event.addListener(marker, "click", () => {
                            scope.clickOnPropertyMarker({ supply: [p] });
                        });
                    });
                }

                oldPropertyMarkers.forEach(m => {
                    m.setMap(null);
                });

                var oldVicinityMarkers = existingVicinityMarkers;
                existingVicinityMarkers = [];

                if (properties != null && properties.SupplyGroupsSummaries != null && properties.SupplyGroupsSummaries.length !== 0) {
                    properties.SupplyGroupsSummaries.forEach(v => {
                        if (!(v.GeographicLocation) || (v.GeographicLocation.Lng === 0 && v.GeographicLocation.Lat === 0)) {
                            return;
                        }
                        var marker = new google.maps.Marker({
                            title: v.NumberOfPropertyListings.toString() + "ملک",
                            position: new google.maps.LatLng(v.GeographicLocation.Lat, v.GeographicLocation.Lng),
                            icon: circleMarker
                        });
                        marker.setMap(map);
                        existingVicinityMarkers.push(marker);

                        google.maps.event.addListener(marker, "click", () => {
                            scope.clickOnVicinityMarker({ vicinityId: v.VicinityID});
                        });
                    });
                }

                oldVicinityMarkers.forEach(m => {
                    m.setMap(null);
                });
            });

            google.maps.event.addListener(map, "click", event => {
                if (scope.editMode) {
                    userPointMarker.setPosition(event.latLng);
                    userPointMarker.setMap(map);
                    scope.property.GeographicLocation = {
                        Lat: event.latLng.lat(),
                        Lng: event.latLng.lng()
                    };

                    this.$http.post("/api/web/vicinities/findbypoint", {
                        UserPoint: scope.property.GeographicLocation
                    }).success((data: any) => {
                            if (data != null) {
                                scope.property.Vicinity = data;
                            }
                        });
                }
            });

            function placeUserPointMarker(location) {
                cursorMarker = new google.maps.Marker({
                    position: location,
                    map: map,
                    clickable: false
                });
            }

            google.maps.event.addListener(map, "mouseover", event => {
                if (scope.editMode) {
                    placeUserPointMarker(event.latLng);
                    cursorMarker.setMap(map);
                }
            });

            google.maps.event.addListener(map, "mouseout", () => {
                if (scope.editMode) {
                    if (cursorMarker)
                        cursorMarker.setVisible(false);
                }
            });

            google.maps.event.addListener(map, "mousemove", event => {
                if (scope.editMode) {
                    if (cursorMarker)
                        cursorMarker.setPosition(event.latLng);
                }
            });

            google.maps.event.addListener(map, "bounds_changed", () => {
                scope.$apply(() => { //because binding had one cycle delay
                    scope.boundary = map.getBounds();
                });
            });

            google.maps.event.addListener(map, 'idle', () => {
                scope.$emit('MapIsReady');
            });

        }
    }

    class GoogleMapDirectiveDefinition implements ng.IDirective {
        static $name = "jjGoogleMap";
        static $inject = ["$http", "$modal", "$compile"];

        constructor(private $http, private $modal, private $compile) {
        }

        static getFactory(): ng.IDirectiveFactory {
            var factory = ($http, $modal, $compile) => new GoogleMapDirectiveDefinition($http, $modal, $compile);
            factory.$inject = GoogleMapDirectiveDefinition.$inject;
            return factory;
        }

        restrict = "E";
        scope = {
            property: "=",
            vicinity: "=",
            visibility: "=",
            displayClass: "=",
            editMode: "=",
            properties: "=",
            boundary: "=",
            clickOnPropertyMarker: "&",
            clickOnVicinityMarker: "&"
        };
        link = (scope: IGoogleMapScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            new GoogleMapDirectiveInstance(this.$http, this.$modal, this.$compile).link(scope, element, attrs);
        };
    }

    ngModule.directive(GoogleMapDirectiveDefinition.$name, GoogleMapDirectiveDefinition.getFactory());
}