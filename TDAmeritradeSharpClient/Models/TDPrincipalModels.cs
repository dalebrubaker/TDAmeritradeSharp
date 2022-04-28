namespace TDAmeritradeSharpClient;

[Serializable]
public class TDStreamerInfo
{
    public string? StreamerBinaryUrl { get; set; }
    public string? StreamerSocketUrl { get; set; }
    public string? Token { get; set; }
    public string? TokenTimestamp { get; set; }
    public string? UserGroup { get; set; }
    public string? AccessLevel { get; set; }
    public string? Acl { get; set; }
    public string? AppId { get; set; }
}

[Serializable]
public class TDQuotes
{
    public bool IsNyseDelayed { get; set; }
    public bool IsNasdaqDelayed { get; set; }
    public bool IsOpraDelayed { get; set; }
    public bool IsAmexDelayed { get; set; }
    public bool IsCmeDelayed { get; set; }
    public bool IsIceDelayed { get; set; }
    public bool IsForexDelayed { get; set; }
}

[Serializable]
public class TDKey
{
    public string? Key { get; set; }
}

[Serializable]
public class TDStreamerSubscriptionKeys
{
    public List<TDKey>? Keys { get; set; }
}

[Serializable]
public class TDPreferences
{
    public bool ExpressTrading { get; set; }
    public bool DirectOptionsRouting { get; set; }
    public bool DirectEquityRouting { get; set; }
    public string? DefaultEquityOrderLegInstruction { get; set; }
    public string? DefaultEquityOrderType { get; set; }
    public string? DefaultEquityOrderPriceLinkType { get; set; }
    public string? DefaultEquityOrderDuration { get; set; }
    public string? DefaultEquityOrderMarketSession { get; set; }
    public int DefaultEquityQuantity { get; set; }
    public string? MutualFundTaxLotMethod { get; set; }
    public string? OptionTaxLotMethod { get; set; }
    public string? EquityTaxLotMethod { get; set; }
    public string? DefaultAdvancedToolLaunch { get; set; }
    public string? AuthTokenTimeout { get; set; }
}                

[Serializable]
public class TDAuthorizations
{
    public bool Apex { get; set; }
    public bool LevelTwoQuotes { get; set; }
    public bool StockTrading { get; set; }
    public bool MarginTrading { get; set; }
    public bool StreamingNews { get; set; }
    public string? OptionTradingLevel { get; set; }
    public bool StreamerAccess { get; set; }
    public bool AdvancedMargin { get; set; }
    public bool ScottradeAccount { get; set; }
}

[Serializable]
public class TDPrincipalAccount
{
    public string? AccountId { get; set; }
    public string? Description { get; set; }
    public string? DisplayName { get; set; }
    public string? AccountCdDomainId { get; set; }
    public string? Company { get; set; }
    public string? Segment { get; set; }
    public string? SurrogateIds { get; set; }
    public TDPreferences? Preferences { get; set; }
    public string? Acl { get; set; }
    public TDAuthorizations? Authorizations { get; set; }
}

[Serializable]
public class TDPrincipal
{
    public string? AuthToken { get; set; }
    public string? UserId { get; set; }
    public string? UserCdDomainId { get; set; }
    public string? PrimaryAccountId { get; set; }
    public string? LastLoginTime { get; set; }
    public string? TokenExpirationTime { get; set; }
    public string? LoginTime { get; set; }
    public string? AccessLevel { get; set; }
    public bool StalePassword { get; set; }
    public TDStreamerInfo? StreamerInfo { get; set; }
    public string? ProfessionalStatus { get; set; }
    public TDQuotes? Quotes { get; set; }
    public TDStreamerSubscriptionKeys? StreamerSubscriptionKeys { get; set; }
    public List<TDPrincipalAccount>? Accounts { get; set; }
}