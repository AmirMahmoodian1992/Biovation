namespace DataAccessLayerCore.Domain
{
    /// <summary>
    /// Fetch Type of a mapping Operation. LAZY if fetching of nesting object is not required in each transaction. EAGER force DAL to load the nesting components.
    /// Dear Friend, it doesn't work right now, maybe we add it later. if you need it you can add it in GenericRepository yourself :)
    /// </summary>
    public enum FetchType
    {
        LAZY = 1,
        EAGER = 0
    }
}
