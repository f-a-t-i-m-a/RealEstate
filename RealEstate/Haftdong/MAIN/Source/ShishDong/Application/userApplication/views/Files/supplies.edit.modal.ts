module JahanJooy.ShishDong.File {
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
    import EnumExtentions = Common.EnumExtentions;
    appModule.controller("EditSupplyModalController",
    [
        '$scope', '$timeout', '$state', '$http', "toastr", "$modalInstance", "allPriceSpecificationTypePerProperty",
        "$rootScope",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            $http,
            toastr,
            $modalInstance,
            allPriceSpecificationTypePerProperty,
            $rootScope) => {

            $scope.TotalCalculatedPrice = 0;
            $scope.isCollapsed = true;

            $scope.$watch("supply",
                s => {
                    if (s != null && s.PriceSpecificationType === 1) {
                        s.Price = s.TotalPrice;
                    } else if (s != null && s.PriceSpecificationType === 2) {
                        s.Price = s.PricePerEstateArea;
                    } else if (s != null && s.PriceSpecificationType === 3) {
                        s.Price = s.PricePerUnitArea;
                    }

                    if (s != null) {
                        s.OwnerCanBeContacted = s.ContactToOwner;

                        if (s.AgencyContact != null) {
                            s.AgencyContact.Phones.forEach(p => {
                                p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                            });
                            s.AgencyContact.Emails.forEach(e => {
                                e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                            });
                            s.AgencyContact.Addresses.forEach(a => {
                                a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                            });
                        }

                        if (s.OwnerContact != null) {
                            s.OwnerContact.Phones.forEach(p => {
                                p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                            });
                            s.OwnerContact.Emails.forEach(e => {
                                e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                            });
                            s.OwnerContact.Addresses.forEach(a => {
                                a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                            });
                        }
                    };
                });

            $scope.$watch("supply.Property.PropertyType",
                p => {
                    if ($scope.supply != null) {
                        var allSalePriceSpecificationTypes =
                            EnumExtentions.getValues(SalePriceSpecificationType)
                                .select(i => {
                                    return {
                                        id: i,
                                        text: Common.localization.translateEnum("SalePriceSpecificationType", i)
                                    }
                                })
                                .toArray();
                        $scope
                            .priceSpecificationTypeOptions =
                            Enumerable.from<any>(allSalePriceSpecificationTypes)
                            .where(pr => Enumerable.from<any>(allPriceSpecificationTypePerProperty)
                                .any(a => a.propertyType === p &&
                                    Enumerable.from(a.priceTypes).any(pt => pt === pr.id)))
                            .toArray();
                    }
                });

            $scope.$watch("supply.IntentionOfOwner",
                p => {
                    if ($scope.supply != null) {
                        if (p != null && p === 2) {
                            $scope.showRentPanel = false;
                            $scope.showSalePanel = true;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 3) {
                            $scope.showSalePanel = false;
                            $scope.showRentPanel = true;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 1) {
                            $scope.showRentPanel = true;
                            $scope.showSalePanel = false;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 4) {
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = true;
                            $scope.showSalePanel = false;
                        } else {
                            $scope.showSalePanel = false;
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = false;
                        }
                    }
                });

            $scope.clearPrice = () => {
                $scope.supply.Price = null;
            }

            function calculatePrice() {
                if ($scope.supply.PriceSpecificationType === null) {
                    return 0;
                }
                if ($scope.supply.PriceSpecificationType === 1) {
                    return $scope.supply.Price;
                } else if ($scope.supply.PriceSpecificationType === 2 && $scope.supply.Property.EstateArea != null) {
                    return $scope.supply.Price * $scope.supply.Property.EstateArea;
                } else if ($scope.supply.PriceSpecificationType === 3 && $scope.supply.Property.UnitArea != null) {
                    return $scope.supply.Price * $scope.supply.Property.UnitArea;
                }
                return 0;
            }

            $scope.$watchGroup([
                "supply.PriceSpecificationType", "supply.Price"
            ],
            () => {
                if ($scope.supply != null)
                    $scope.TotalCalculatedPrice = calculatePrice();
            });

            var setOwnerFirstTime = true;
            $scope.$watch("supply.Contact",
                rc => {
                    if (setOwnerFirstTime) {
                        if (rc !== null && rc !== undefined && Object.getOwnPropertyNames(rc).length > 0) {
                            $scope.originContact = JSON.parse(JSON.stringify(rc));
                            setOwnerFirstTime = false;
                        } else {
                            $scope.originContact = null;
                        }
                    }
                });

            var listener = $rootScope.$on("SetOriginContact",
            () => {
                $scope.supply.Contact = $scope.originContact;

                if ($scope.supply.Contact.AgencyContact != null) {
                    $scope.supply.Contact.AgencyContact.Phones.forEach(p => {
                        p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                    });
                    $scope.supply.Contact.AgencyContact.Emails.forEach(e => {
                        e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                    });
                    $scope.supply.Contact.AgencyContact.Addresses.forEach(a => {
                        a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                    });
                }

                if ($scope.supply.Contact.OwnerContact != null) {
                    $scope.supply.Contact.OwnerContact.Phones.forEach(p => {
                        p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                    });
                    $scope.supply.Contact.OwnerContact.Emails.forEach(e => {
                        e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                    });
                    $scope.supply.Contact.OwnerContact.Addresses.forEach(a => {
                        a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                    });
                }
            });

            $scope.$on("$destroy", listener);

            $scope.onSave = () => {
                if ($scope.supply.PriceSpecificationType === null) {
                    $scope.supply.TotalPrice = 0;
                } else if ($scope.supply.PriceSpecificationType === 1) {
                    $scope.supply.TotalPrice = $scope.supply.Price;
                } else if ($scope.supply.PriceSpecificationType === 2) {
                    $scope.supply.PricePerEstateArea = $scope.supply.Price;
                } else if ($scope.supply.PriceSpecificationType === 3) {
                    $scope.supply.PricePerUnitArea = $scope.supply.Price;
                }

                if ($scope.supply.Contact !== null) {
                    $scope.supply.OwnerCanBeContacted = $scope.supply.Contact.OwnerCanBeContacted;
                    if ($scope.supply.Contact.OwnerCanBeContacted === true) {
                        $scope.supply.OwnerContact = $scope.supply.Contact.OwnerContact;
                    } else if ($scope.supply.Contact.OwnerCanBeContacted === false) {
                        $scope.supply.AgencyContact = $scope.supply.Contact.AgencyContact;
                    }
                }

                $http.post("/api/haftdong/files/updatesupply", $scope.supply)
                    .success(() => {
                        toastr.success("آگهی با موفقیت ویرایش شد.");
                        $modalInstance.close();
                    });
            }

            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);
}