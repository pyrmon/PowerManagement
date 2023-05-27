# PowerManagement

Lambda Function to run SSH Requests

I can't create secure strings via CDK, so I will create the Raspi Key via CLI in Github Actions:

```bash
aws ssm put-parameter \
--name "/raspi/private" \
--value "your-secret-value" \
--type SecureString \
--overwrite
```