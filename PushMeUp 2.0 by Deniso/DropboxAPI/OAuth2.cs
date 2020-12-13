using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso.DropboxAPI
{
    class OAuth2
    {
        //private async Task HandleOAuth2Redirect(HttpListener http)
        //{
        //    var context = await http.GetContextAsync();

        //    // We only care about request to RedirectUri endpoint.
        //    while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
        //    {
        //        context = await http.GetContextAsync();
        //    }

        //    // Respond with a HTML page which runs JS to send URl fragment.
        //    RespondPageWithJSRedirect();
        //}


        //private async Task<OAuth2Response> HandleJSRedirect(HttpListener http)
        //{
        //    var context = await http.GetContextAsync();

        //    // We only care about request to TokenRedirectUri endpoint.
        //    while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
        //    {
        //        context = await http.GetContextAsync();
        //    }

        //    var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

        //    var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);

        //    return result;
        //}

        //private async Task GetAccessToken()
        //{
        //    var state = Guid.NewGuid().ToString("N");
        //    var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ApiKey, new Uri(RedirectUri), state: state);

        //    var http = new HttpListener();
        //    http.Prefixes.Add(RedirectUri);
        //    http.Start();

        //    System.Diagnostics.Process.Start(authorizeUri.ToString());

        //    // Handle OAuth redirect and send URL fragment to local server using JS.
        //    await HandleOAuth2Redirect(http);

        //    // Handle redirect from JS and process OAuth response.
        //    var result = await HandleJSRedirect(http);

        //    if (result.State != state)
        //    {
        //        // The state in the response doesn't match the state in the request.
        //        return null;
        //    }

        //    Settings.Default.AccessToken = result.AccessToken;
        //}


    }
}
