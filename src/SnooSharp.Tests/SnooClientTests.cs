using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace SnooSharp.Tests;

public class SnooClientTests
{
    [Fact]
    public async Task WhenMakingMultipleCallsTokenIsRetrievedOnce()
    {
        var fixture = new Fixture
        {
            RepeatCount = 3
        };
        var st = new DateTime(2022, 08, 05, 08,00,00);
        var et = new DateTime(2022, 08, 06, 08,00,00);
        
        var opts = fixture.Create<SnooClientOptions>();
        var token = fixture.Build<TokenResponse>().With(o => o.expires_in, 300).Create();
        var stRes = fixture.Create<DataResponse>();
        var etRes = fixture.Create<DataResponse>();
        var mockMessageHandler = new Mock<HttpMessageHandler>();

        var tokenCalls = 0;
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>o.RequestUri!.ToString().Contains(Constants.UrlParts.LoginEndpoint))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(token)
            }).Callback(()=>tokenCalls++);
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>
                    o.RequestUri!.ToString().Contains(Constants.UrlParts.DataEndpoint)
                    && o.RequestUri!.ToString().Contains(st.ToString("MM/dd/yyyy")))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(stRes)
            });
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>
                    o.RequestUri!.ToString().Contains(Constants.UrlParts.DataEndpoint)
                    && o.RequestUri!.ToString().Contains(et.ToString("MM/dd/yyyy")))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(etRes)
            });
        
        var httpClient = new HttpClient(mockMessageHandler.Object);
        var snooClient = new SnooClient(httpClient,opts);
        
        var dataResponses = await snooClient.GetDataResponses(st, et);
        var responses = dataResponses as DataResponse[] ?? dataResponses.ToArray();

        mockMessageHandler.Verify();

        responses.Should().ContainEquivalentOf(stRes);
        responses.Should().ContainEquivalentOf(etRes);
        responses.Count().Should().Be(2);
        tokenCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetDataFails()
    {
        var fixture = new Fixture
        {
            RepeatCount = 3
        };
        var st = new DateTime(2022, 08, 05, 08,00,00);
        
        var opts = fixture.Create<SnooClientOptions>();
        var token = fixture.Build<TokenResponse>().With(o => o.expires_in, 300).Create();
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>o.RequestUri!.ToString().Contains(Constants.UrlParts.LoginEndpoint))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(token)
            });
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>
                    o.RequestUri!.ToString().Contains(Constants.UrlParts.DataEndpoint)
                    && o.RequestUri!.ToString().Contains(st.ToString("MM/dd/yyyy")))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("uh oh")
            });
        
        var httpClient = new HttpClient(mockMessageHandler.Object);
        var snooClient = new SnooClient(httpClient,opts);
        
        Func<Task> dataResponses = async ()=> await snooClient.GetData(st);
        
        mockMessageHandler.Verify();

        await dataResponses.Should().ThrowAsync<Exception>().WithMessage("uh oh");
    }
    
    
    
        [Fact]
    public async Task TokenFailsSuccessfully()
    {
        var fixture = new Fixture
        {
            RepeatCount = 3
        };
        var st = new DateTime(2022, 08, 05, 08,00,00);
        var et = new DateTime(2022, 08, 06, 08,00,00);
        
        var opts = fixture.Create<SnooClientOptions>();
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var message = "bad times";
        
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync"
                , ItExpr.Is<HttpRequestMessage>(o=>o.RequestUri!.ToString().Contains(Constants.UrlParts.LoginEndpoint))
                , ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.BadGateway,
                Content = new StringContent(message)
            });

        var httpClient = new HttpClient(mockMessageHandler.Object);
        var snooClient = new SnooClient(httpClient,opts);
        
        Func<Task> call = async ()=> await snooClient.GetDataResponses(st, et);

        await call.Should().ThrowAsync<Exception>().WithMessage(message);
        mockMessageHandler.Verify();
    }
}