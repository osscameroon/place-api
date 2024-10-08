using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;

namespace PlaceAPi.Identity.Authenticate.Endpoints;

public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointConventionBuilder MapAuthenticationEndpoints<TUser>(
        this IEndpointRouteBuilder endpoints
    )
        where TUser : class, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        RouteGroupBuilder routeGroup = endpoints.MapGroup("");

        routeGroup
            .MapLoginEndpoint<TUser>()
            .MapRegisterEndpoint<TUser>()
            .MapRefreshEndpoint<TUser>()
            .MapConfirmEmailEndpoint<TUser>()
            .MapResendConfirmationEmailEndpoint<TUser>()
            .MapForgotPasswordEndpoint<TUser>()
            .MapResetPasswordEndpoint<TUser>();

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }

    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner)
        : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention) =>
            InnerAsConventionBuilder.Add(convention);

        public void Finally(Action<EndpointBuilder> finallyConvention) =>
            InnerAsConventionBuilder.Finally(finallyConvention);
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata { }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata { }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
    {
        public string? Name => null;
    }
}
