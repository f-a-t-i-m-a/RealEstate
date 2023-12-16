conn = new Mongo();
db = conn.getDB("realestateagency");
db.Property.update({}, { $rename: { "TestParameter": "NewTestParameter" } }, { multi: true });