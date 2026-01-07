using Sentry.Unity;

public class SentryOptionConfiguration : SentryOptionsConfiguration
{
    public bool ShouldCaptureEvents { get; private set; }

    public override void Configure(SentryUnityOptions options)
    {
        // Here you can programmatically modify the Sentry option properties used for the SDK's initialization
        options.SetBeforeSend((sentryEvent, hint) =>
        {
            if (ShouldCaptureEvents) { return sentryEvent; }
            else { return null; }
        });
    }

    public void EnableSentry()
    {
        ShouldCaptureEvents = true;
    }

    public void DisableSentry()
    {
        ShouldCaptureEvents = false;
    }
}