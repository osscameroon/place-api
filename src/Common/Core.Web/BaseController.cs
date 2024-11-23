﻿using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Web;

[Route(BaseApiPath)]
[ApiController]
[ApiVersion("1.0")]
public abstract class BaseController : ControllerBase
{
    protected const string BaseApiPath = "api/v{version:apiVersion}";

    private IMediator _mediator;

    protected IMediator? Mediator =>
        _mediator ?? HttpContext.RequestServices.GetRequiredService<IMediator>();
}
