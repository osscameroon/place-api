using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;

namespace PlaceApi.Authentication.UseCases.GetUserInfo;

public sealed record GetUserInfoCommand : IRequest<Result<InfoResponse>>;
