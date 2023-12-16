conn = new Mongo();
db = conn.getDB("realestateagency");
//Version 0.0.0
db.ApplicationUser.createIndex({ LastIndexingTime: 1 });
db.Contract.createIndex({ LastIndexingTime: 1 });
db.Customer.createIndex({ LastIndexingTime: 1 });
db.Property.createIndex({ LastIndexingTime: 1 });
db.Request.createIndex({ LastIndexingTime: 1 });
db.Supply.createIndex({ LastIndexingTime: 1 });
db.Vicinity.createIndex({ SqlID: 1 });
//Version 0.0.12
db.ApplicationUser.update({}, { $unset: { Approved: 1 } }, { multi: true });
db.Contract.update({}, { $unset: { "PropertyReference.IsHidden": 1 } }, { multi: true });
db.Supply.update({}, { $unset: { "Property.IsHidden": 1 } }, { multi: true });
//Version 0.0.15
db.Supply.update({}, { $unset: { ExpirationTime: 1 } }, { multi: true });
db.Supply.update({}, { $unset: { ExpiratedByID: 1 } }, { multi: true });
db.Supply.update({}, { $unset: { StartTime: 1 } }, { multi: true });
db.Supply.update({}, { $unset: { "Property.OwnerID": 1 } }, { multi: true });
db.Supply.update({}, { $unset: { "Property.OwnerFullName": 1 } }, { multi: true });
db.Contract.update({}, { $unset: { "PropertyReference.OwnerID": 1 } }, { multi: true });
db.Contract.update({}, { $unset: { "PropertyReference.OwnerFullName": 1 } }, { multi: true });
//Version 0.0.20

//Version 0.0.21
db.Request.update({}, { $rename: { Neighborhood: 'NeighborhoodDescription' } }, { multi: true });

//Version 0.0.22
db.Contract.update({}, { $unset: { "RequestReference.Address": 1 } }, { multi: true });

//Version 0.0.44
db.ApplicationUser.update({}, { $unset: { Agency: 1 } }, { multi: true });
db.ApplicationUser.update({}, { $unset: { AgencyID: 1 } }, { multi: true });
db.Supply.update({}, { $rename: { SqlID: 'ExternalID' } }, { multi: true });
db.Property.update({}, { $rename: { SqlID: 'ExternalID' } }, { multi: true });

db.Supply.find()
    .forEach(function(x) {
        if (x.ExternalID != null) {
            x.ExternalID = x.ExternalID.toString();
            db.Supply.save(x);
        }
    });

db.Property.find()
    .forEach(function(x) {
        if (x.ExternalID != null) {
            x.ExternalID = x.ExternalID.toString();
            db.Property.save(x);
        }
    });

db.Property.find()
    .forEach(function(p) {
        if (p.Photos != null) {
            p.Photos.forEach(function(h) {
                if (h.SqlID != null) {
                    h.ExternalID = "" + h.SqlID;
                    delete h.SqlID;
                }
            });
            db.Property.save(p);
        }
    });

//Version 0.0.62
db.Customer.find()
    .forEach(function(x) {
        x.FullName = x.FirstName + " " + x.LastName;
        db.Customer.save(x);
    });

db.Supply.update({}, { $unset: { "Property.Owner.FirstName": 1 } }, { multi: true });
db.Supply.update({}, { $unset: { "Property.Owner.LastName": 1 } }, { multi: true });
db.Property.update({}, { $unset: { "Owner.FirstName": 1 } }, { multi: true });
db.Property.update({}, { $unset: { "Owner.LastName": 1 } }, { multi: true });
db.Customer.update({}, { $unset: { "Deputy.FirstName": 1 } }, { multi: true });
db.Customer.update({}, { $unset: { "Deputy.LastName": 1 } }, { multi: true });
db.Customer.update({}, { $unset: { FirstName: 1 } }, { multi: true });
db.Customer.update({}, { $unset: { LastName: 1 } }, { multi: true });
db.Request.update({}, { $unset: { "Owner.FirstName": 1 } }, { multi: true });
db.Request.update({}, { $unset: { "Owner.LastName": 1 } }, { multi: true });

