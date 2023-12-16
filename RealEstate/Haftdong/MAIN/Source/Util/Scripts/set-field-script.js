conn = new Mongo();
db = conn.getDB("realestateagency");
db.Property.update({}, { $set: { State: 1 } }, { multi: true });
db.Supply.update({}, { $set: { "Property.SourceType": 2 } }, { multi: true });