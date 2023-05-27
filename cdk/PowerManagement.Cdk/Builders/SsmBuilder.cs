using Amazon.CDK.AWS.SSM;
using Constructs;
using CfnParameter = Amazon.CDK.CfnParameter;
using CfnParameterProps = Amazon.CDK.CfnParameterProps;

namespace PowerManagement.CDK.Builders;

public static class SsmBuilder
{
    public static StringParameter CreateSshConnectionPlanParameter(Construct scope, string parameterName)
    {
        var value = new CfnParameter(scope, "SshActionPlan", new CfnParameterProps
        {
            Type = "String",
            Description = "This is the Json that will define what will be done per keyword with the Ssh requests"
        });
        var cfnParameter = new StringParameter(scope, parameterName, new StringParameterProps
        {
            ParameterName = parameterName,
            StringValue = value.ValueAsString
        });
        return cfnParameter;
    }
}