//Version 0.0.65
db.Property.update({}, { $unset: { IsPublic: 1 } }, { multi: true });
db.Supply.update({}, { $unset: { "Property.IsPublic": 1 } }, { multi: true });
db.Supply.update({ "Property.SourceType": 2 }, { $set: { IsPublic: true } }, { multi: true });
db.Supply.update({ "Property.SourceType": 3 }, { $set: { IsPublic: true } }, { multi: true });

db.Property.find({ SourceType: 2 })
    .forEach(function(p) {
        if (p.Supplies != null) {
            p.Supplies.forEach(function(s) {
                s.IsPublic = true;
            });
            db.Property.save(p);
        }
    });

db.Property.find({ SourceType: 3 })
    .forEach(function(p) {
        if (p.Supplies != null) {
            p.Supplies.forEach(function(s) {
                s.IsPublic = true;
            });
            db.Property.save(p);
        }
    });

db.Customer.update({}, { $unset: { IsPublic: 1 } }, { multi: true });
db.Request.update({}, { $set: { SourceType: 1 } }, { multi: true });
db.UserActivity.update({}, { $set: { ApplicationType: 1 } }, { multi: true });

db.Property.find({})
    .forEach(function(p) {
        if (p.Supplies != null) {
            p.Supplies.forEach(function(s) {

                var supply = db.Supply.findOne({ "_id": s._id });

                if (supply != null &&
                    supply.ContactInfo != null &&
                    supply.ContactInfo.OwnerName != null &&
                    supply.ContactInfo.OwnerName !== "") {
                    s.ContactToOwner = true;
                } else if (supply != null &&
                    supply.ContactInfo != null &&
                    ((supply.ContactInfo.AgencyName != null && supply.ContactInfo.AgencyName !== "") ||
                    (supply.ContactInfo.ContactName != null && supply.ContactInfo.ContactName !== ""))) {
                    s.ContactToAgency = true;
                }
            });
            db.Property.save(p);
        }
    });

//Version 1.0.3
db.Request.update({}, { $rename: { NeighborhoodDescription: 'Description' } }, { multi: true });

db.Request.update({}, { $rename: { "UnitUsageType": "UsageType" } }, { multi: true });
db.Property.update({}, { $rename: { "UnitUsageType": "UsageType" } }, { multi: true });
db.Supply.update({ "Property": { $ne: null } },
{ $rename: { "Property.UnitUsageType": "Property.UsageType" } },
{ multi: true });
db.Request.update({}, { $rename: { NumberFloorsAboveGround: "TotalNumberOfFloors" } }, { multi: true });
db.Property.update({}, { $rename: { NumberFloorsAboveGround: "TotalNumberOfFloors" } }, { multi: true });
db.Customer.update({}, { $rename: { "FullName": "DisplayName" } }, { multi: true });

db.Request.update({ "Owner": { $ne: null } }, { $unset: { "Owner.MobileNumber": 1 } }, { multi: true });
db.Property.update({ "Owner": { $ne: null } }, { $unset: { "Owner.MobileNumber": 1 } }, { multi: true });
db.Supply.update({ "Property": { $ne: null }, "Property.Owner": { $ne: null } },
{ $unset: { "Property.Owner.MobileNumber": 1 } },
{ multi: true });

db.ApplicationUser.update({}, { $unset: { "ShowInUsersList": 1 } }, { multi: true });
db.ApplicationUser.update({}, { $unset: { "FullName": 1 } }, { multi: true });
db.Request.update({ "Owner": { $ne: null } }, { $rename: { "Owner.FullName": "Owner.DisplayName" } }, { multi: true });
db.Property.update({ "Owner": { $ne: null } }, { $rename: { "Owner.FullName": "Owner.DisplayName" } }, { multi: true });
db.Supply.update({ "Property": { $ne: null }, "Property.Owner": { $ne: null } },
{ $rename: { "Property.Owner.FullName": "Property.Owner.DisplayName" } },
{ multi: true });

