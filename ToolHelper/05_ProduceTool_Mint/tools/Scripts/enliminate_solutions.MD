Microsoft.Exchange.AntiMalware.Client.dll
    ConfigurationChannelFactory -》ChannelFactory  (hard code)

# ------------------------------------------------------------

    System.ServiceModel.ChannelFactory


# ???
    System.Net.Http.WebRequestHandler => HttpClientHandler
    System.Security.Cryptography => # if else
    System.ServiceModel.Description.ServiceDescription => seprate
    System.ServiceModel.NetNamedPipeBinding
    {
        if client side => TcpBinding
        if server side => seprate
    }

    ( allen:
        System.ServiceModel.Web.OutgoingWebRequestContext
        System.ServideModel.Web.WebOperationCOntext
    )
    System.Web.Caching => ?
    System.Web.UI

# Seprate Server Side Code
    System.ServiceModel.ServiceHost (seprate server side code)
    System.ServiceModel.ServideHostBase

# Investigate
    System.Web.IHttpModule
    System.Web.HttpApplication => seprate if used only by IHttpModule
    System.ServiceModel.FaultException

## ADapter @Shirley
    System.Web.Hosting.HostingEnvironment
    System.Web.Hosting.HostingEnvironment.get_ApplicationVirtualPath
    System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(System.Func{System.Threading.CancellationToken,System.Threading.Tasks.Task})
    System.Web.HttpCacheability
    System.Web.HttpCachePolicy
    System.Web.HttpCachePolicy.SetCacheability(System.Web.HttpCacheability)
    System.Web.HttpCachePolicy.SetExpires(System.DateTime)
    System.Web.HttpCachePolicyBase
    System.Web.HttpCachePolicyBase.SetCacheability(System.Web.HttpCacheability)
    System.Web.HttpCachePolicyBase.SetExpires(System.DateTime)
    System.Web.HttpCachePolicyBase.SetNoStore
    System.Web.HttpServerUtility
    System.Web.HttpServerUtility.get_MachineName
    System.Web.HttpServerUtility.UrlTokenEncode(System.Byte[])
    System.Web.Routing.RequestContext
    System.Web.Routing.RequestContext.get_HttpContext

# NewtonSoft.Json
    System.Web.Script.Serialization.JavaScriptSerializer

# No Solution Yet
    System.Web.Configuration.*
