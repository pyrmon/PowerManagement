using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using PowerManagement.CDK.Extensions;

namespace PowerManagement.CDK.Builders;

public static class LambdaBuilder
{
    public static Function CreateLambda(Construct scope, string lambdaZipName, string functionName,
        string lambdaHandler)
    {
        var code = Code.FromAsset(lambdaZipName);
        var role = CreateLambdaRole(scope, functionName);
        role.AddManagedPolicy();
        return new Function(scope, functionName, new FunctionProps
        {
            Code = code,
            Runtime = Runtime.DOTNET_6,
            FunctionName = functionName,
            Handler = lambdaHandler,
            MemorySize = 128,
            Timeout = Duration.Seconds(180),
            LogRetention = RetentionDays.ONE_WEEK,
            Role = role,
            Architecture = Architecture.X86_64,
            Tracing = Tracing.ACTIVE
        });
    }

    private static Role CreateLambdaRole(Construct scope, string functionName)
    {
        return new Role(scope, $"{functionName}-Role", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            RoleName = $"{functionName}-ServiceRole",
            InlinePolicies = new Dictionary<string, PolicyDocument>
            {
                {
                    $"{functionName}-Policy", new PolicyDocument(new PolicyDocumentProps
                    {
                        Statements = new[]
                        {
                            new PolicyStatement(new PolicyStatementProps
                            {
                                Actions = CreateAllowedActions(),
                                Resources = new[] { "*" },
                                Effect = Effect.ALLOW
                            })
                        }
                    })
                }
            }
        });
    }

    private static string[] CreateAllowedActions()
    {
        return new[]
        {
            "lambda:InvokeFunction",
            "logs:CreateLogGroup",
            "logs:CreateLogStream",
            "logs:PutLogEvents",
            "ssm:GetParametersByPath",
            "ssm:GetParameter",
            "kms:DecryptKey"
        };
    }
}