db.ApplicationUser.find()
    .forEach(function(u) {
        u.Contact = {
            ContactName: u.DisplayName,
            Phones: [],
            Emails: [],
            Addresses: []
        };
        var newContact;
        if (u.ContactMethods != null) {
            u.ContactMethods.forEach(function(c) {
                if (c.ContactMethodType === 1) {
                    newContact = {
                        ID: c.ID,
                        Value: c.ContactMethodText,
                        NormalizedValue: "+98" + c.ContactMethodText.substring(1),
                        IsVerifiable: c.IsVerifiable,
                        IsVerified: c.IsVerified,
                        IsActive: c.IsActive,
                        IsDeleted: c.IsDeleted,
                        UserContactMethodVerification: c.UserContactMethodVerification,
                        CountryCode: "98",
                        AreaCode: c.ContactMethodText.substring(0, 4),
                        CanReceiveSms: true
                    };
                    u.Contact.Phones.push(newContact);
                } else if (c.ContactMethodType === 2) {
                    newContact = {
                        ID: c.ID,
                        Value: c.ContactMethodText,
                        NormalizedValue: c.ContactMethodText,
                        IsVerifiable: c.IsVerifiable,
                        IsVerified: c.IsVerified,
                        IsActive: c.IsActive,
                        IsDeleted: c.IsDeleted,
                        UserContactMethodVerification: c.UserContactMethodVerification
                    };
                    u.Contact.Emails.push(newContact);
                }
            });
        }

        if (u.Address != null) {
            newContact = {
                Value: u.Address,
                NormalizedValue: u.Address,
                IsVerifiable: false,
                IsVerified: false,
                IsActive: true,
                IsDeleted: false,
                UserContactMethodVerification: {}
            };
            u.Contact.Addresses.push(newContact);
        }

        delete u.ContactMethods;
        delete u.Address;

        db.ApplicationUser.save(u);
    });

db.Customer.find()
    .forEach(function(cu) {
        cu.Contact = {
            ContactName: cu.DisplayName,
            Phones: [],
            Emails: [],
            Addresses: []
        };
        var newContact;
        if (cu.PhoneNumbers != null) {
            cu.PhoneNumbers.forEach(function(c) {
                if (c != null) {
                    newContact = {
                        Value: c,
                        NormalizedValue: c,
                        IsVerifiable: false,
                        IsVerified: false,
                        IsActive: true,
                        IsDeleted: false,
                        UserContactMethodVerification: {},
                        CountryCode: "98",
                        AreaCode: "021",
                        CanReceiveSms: false
                    };
                    cu.Contact.Phones.push(newContact);
                }
            });
        }

        if (cu.MobileNumbers != null) {
            cu.MobileNumbers.forEach(function(c) {
                if (c != null) {
                    newContact = {
                        Value: c,
                        NormalizedValue: c,
                        IsVerifiable: true,
                        IsVerified: false,
                        IsActive: true,
                        IsDeleted: false,
                        UserContactMethodVerification: {},
                        CountryCode: "98",
                        AreaCode: c.substring(0, 4),
                        CanReceiveSms: true
                    };
                    cu.Contact.Phones.push(newContact);
                }
            });
        }

        if (cu.Address != null) {
            newContact = {
                Value: cu.Address,
                NormalizedValue: cu.Address,
                IsVerifiable: false,
                IsVerified: false,
                IsActive: true,
                IsDeleted: false,
                UserContactMethodVerification: {}
            };
            cu.Contact.Addresses.push(newContact);
        }

        if (cu.Email != null) {
            newContact = {
                Value: cu.Email,
                NormalizedValue: cu.Email,
                IsVerifiable: true,
                IsVerified: false,
                IsActive: true,
                IsDeleted: false,
                UserContactMethodVerification: {}
            };
            cu.Contact.Emails.push(newContact);
        }

        delete cu.Email;
        delete cu.Address;
        delete cu.PhoneNumbers;
        delete cu.MobileNumbers;

        db.Customer.save(cu);
    });

