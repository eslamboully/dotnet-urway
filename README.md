# Dotnet URWAY Payment Gateway

<h3>Step 1: Installation</h3>

<code>dotnet add package eslamboully.DotnetURWay</code>

<h3>Or you can search <code>eslamboully.DotnetURWay</code> in nuget libraries<h3>

<hr/>

<h3>Step 2: appsettings.Development.json</h3>

<code>

        "URWay": {
            "terminalId": "your_terminal_id",
            "password": "your_password",
            "merchantKey": "your_merchant_key",
            "redirectUrl": "your_redirect_url_after_payment",
            "isProduction": false
        }
</code>

<h5>don't forget to change <code>isProduction: true</code> on live mode</h5>
<hr>
<h3>Step 3: Usage</h3>

<code>

        var client = new DotnetURWay.HttpClient(_config);

        client = client.SetTrackId((new Random()).Next().ToString())
                    .SetCustomerEmail(email)
                    .SetCustomerIp("10.10.10.10")
                    .SetMerchantIp("10.10.10.10")
                    .SetCurrency("SAR")
                    .SetCountry("SA")
                    .SetAmount(amount)
                    .SetRedirectUrl(_config.GetSection("URWay")["redirectUrl"]);

        // if you want to redirect to payment page
        return Redirect(client.Pay().Result.GetPaymentUrl());

        // if you want to send the payment url to another client
        return Ok(new
        {
            redirectLink = client.Pay().Result.GetPaymentUrl()
        });
</code>

<h5>And on callback :</h5>

<code>

        var client = new DotnetURWay.HttpClient(_config);

        client = client.SetTrackId(Request.Form["trackid"])
                    .SetCustomerEmail(email)
                    .SetCustomerIp("10.10.10.10")
                    .SetMerchantIp("10.10.10.10")
                    .SetCurrency("SAR")
                    .SetCountry("SA")
                    .SetAmount(Request.Form["amount"]);

        if (response.IsSuccess())
        {
            //
        }

        if (response.IsFailure()) {
            //
        }

        return BadRequest("Something Error !");
</code>