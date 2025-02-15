using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using PowerManagement.CDK.Builders;

namespace PowerManagement.CDK;

public sealed class PowerManagementCdkStack : Stack
{
    private const string FunctionName = "PowerManagement-Lambda";
    private const string LambdaHandler = "PowerManagement.Lambda::PowerManagement.Lambda.Function::FunctionHandler";
    private const string LambdaZipName = "PowerManagement.Lambda.zip";

    public PowerManagementCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var lambda = LambdaBuilder.CreateLambda(this, LambdaZipName, FunctionName, LambdaHandler)
                                  .AddEnvironment("AWS_LAMBDA_INITIALIZATION_TYPE", "snap-start");
        var cfnLambda = (CfnFunction)lambda.Node.DefaultChild;
        cfnLambda.SnapStart = new CfnFunction.SnapStartProperty
        {
            ApplyOn = "PublishedVersions"
        };
        var version = lambda.CurrentVersion;
        
        CreateCfnOutput(lambda);

        var restApi = ApiGatewayBuilder.CreateRestApi(this, "PowerManagement-Api",
            "PowerManagement-Lambda-Api", lambda);
        CreateCfnOutput(restApi);
    }

    private void CreateCfnOutput(IClientVpnConnectionHandler lambda)
    {
        _ = new CfnOutput(this, "ApiMiddlewareLambdaUserArn", new CfnOutputProps
        {
            Value = lambda.FunctionArn
        });
    }

    private void CreateCfnOutput(IRestApi restApi)
    {
        _ = new CfnOutput(this, "ApiGatewayRestApiId", new CfnOutputProps
        {
            Value = restApi.RestApiId
        });
    }
}