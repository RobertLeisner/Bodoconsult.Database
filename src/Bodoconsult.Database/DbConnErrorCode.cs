namespace Bodoconsult.Database
{
    public enum DbConnErrorCode : int
    {
        // failure
        ERC_ERROR = 10,
        ERC_NOTIMPLEMENTED = 11,
        ERC_UNSUPPORTEDPROVIDER = 12,
        ERC_UDLNOTFOUND = 13,
        ERC_ASYNCNOTPOSSIBLE = 14
    }
}