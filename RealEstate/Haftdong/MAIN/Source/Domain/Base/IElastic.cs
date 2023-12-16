namespace JahanJooy.RealEstateAgency.Domain.Base
{
    public interface IElastic<out T>
    {
        T GetIe();
    }
}