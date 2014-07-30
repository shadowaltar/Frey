namespace Maintenance.CountryOverrides.Entities
{
    /// <summary>
    /// Indicates which field has a filter applied.
    /// </summary>
    public enum FilterOptionType
    {
        None,
        OverrideType,
        StartOfId,
        StartOfName,
        OriginalCountry,
        NewCountry,
        StartOfCusip,
        StartOfSedol,
        PortfolioManagerId,
        PortfolioId,
    }
}