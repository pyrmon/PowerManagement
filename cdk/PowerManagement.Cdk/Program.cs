using Amazon.CDK;

namespace PowerManagement.CDK;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        _ = new PowerManagementCdkStack(app, nameof(PowerManagementCdkStack), new StackProps());
        app.Synth();
    }
}