namespace Trading.CountryOverrides.Entities
{
    public enum ItemPrecedence
    {
        /// <summary>
        /// Status that no other override entries of the same security exists.
        /// </summary>
        Normal,

        /// <summary>
        /// Status that the override entry is of the highest precedence.
        /// </summary>
        Final,

        /// <summary>
        /// Status that the override entry will never be observed as higher precedence entry exists.
        /// </summary>
        Overridden,

        /// <summary>
        /// Status that the override entry is observed by some users while isn't by others, especially
        /// for those FILC / ALL / PM entries which also have PORTFOLIO siblings.
        /// </summary>
        Ambiguous,
    }
}