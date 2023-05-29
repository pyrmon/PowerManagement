using System.Collections.Generic;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace PowerManagement.CDK.Builders;

public static class ApiGatewayBuilder
{
    public static RestApi CreateRestApi(Construct scope, string name, string description, IFunction lambda)
    {
        // Create an API Gateway REST API
        var restApi = new RestApi(scope, name, new RestApiProps
        {
            RestApiName = description,
            EndpointTypes = new[] { EndpointType.REGIONAL }
        });

        // Create a resource under the REST API
        var proxyResource = restApi.Root.AddResource("{proxy+}");

        // Create a method for the resource
        _ = proxyResource.AddMethod("ANY", new LambdaIntegration(lambda), new MethodOptions
        {
            ApiKeyRequired = true,
            RequestParameters = new Dictionary<string, bool>
            {
                { "method.request.path.proxy", true }
            }
        });

        // Create an API Gateway API key
        var apiKey = new ApiKey(scope, "PowerManagementApiKey", new ApiKeyProps
        {
            ApiKeyName = $"{name}-Key"
        });

        // Create an API Gateway deployment
        var deployment = new Deployment(scope, "PowerManagement-Api-Deployment", new DeploymentProps
        {
            Api = restApi,
            Description = "Deployment for the PowerManagement REST API"
        });

        // Create a stage for the deployment
        var stage = new Stage(scope, "PowerManagement-Api-Stage", new StageProps
        {
            Deployment = deployment,
            StageName = "production"
        });


        var usagePlan = new UsagePlan(scope, "PowerManagement-Api-Usage-Plan", new UsagePlanProps
        {
            Name = "PowerManagement-Second-Api-Usage-Plan",
            ApiStages = new IUsagePlanPerApiStage[]
            {
                new UsagePlanPerApiStage
                {
                    Api = restApi,
                    Stage = stage
                }
            }
        });

        // Associate the API key with the usage plan
        usagePlan.AddApiKey(apiKey);

        // Add permissions for API Gateway to invoke the Lambda function
        lambda.GrantInvoke(new ServicePrincipal("apigateway.amazonaws.com"));

        return restApi;
    }
}