using Amazon.CDK.AWS.Events;
using Constructs;

namespace PowerManagement.CDK.Builders;

public static class EventBridgeBuilder
{
    public static Rule CreateNewEventRule(Construct scope)
    {
        return new Rule(scope, "EventBridgeRule", new RuleProps
        {
            Schedule = Schedule.Cron(new CronOptions
            {
                Minute = "30",
                Hour = "14",
                WeekDay = "4"
            }),
            RuleName = "PowerManagement-Lambda-Bitwarden-EventRule",
            Description = "Cron Job to Backup Bitwarden"
        });
    }
}