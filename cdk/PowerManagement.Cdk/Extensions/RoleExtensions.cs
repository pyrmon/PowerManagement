using Amazon.CDK.AWS.IAM;

namespace PowerManagement.CDK.Extensions
{
    public static class RoleExtensions
    {
        public static void AddManagedPolicy(this Role role)
        {
            role.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSLambdaExecute"));
        }
    }
}
