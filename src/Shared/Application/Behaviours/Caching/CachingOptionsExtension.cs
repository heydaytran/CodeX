using Application.Messaging;

namespace Application.Behaviours.Caching;

public static class CachingOptionsExtension
{
    public static DistributedCacheEntryOptions ToDistributedOptions(this CachingOptions options, ICacheableQuery query)
    {
        var slidingExpiration = query.SlidingExpiration;
        var absoluteExpiration = query.AbsoluteExpiration;

        if (slidingExpiration == null && query.AbsoluteExpiration == null)
        {
            slidingExpiration = options.SlidingExpiration;
        }

        if (absoluteExpiration == null && query.SlidingExpiration == null)
        {
            absoluteExpiration = options.AbsoluteExpiration;
        }

        return new DistributedCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration,
            AbsoluteExpirationRelativeToNow = absoluteExpiration,
        };
    }
}