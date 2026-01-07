using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sentry.Unity;
using Sentry;

public static class SentryUtils 
{

    private const string DSN = "https://498df0aa239f5318bc6c57353c5ef0a1@o4510653008510976.ingest.us.sentry.io/4510653014540288";
    public static void EnableSentry()
    {
        SentrySdk.Init(options =>
        {
            options.Dsn = DSN;
        });
    }

    public static void DisableSentry()
    {
        SentrySdk.Init(options =>
        {
            options.Dsn = "";
        });
    }
}
