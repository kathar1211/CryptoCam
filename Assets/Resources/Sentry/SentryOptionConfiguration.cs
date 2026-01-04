using Sentry.Unity;

public class SentryOptionConfiguration : SentryOptionsConfiguration
{
    const string DSN = "https://498df0aa239f5318bc6c57353c5ef0a1@o4510653008510976.ingest.us.sentry.io/4510653014540288";
    private bool enabled;

    public override void Configure(SentryUnityOptions options)
    {
       // options.Dsn = enabled ? DSN : null;
    }

    public void EnableSentry(bool enable)
    {
        enabled = enable;
    }
}