﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using NUnit.Framework;

namespace Flurl.Test.Http
{
	[TestFixture, Parallelizable]
    class FlurlHttpExceptionTests : HttpTestFixtureBase
    {
	    [Test]
	    public async Task exception_message_is_nice() {
		    HttpTest.RespondWithJson(new { message = "bad data!" }, 400);

		    try {
			    await "http://myapi.com".PostJsonAsync(new { data = "bad" });
			    Assert.Fail("should have thrown 400.");
		    }
		    catch (FlurlHttpException ex) {
			    Assert.AreEqual("Call failed with status code 400 (Bad Request): POST http://myapi.com", ex.Message);
		    }
	    }

	    [Test]
	    public async Task exception_message_excludes_request_response_labels_when_body_empty() {
		    HttpTest.RespondWith("", 400);

		    try {
			    await "http://myapi.com".GetAsync();
			    Assert.Fail("should have thrown 400.");
		    }
		    catch (FlurlHttpException ex) {
				// no "Request body:", "Response body:", or line breaks
			    Assert.AreEqual("Call failed with status code 400 (Bad Request): GET http://myapi.com", ex.Message);
		    }
	    }

	    [Test]
	    public async Task can_catch_parsing_error() {
		    HttpTest.RespondWith("I'm not JSON!");

		    try {
			    await "http://myapi.com".GetJsonAsync();
			    Assert.Fail("should have failed to parse response.");
		    }
		    catch (FlurlParsingException ex) {
			    Assert.AreEqual("Response could not be deserialized to JSON: GET http://myapi.com", ex.Message);
			    Assert.AreEqual("I'm not JSON!", await ex.GetResponseStringAsync());
				// will differ if you're using a different serializer (which you probably aren't):
			    Assert.IsInstanceOf<Newtonsoft.Json.JsonReaderException>(ex.InnerException);
		    }
		}
	}
}
