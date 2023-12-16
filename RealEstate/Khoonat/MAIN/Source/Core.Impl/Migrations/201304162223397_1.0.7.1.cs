namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class _1071 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProvinceAdjacencies",
                c => new
                    {
                        BaseProvinceID = c.Long(nullable: false),
                        AdjacentProvinceID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseProvinceID, t.AdjacentProvinceID })
                .ForeignKey("dbo.Province", t => t.BaseProvinceID)
                .ForeignKey("dbo.Province", t => t.AdjacentProvinceID)
                .Index(t => t.BaseProvinceID)
                .Index(t => t.AdjacentProvinceID);
            
            CreateTable(
                "dbo.CityAdjacencies",
                c => new
                    {
                        BaseCityID = c.Long(nullable: false),
                        AdjacentCityID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseCityID, t.AdjacentCityID })
                .ForeignKey("dbo.City", t => t.BaseCityID)
                .ForeignKey("dbo.City", t => t.AdjacentCityID)
                .Index(t => t.BaseCityID)
                .Index(t => t.AdjacentCityID);
            
            CreateTable(
                "dbo.CityRegionAdjacencies",
                c => new
                    {
                        BaseCityRegionID = c.Long(nullable: false),
                        AdjacentCityRegionID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseCityRegionID, t.AdjacentCityRegionID })
                .ForeignKey("dbo.CityRegion", t => t.BaseCityRegionID)
                .ForeignKey("dbo.CityRegion", t => t.AdjacentCityRegionID)
                .Index(t => t.BaseCityRegionID)
                .Index(t => t.AdjacentCityRegionID);
            
            CreateTable(
                "dbo.NeighborhoodAdjacencies",
                c => new
                    {
                        BaseNeighborhoodID = c.Long(nullable: false),
                        AdjacentNeighborhoodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseNeighborhoodID, t.AdjacentNeighborhoodID })
                .ForeignKey("dbo.Neighborhood", t => t.BaseNeighborhoodID)
                .ForeignKey("dbo.Neighborhood", t => t.AdjacentNeighborhoodID)
                .Index(t => t.BaseNeighborhoodID)
                .Index(t => t.AdjacentNeighborhoodID);
            
            CreateTable(
                "dbo.NeighborhoodSynonyms",
                c => new
                    {
                        BaseNeighborhoodID = c.Long(nullable: false),
                        SynonymNeighborhoodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseNeighborhoodID, t.SynonymNeighborhoodID })
                .ForeignKey("dbo.Neighborhood", t => t.BaseNeighborhoodID)
                .ForeignKey("dbo.Neighborhood", t => t.SynonymNeighborhoodID)
                .Index(t => t.BaseNeighborhoodID)
                .Index(t => t.SynonymNeighborhoodID);
            
            AddColumn("dbo.Province", "CenterPoint", c => c.Geography());
            AddColumn("dbo.Province", "Boundary", c => c.Geography());
            AddColumn("dbo.City", "CenterPoint", c => c.Geography());
            AddColumn("dbo.City", "Boundary", c => c.Geography());
            AddColumn("dbo.Neighborhood", "CenterPoint", c => c.Geography());
            AddColumn("dbo.Neighborhood", "Boundary", c => c.Geography());
            AddColumn("dbo.CityRegion", "CenterPoint", c => c.Geography());
            AddColumn("dbo.CityRegion", "Boundary", c => c.Geography());
            AddColumn("dbo.Estate", "Address1", c => c.String(maxLength: 150));
            AddColumn("dbo.Estate", "Address2", c => c.String(maxLength: 150));
            AddColumn("dbo.Unit", "CurrentMonthlyChargeAmount", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.SaleConditions", "Price", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.SaleConditions", "PricePerEstateArea", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.SaleConditions", "PricePerUnitArea", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.SaleConditions", "MinimumMonthlyPaymentForDebt", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.SaleConditions", "TransferableLoanAmount", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.RentConditions", "Mortgage", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.RentConditions", "Rent", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.RentConditions", "MinimumMortgage", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.RentConditions", "MinimumRent", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.HttpSession", "GotInteractiveAck", c => c.Boolean(nullable: false));

			Sql("UPDATE [dbo].[Unit] SET CurrentMonthlyChargeAmount = CurrentMonthlyChargeK * 1000;");
			Sql("UPDATE [dbo].[SaleConditions] SET Price = PriceM * 1000000;");
			Sql("UPDATE [dbo].[SaleConditions] SET PricePerEstateArea = PricePerEstateAreaK * 1000;");
			Sql("UPDATE [dbo].[SaleConditions] SET PricePerUnitArea = PricePerUnitAreaK * 1000;");
			Sql("UPDATE [dbo].[SaleConditions] SET MinimumMonthlyPaymentForDebt = MinimumMonthlyPaymentForDebtK * 1000;");
			Sql("UPDATE [dbo].[SaleConditions] SET TransferableLoanAmount = TransferableLoanAmount * 1000000;");
			Sql("UPDATE [dbo].[RentConditions] SET Mortgage = MortgageM * 1000000;");
			Sql("UPDATE [dbo].[RentConditions] SET Rent = RentK * 1000;");
			Sql("UPDATE [dbo].[RentConditions] SET MinimumMortgage = MinimumMortgageM * 1000000;");
			Sql("UPDATE [dbo].[RentConditions] SET MinimumRent = MinimumRentK * 1000;");

			Sql("UPDATE [dbo].[Estate] SET [Address1] = CONCAT([Street1], ' ', [Street2]);");
			Sql("UPDATE [dbo].[Estate] SET [Address2] = CONCAT([Alley1], ' ', [Alley2]);");

			DropColumn("dbo.Estate", "Street1");
            DropColumn("dbo.Estate", "Street2");
            DropColumn("dbo.Estate", "Alley1");
            DropColumn("dbo.Estate", "Alley2");
            DropColumn("dbo.Unit", "CurrentMonthlyChargeK");
            DropColumn("dbo.SaleConditions", "PriceM");
            DropColumn("dbo.SaleConditions", "PricePerEstateAreaK");
            DropColumn("dbo.SaleConditions", "PricePerUnitAreaK");
            DropColumn("dbo.SaleConditions", "MinimumMonthlyPaymentForDebtK");
            DropColumn("dbo.SaleConditions", "TransferableLoanAmountM");
            DropColumn("dbo.RentConditions", "MortgageM");
            DropColumn("dbo.RentConditions", "RentK");
            DropColumn("dbo.RentConditions", "MinimumMortgageM");
            DropColumn("dbo.RentConditions", "MinimumRentK");
            DropColumn("dbo.User", "Email");
            DropColumn("dbo.User", "Phone1");
            DropColumn("dbo.User", "Phone2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "Phone2", c => c.String(maxLength: 25));
            AddColumn("dbo.User", "Phone1", c => c.String(maxLength: 25));
            AddColumn("dbo.User", "Email", c => c.String(maxLength: 120));
            AddColumn("dbo.RentConditions", "MinimumRentK", c => c.Decimal(precision: 12, scale: 3));
            AddColumn("dbo.RentConditions", "MinimumMortgageM", c => c.Decimal(precision: 12, scale: 6));
            AddColumn("dbo.RentConditions", "RentK", c => c.Decimal(precision: 12, scale: 3));
            AddColumn("dbo.RentConditions", "MortgageM", c => c.Decimal(precision: 12, scale: 6));
            AddColumn("dbo.SaleConditions", "TransferableLoanAmountM", c => c.Decimal(precision: 14, scale: 6));
            AddColumn("dbo.SaleConditions", "MinimumMonthlyPaymentForDebtK", c => c.Decimal(precision: 10, scale: 3));
            AddColumn("dbo.SaleConditions", "PricePerUnitAreaK", c => c.Decimal(precision: 10, scale: 3));
            AddColumn("dbo.SaleConditions", "PricePerEstateAreaK", c => c.Decimal(precision: 10, scale: 3));
            AddColumn("dbo.SaleConditions", "PriceM", c => c.Decimal(precision: 14, scale: 6));
            AddColumn("dbo.Unit", "CurrentMonthlyChargeK", c => c.Decimal(precision: 9, scale: 3));
            AddColumn("dbo.Estate", "Alley2", c => c.String(maxLength: 100));
            AddColumn("dbo.Estate", "Alley1", c => c.String(maxLength: 100));
            AddColumn("dbo.Estate", "Street2", c => c.String(maxLength: 100));
            AddColumn("dbo.Estate", "Street1", c => c.String(maxLength: 100));
            DropIndex("dbo.NeighborhoodSynonyms", new[] { "SynonymNeighborhoodID" });
            DropIndex("dbo.NeighborhoodSynonyms", new[] { "BaseNeighborhoodID" });
            DropIndex("dbo.NeighborhoodAdjacencies", new[] { "AdjacentNeighborhoodID" });
            DropIndex("dbo.NeighborhoodAdjacencies", new[] { "BaseNeighborhoodID" });
            DropIndex("dbo.CityRegionAdjacencies", new[] { "AdjacentCityRegionID" });
            DropIndex("dbo.CityRegionAdjacencies", new[] { "BaseCityRegionID" });
            DropIndex("dbo.CityAdjacencies", new[] { "AdjacentCityID" });
            DropIndex("dbo.CityAdjacencies", new[] { "BaseCityID" });
            DropIndex("dbo.ProvinceAdjacencies", new[] { "AdjacentProvinceID" });
            DropIndex("dbo.ProvinceAdjacencies", new[] { "BaseProvinceID" });
            DropForeignKey("dbo.NeighborhoodSynonyms", "SynonymNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodSynonyms", "BaseNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodAdjacencies", "AdjacentNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodAdjacencies", "BaseNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.CityRegionAdjacencies", "AdjacentCityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.CityRegionAdjacencies", "BaseCityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.CityAdjacencies", "AdjacentCityID", "dbo.City");
            DropForeignKey("dbo.CityAdjacencies", "BaseCityID", "dbo.City");
            DropForeignKey("dbo.ProvinceAdjacencies", "AdjacentProvinceID", "dbo.Province");
            DropForeignKey("dbo.ProvinceAdjacencies", "BaseProvinceID", "dbo.Province");
            DropColumn("dbo.HttpSession", "GotInteractiveAck");
            DropColumn("dbo.RentConditions", "MinimumRent");
            DropColumn("dbo.RentConditions", "MinimumMortgage");
            DropColumn("dbo.RentConditions", "Rent");
            DropColumn("dbo.RentConditions", "Mortgage");
            DropColumn("dbo.SaleConditions", "TransferableLoanAmount");
            DropColumn("dbo.SaleConditions", "MinimumMonthlyPaymentForDebt");
            DropColumn("dbo.SaleConditions", "PricePerUnitArea");
            DropColumn("dbo.SaleConditions", "PricePerEstateArea");
            DropColumn("dbo.SaleConditions", "Price");
            DropColumn("dbo.Unit", "CurrentMonthlyChargeAmount");
            DropColumn("dbo.Estate", "Address2");
            DropColumn("dbo.Estate", "Address1");
            DropColumn("dbo.CityRegion", "Boundary");
            DropColumn("dbo.CityRegion", "CenterPoint");
            DropColumn("dbo.Neighborhood", "Boundary");
            DropColumn("dbo.Neighborhood", "CenterPoint");
            DropColumn("dbo.City", "Boundary");
            DropColumn("dbo.City", "CenterPoint");
            DropColumn("dbo.Province", "Boundary");
            DropColumn("dbo.Province", "CenterPoint");
            DropTable("dbo.NeighborhoodSynonyms");
            DropTable("dbo.NeighborhoodAdjacencies");
            DropTable("dbo.CityRegionAdjacencies");
            DropTable("dbo.CityAdjacencies");
            DropTable("dbo.ProvinceAdjacencies");
        }
    }
}
