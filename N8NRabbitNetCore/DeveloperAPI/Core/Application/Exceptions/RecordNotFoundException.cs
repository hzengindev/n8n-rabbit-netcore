using System;
namespace DeveloperAPI.Core.Application.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException() : base(message: "There is not record.") { }
        public RecordNotFoundException(string message) : base(message) { }
    }
}

