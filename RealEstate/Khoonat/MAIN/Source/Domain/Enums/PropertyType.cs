namespace JahanJooy.RealEstate.Domain.Enums
{
	public enum PropertyType : byte
	{
		Land = 1,
		Garden = 2,
		House = 101,
		Villa = 102,
		Apartment = 201,
		Penthouse = 202,
        Shop = 203
	}

	public static class PropertyTypeExtensions
	{
		public static bool IsEstateSignificant(this PropertyType? type)
		{
			return type.HasValue && type.Value.IsEstateSignificant();
		}

		public static bool IsSingleUnitBuilding(this PropertyType? type)
		{
			return type.HasValue && type.Value.IsSingleUnitBuilding();
		}

		public static bool HasBuilding(this PropertyType? type)
		{
			return type.HasValue && type.Value.HasBuilding();
		}

		public static bool HasUnit(this PropertyType? type)
		{
			return type.HasValue && type.Value.HasUnit();
		}

		public static bool IsEstateSignificant(this PropertyType type)
		{
			return type == PropertyType.Land ||
			       type == PropertyType.Garden ||
			       type == PropertyType.House ||
			       type == PropertyType.Villa;
		}

		public static bool IsSingleUnitBuilding(this PropertyType type)
		{
			return type == PropertyType.House ||
			       type == PropertyType.Villa||
                   type == PropertyType.Shop;
		}

		public static bool HasBuilding(this PropertyType type)
		{
			return type == PropertyType.House ||
				   type == PropertyType.Villa ||
				   type == PropertyType.Apartment ||
				   type == PropertyType.Penthouse ||
                   type == PropertyType.Shop;
		}

		public static bool HasUnit(this PropertyType type)
		{
			return type == PropertyType.House ||
				   type == PropertyType.Villa ||
				   type == PropertyType.Apartment ||
				   type == PropertyType.Penthouse ||
                   type == PropertyType.Shop;
		}
	}
}