db.Request.find()
    .forEach(function(r) {
        if (r.IsPublic) {
            if (r.ContactInfo != null) {
                r.OwnerCanBeContacted = r.ContactInfo.OwnerCanBeContacted;

                var newContact;
                if (r.OwnerCanBeContacted) {
                    r.OwnerContact = {
                        ContactName: r.ContactInfo.OwnerName,
                        Phones: [],
                        Emails: [],
                        Addresses: []
                    };

                    if (r.ContactInfo.OwnerPhoneNumbers != null) {
                        r.ContactInfo.OwnerPhoneNumbers.forEach(function(c) {
                            if (c != null) {
                                newContact = {
                                    Value: c,
                                    NormalizedValue: c,
                                    IsVerifiable: true,
                                    IsVerified: false,
                                    IsActive: true,
                                    IsDeleted: false,
                                    UserContactMethodVerification: {},
                                    CountryCode: "98",
                                    AreaCode: c.substring(0, 4),
                                    CanReceiveSms: true
                                };
                                r.OwnerContact.Phones.push(newContact);
                            }
                        });
                    }

                    if (r.ContactInfo.OwnerEmailAddress != null) {
                        newContact = {
                            Value: r.ContactInfo.OwnerEmailAddress,
                            NormalizedValue: r.ContactInfo.OwnerEmailAddress,
                            IsVerifiable: true,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        r.OwnerContact.Emails.push(newContact);
                    }
                } else {
                    r.AgencyContact = {
                        OrganizationName: r.ContactInfo.AgencyName,
                        ContactName: r.ContactInfo.ContactName,
                        Phones: [],
                        Emails: [],
                        Addresses: []
                    };

                    if (r.ContactInfo.ContactPhoneNumbers != null) {
                        r.ContactInfo.ContactPhoneNumbers.forEach(function(c) {
                            if (c != null) {
                                newContact = {
                                    Value: c,
                                    NormalizedValue: c,
                                    IsVerifiable: true,
                                    IsVerified: false,
                                    IsActive: true,
                                    IsDeleted: false,
                                    UserContactMethodVerification: {},
                                    CountryCode: "98",
                                    AreaCode: c.substring(0, 4),
                                    CanReceiveSms: true
                                };
                                r.AgencyContact.Phones.push(newContact);
                            }
                        });
                    }

                    if (r.ContactInfo.AgencyAddress != null) {
                        newContact = {
                            Value: r.ContactInfo.AgencyAddress,
                            NormalizedValue: r.ContactInfo.AgencyAddress,
                            IsVerifiable: false,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        r.AgencyContact.Addresses.push(newContact);
                    }

                    if (r.ContactInfo.ContactEmailAddress != null) {
                        newContact = {
                            Value: r.ContactInfo.ContactEmailAddress,
                            NormalizedValue: r.ContactInfo.ContactEmailAddress,
                            IsVerifiable: true,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        r.AgencyContact.Emails.push(newContact);
                    }
                }
            }
        }

        delete r.ContactInfo;

        db.Request.save(r);
    });

db.Supply.find()
    .forEach(function(s) {
        if (s.IsPublic) {
            if (s.ContactInfo != null) {
                s.OwnerCanBeContacted = s.ContactInfo.OwnerCanBeContacted;

                var newContact;
                if (s.OwnerCanBeContacted) {
                    s.OwnerContact = {
                        ContactName: s.ContactInfo.OwnerName,
                        Phones: [],
                        Emails: [],
                        Addresses: []
                    };

                    if (s.ContactInfo.OwnerPhoneNumbers != null) {
                        s.ContactInfo.OwnerPhoneNumbers.forEach(function(c) {
                            if (c != null) {
                                newContact = {
                                    Value: c,
                                    NormalizedValue: c,
                                    IsVerifiable: true,
                                    IsVerified: false,
                                    IsActive: true,
                                    IsDeleted: false,
                                    UserContactMethodVerification: {},
                                    CountryCode: "98",
                                    AreaCode: c.substring(0, 4),
                                    CanReceiveSms: true
                                };
                                s.OwnerContact.Phones.push(newContact);
                            }
                        });
                    }

                    if (s.ContactInfo.OwnerEmailAddress != null) {
                        newContact = {
                            Value: s.ContactInfo.OwnerEmailAddress,
                            NormalizedValue: s.ContactInfo.OwnerEmailAddress,
                            IsVerifiable: true,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        s.OwnerContact.Emails.push(newContact);
                    }
                } else {
                    s.AgencyContact = {
                        OrganizationName: s.ContactInfo.AgencyName,
                        ContactName: s.ContactInfo.ContactName,
                        Phones: [],
                        Emails: [],
                        Addresses: []
                    };

                    if (s.ContactInfo.ContactPhoneNumbers != null) {
                        s.ContactInfo.ContactPhoneNumbers.forEach(function(c) {
                            if (c != null) {
                                newContact = {
                                    Value: c,
                                    NormalizedValue: c,
                                    IsVerifiable: true,
                                    IsVerified: false,
                                    IsActive: true,
                                    IsDeleted: false,
                                    UserContactMethodVerification: {},
                                    CountryCode: "98",
                                    AreaCode: c.substring(0, 4),
                                    CanReceiveSms: true
                                };
                                s.AgencyContact.Phones.push(newContact);
                            }
                        });
                    }

                    if (s.ContactInfo.AgencyAddress != null) {
                        newContact = {
                            Value: s.ContactInfo.AgencyAddress,
                            NormalizedValue: s.ContactInfo.AgencyAddress,
                            IsVerifiable: false,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        s.AgencyContact.Addresses.push(newContact);
                    }

                    if (s.ContactInfo.ContactEmailAddress != null) {
                        newContact = {
                            Value: s.ContactInfo.ContactEmailAddress,
                            NormalizedValue: s.ContactInfo.ContactEmailAddress,
                            IsVerifiable: true,
                            IsVerified: false,
                            IsActive: true,
                            IsDeleted: false,
                            UserContactMethodVerification: {}
                        };
                        s.AgencyContact.Emails.push(newContact);
                    }
                }
            }
        }

        delete s.ContactInfo;

        db.Supply.save(s);
    });


db.Supply.find({ LastModificationTime: null })
    .forEach(function(s) {
        s.LastModificationTime = s.CreationTime;
        db.Supply.save(s);
    });

db.ApplicationUser.find({ ModificationTime: null })
    .forEach(function (u) {
        u.ModificationTime = u.CreationTime;
        db.ApplicationUser.save(u);
    });

db.Contract.find({ LastModificationTime: null })
    .forEach(function (c) {
        c.LastModificationTime = c.CreationTime;
        db.Contract.save(c);
    });

db.Customer.find({ LastModificationTime: null })
    .forEach(function (c) {
        c.LastModificationTime = c.CreationTime;
        db.Customer.save(c);
    });

db.Property.find({ LastModificationTime: null })
    .forEach(function (p) {
        p.LastModificationTime = p.CreationTime;
        db.Property.save(p);
    });

db.Request.find({ LastModificationTime: null })
    .forEach(function (r) {
        r.LastModificationTime = r.CreationTime;
        db.Request.save(r);
    });

db.Property.update({}, { $rename: { "KitchenCabinetTopType": "KitchenCabinetType" } }, { multi: true });
db.Property.update({}, { $unset: { "KitchenCabinetBodyType": 1 } }, { multi: true });
db.Customer.update({ "Deputy": { $ne: null } }, { $rename: { "Deputy.FullName": "Deputy.DisplayName" } }, { multi: true });
db.Customer.update({ "Deputy": { $ne: null } }, { $unset: { "Deputy.MobileNumber": 1 } }, { multi: true });

db.Request.update({}, { $rename: { "PropertyTypeIDs": "PropertyTypes" } }, { multi: true });

//Version 2.0.6
db.Contract.update({ "RequestReference": { $ne: null } }, { $rename: { "RequestReference.PropertyTypeIDs": "RequestReference.PropertyTypes" } }, { multi: true });