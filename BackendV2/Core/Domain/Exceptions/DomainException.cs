using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class NotFoundException_Base : Exception
    {
        public NotFoundException_Base(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public IEnumerable<string> _errors { get; }

        public BadRequestException(string message, IEnumerable<string> errors = null) : base(message)
        {
            _errors = errors ?? new List<string>();
        }
    }
}
