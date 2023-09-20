using System.Collections.Generic;
using System.Text;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Scheduler;
using Constructs;

namespace PowerManagement.CDK.Builders;

public static class EventBridgeBuilder
{
    public static CfnSchedule CreateEventSchedule(Construct scope)
    {
        return new CfnSchedule(scope, "PowerManagement-Lambda-Bitwarden-EventSchedule", new CfnScheduleProps
        {
            ScheduleExpression = "cron(0 30 12 * ? *)",
            State = "ENABLED"
        });
    }

    public static Rule CreateNewEventRule(Construct scope, string id, IFunction function)
    {
        return new Rule(scope, "EventBridgeRule", new RuleProps
        {
            EventPattern = new EventPattern
            {
                Source = new[] { "aws.scheduler" },
                Id = new[] { id }
            },
            Targets = new IRuleTarget[]
            {
                new LambdaFunction(function, new LambdaFunctionProps
                {
                    Event = RuleTargetInput.FromText("{ \"version\": \"2.0\", \"http\": { \"method\": \"POST\", \"path\": \"/power\" }, \"body\": { \\\"request\\\": \\\"bitwarden_backup\\\" } }")
                })
            },
            Enabled = true,
            RuleName = "PowerManagement-Lambda-Bitwarden-EventRule",
            Description = "Cron Job to Backup Bitwarden"
        });
    }
}