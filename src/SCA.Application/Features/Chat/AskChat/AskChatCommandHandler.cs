using System.Security.Cryptography;
using System.Text;
using SCA.Application.Abstractions;
using SCA.Application.Common;
using SCA.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SCA.Application.Features.Chat.AskChat;

/// <summary>
/// Handles chat requests by checking Redis first and calling Ollama when needed.
/// </summary>
public sealed class AskChatCommandHandler(
    IRedisCache redisCache,
    IOllamaClient ollamaClient,
    ILogger<AskChatCommandHandler> logger) : IRequestHandler<AskChatCommand, ChatResponse>
{
    /// <summary>
    /// Returns a cached answer or asks Ollama for a new one.
    /// </summary>
    public async Task<ChatResponse> Handle(AskChatCommand request, CancellationToken cancellationToken)
    {
        string message = request.Message.Trim();
        string cacheKey = CreateCacheKey(message);

        string? cachedResponse = await redisCache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            logger.LogInformation("Answer returned from Redis cache.");
            return new ChatResponse(cachedResponse);
        }

        logger.LogInformation("Sending message to Ollama.");
        string response = await ollamaClient.AskAsync(message, cancellationToken);

        if (string.IsNullOrWhiteSpace(response))
        {
            throw new ExternalServiceException("Ollama returned an empty response.");
        }

        await redisCache.SetStringAsync(cacheKey, response, ChatRules.CacheDuration, cancellationToken);
        logger.LogInformation("Answer saved to Redis cache.");

        return new ChatResponse(response);
    }

    /// <summary>
    /// Creates a stable cache key for the same question.
    /// </summary>
    private static string CreateCacheKey(string message)
    {
        string normalizedMessage = message.Trim().ToLowerInvariant();
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedMessage));
        string hash = Convert.ToHexString(bytes).ToLowerInvariant();

        return $"cache:chat:{hash}";
    }
}
