namespace Netsoft.Core.Model
{
    using System;

    public interface IPolicyConfig
    {
        bool HasResilicence { get; }

        TimeSpan HandlerLifetime { get; }
    }
}
