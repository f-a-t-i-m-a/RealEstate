conn = new Mongo();
db = conn.getDB("realestateagency");
db.Property.update({}, { $unset: { TestParameter: 1 } }, { multi: true });