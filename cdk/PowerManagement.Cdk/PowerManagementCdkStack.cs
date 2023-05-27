using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Constructs;
using PowerManagement.CDK.Builders;

namespace PowerManagement.CDK;

public sealed class PowerManagementCdkStack : Stack
{
    private const string FunctionName = "PowerManagement-Function";
    private const string LambdaHandler = "PowerManagement.Lambda";
    private const string LambdaZipName = $"{LambdaHandler}.zip";

    public PowerManagementCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var lambda = LambdaBuilder.CreateLambda(this, LambdaZipName, FunctionName, LambdaHandler);
        CreateCfnOutput(lambda);
        _ = SsmBuilder.CreateSshConnectionPlanParameter(this, "/ssh/actionPlan");
    }

    private void CreateCfnOutput(IClientVpnConnectionHandler lambda)
    {
        _ = new CfnOutput(this, "ApiMiddlewareLambdaUserArn", new CfnOutputProps
        {
            Value = lambda.FunctionArn
        });
    }
}