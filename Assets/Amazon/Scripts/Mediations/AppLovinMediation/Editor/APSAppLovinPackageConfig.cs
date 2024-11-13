public class APSAppLovinPackageConfig : AmazonPackageConfig
{
    public const string VERSION = "1.9.0";

    public override string Name
    {
        get { return "applovin_max"; }
    }

    public override string Version
    {
        get { return VERSION; }
    }
}
