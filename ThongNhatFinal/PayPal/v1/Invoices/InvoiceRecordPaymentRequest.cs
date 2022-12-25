// This class was generated on Tue, 30 Jan 2018 11:02:40 PST by version 0.1.0-dev+6beb51-dirty of Braintree SDK Generator
// InvoiceRecordPaymentRequest.cs
// @version 0.1.0-dev+6beb51-dirty
// @type request
// @data H4sIAAAAAAAC/7xWX08ctxd9/32KK+cFVsMOCb+26r5RIAptgW1AVSuEkrv2HcbBYzv2HegU8d0rz8zOMrtBaRXE2+rMtfecc//43otTrEjMhLa3TkuaBpIuqB2PTUWWRSYOKcqgPWtnxUycYLiJwCVBZOQ6gisALfSnM1g0cHyYAUbwqNUUjq00tSJA6G8ERYzagFt8IsnAJSao0Ja6a5dhFXHpFKBV4Lik0J+LoG0b9/P52SkE+lxTZFg41UxFJn6rKTRzDFgRU4hidnmViXeEisI6+taFah2bI5cj7F5cND65Ezloey0y8TsGjQtDY9c+aCUy8Qs1Pbxh20VJcHyYzErc+1PADioMN4NbIhP7IWDT/eluJt4TqjNrGjEr0ERKwOdaB1JixqGmTMyD8xRYUxQzWxvzcNXFUOTukkHBvPf1sLVxU0mycKShB8Y65qMkxv/EuAceU14ZfFCHQFY2m8SwcnVbiCtqAzQm9xNGWtZV4QKgMVBoi1ZqNHCLpiYIZJBJQaHJqAhbCzRoU+EO5VlTBsRyuv1s2p4qHtlrPnCKRvrkyoyxwj0wxEwBlhEgXWqu2LeQavvv/Az+/+b1D9+anLaesq+qaH0d0V8iY+5d0qD2qexPQelrzYBFUpN6QpHUFZoIkTwG5JS/lSxtB1ltatMJ9D44HzQyjf14lj76F8oV8lh4D2y2fvoAdyXZUfvfDVNyiClcqJBBR5g0TdNMdiYnJ5OdiVITmPw9ydYsuTy2TMESwyEy5Re6Injb3nC1VTL7WZ6zcyZONXExdeE6L7kyeSjk3t7ej68iycRx57vp99svVCzdVB+ZNkCbtg1vQSpyF/o3YQpLJnCnuWwt/Xj0x8XR+9P9Xz8Oh7jxz1IJXxdl3Vod9MBY0Fn7A80U9iEFAMbopG6n0aCjJ/9C2eCANmJbBOsv2ManL75n7ZSFOTZzNCvjV0e/mKr5/p/zb0rU8whOf/uk5P7jpuhHQS9KfIPsUwQfu5pmxLCcaXsNhXF33bS57PY8CFTUVl1t5crJmKPX+RCdv+oHVfzQL4Vd8PZyN4u19y5wXO/ALqyzp/2zIeWPvqQZ11+QnmMXYIHy5g6DAukqj6wX2mhunsXgq4cUFb2zkbp7EpyJA2eZbL8qCfTeaInJzPxTdFZk4h2zP+kG1EzMz84vRLclipnIb18/8mppVX6/Wgkf8o1l+ugvT5JJnbfLc/vwz97s7j787x8AAAD//w==
// DO NOT EDIT
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;


namespace PayPal.v1.Invoices
{
    /// <summary>
    /// Marks the status of an invoice, by ID, as paid. Include a payment detail object that defines the payment method and other details in the JSON request body.
    /// </summary>
    public class InvoiceRecordPaymentRequest : HttpRequest
    {
        public InvoiceRecordPaymentRequest(string InvoiceId) : base("/v1/invoicing/invoices/{invoice_id}/record-payment?", HttpMethod.Post, typeof(void))
        {
            try {
                this.Path = this.Path.Replace("{invoice_id}", Uri.EscapeDataString(Convert.ToString(InvoiceId) ));
            } catch (IOException) {}
            
            this.ContentType =  "application/json";
        }
        
        public InvoiceRecordPaymentRequest RequestBody(PaymentDetail Body)
        {
            this.Body = Body;
            return this;
        }
    }
}
