namespace DotnetURWay;

public abstract class BaseService
{
    private readonly string _endpoint = "URWAYPGService/transaction/jsonProcess/JSONrequest";
    
        /**
         * @return string
         */
        public string GetEndPointPath(bool isProduction)
        {
            return GetBasePath(isProduction) + "/" + _endpoint;
        }

        public string GetBasePath(bool isProduction)
        {
            // production
            if (isProduction)
            {
                return "https://payments.urway-tech.com";
            }
            
            // testing
            return "https://payments-dev.urway-tech.com";
        }
    }