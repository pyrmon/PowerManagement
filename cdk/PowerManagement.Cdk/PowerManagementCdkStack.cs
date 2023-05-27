using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Constructs;
using PowerManagement.CDK.Builders;
using Stage = Amazon.CDK.AWS.APIGateway.Stage;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

namespace PowerManagement.CDK;

public sealed class PowerManagementCdkStack : Stack
{
    private const string FunctionName = "PowerManagement-Function";
    private const string LambdaHandler = "PowerManagement.Lambda::PowerManagement.Lambda.Function::FunctionHandler";
    private const string LambdaZipName = $"PowerManagement.Lambda.zip";

    public PowerManagementCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var lambda = LambdaBuilder.CreateLambda(this, LambdaZipName, FunctionName, LambdaHandler);
        CreateCfnOutput(lambda);
        _ = SsmBuilder.CreateSshConnectionPlanParameter(this, "/ssh/actionPlan");
        var restApi = ApiGatewayBuilder.CreateRestApi(this, "PowerManagement-Second-Api",
            "PowerManagement-Function-Api", lambda);
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