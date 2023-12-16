conn = new Mongo();
db = conn.getDB("realestateagencytest");
hasAdmin = db.ApplicationUser.count({ UserName: "administrator" }) > 0;
if (!hasAdmin) {
    db.ApplicationUser.insert({
        UserName: "administrator",
        DisplayName: "Administrator",
        CreationTime: ISODate("2015-11-22T13:13:41.551Z"),
        IsVerified: true,
        IsEnabled: true,
        Type: 99,
        PasswordHash: "ABlVLQtUXAOkmIby6jcVRVDRsZV8Fk6ai2MC3PkJbVeOmXorFyl9ol62nKyj5eMU7A==",
        SecurityStamp: "564282f4-3617-45f0-8125-55700871e543",
        Roles: ["Administrator", "VerifiedUser"],
        Contact: {}
    });
} else {
    db.ApplicationUser.update(
    {
        UserName: "administrator"
    },
    {
        UserName: "administrator",
        DisplayName: "Administrator",
        CreationTime: ISODate("2015-11-22T13:13:41.551Z"),
        IsVerified: true,
        IsEnabled: true,
        Type: 99,
        PasswordHash: "ABlVLQtUXAOkmIby6jcVRVDRsZV8Fk6ai2MC3PkJbVeOmXorFyl9ol62nKyj5eMU7A==",
        SecurityStamp: "564282f4-3617-45f0-8125-55700871e543",
        Roles: ["Administrator", "VerifiedUser"],
        Contact: {}
    },
    {
        upsert: true
    });
}