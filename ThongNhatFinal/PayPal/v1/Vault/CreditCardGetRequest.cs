// This class was generated on Tue, 30 Jan 2018 11:02:44 PST by version 0.1.0-dev+6beb51-dirty of Braintree SDK Generator
// CreditCardGetRequest.cs
// @version 0.1.0-dev+6beb51-dirty
// @type request
// @data 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
// DO NOT EDIT
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;


namespace PayPal.v1.Vault
{
    /// <summary>
    /// Shows details for a vaulted credit card, by ID.
    /// </summary>
    public class CreditCardGetRequest : HttpRequest
    {
        public CreditCardGetRequest(string CreditCardId) : base("/v1/vault/credit-cards/{credit_card_id}?", HttpMethod.Get, typeof(CreditCard))
        {
            try {
                this.Path = this.Path.Replace("{credit_card_id}", Uri.EscapeDataString(Convert.ToString(CreditCardId) ));
            } catch (IOException) {}
            
            this.ContentType =  "application/json";
        }
        
    }
}
