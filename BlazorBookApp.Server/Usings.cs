global using System.Text.Json;
global using Polly;
global using Polly.Retry;
global using System.Net;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Caching.Memory;

global using BlazorBookApp.Server.Services;
global using BlazorBookApp.Server.Configuration;
global using BlazorBookApp.Server.Services.Contracts;
global using BlazorBookApp.Shared.Dtos;
global using BlazorBookApp.Server.Extensions;
global using BlazorBookApp.Shared.Utilities;