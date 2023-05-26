using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using PowerManagement.CDK.Extensions;

namespace PowerManagement.CDK;

public class PowerManagementCdkStack : Stack
{
    private const string FunctionName = "PowerManagement-Function";
    private const string LambdaHandler = "PowerManagement.Lambda";
    private const string LambdaZipName = $"{LambdaHandler}.zip";

    public PowerManagementCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var lambda = CreateLambda();
        CreateCfnOutput(lambda);
    }

    private Function CreateLambda()
    {
        var code = Code.FromAsset(LambdaZipName);
        var role = CreateLambdaRole();
        role.AddManagedPolicy();
        return new Function(this, FunctionName, new FunctionProps
        {
            Code = code,
            Runtime = Runtime.DOTNET_6,
            FunctionName = FunctionName,
            Handler = LambdaHandler,
            MemorySize = 128,
            Timeout = Duration.Seconds(180),
            LogRetention = RetentionDays.ONE_WEEK,
            Role = role,
            Architecture = Architecture.X86_64,
            Tracing = Tracing.ACTIVE
        });
    }

    private Role CreateLambdaRole()
    {
        return new Role(this, $"{FunctionName}-Role", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            RoleName = $"{FunctionName}-ServiceRole",
            InlinePolicies = new Dictionary<string, PolicyDocument>
            {
                {
                    $"{FunctionName}-Policy", new PolicyDocument(new PolicyDocumentProps
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

    private void CreateCfnOutput(IClientVpnConnectionHandler lambda)
    {
        _ = new CfnOutput(this, "ApiMiddlewareLambdaUserArn", new CfnOutputProps
        {
            Value = lambda.FunctionArn
        });
    }
}