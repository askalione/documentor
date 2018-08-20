using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public class JsonResponse
    {
        public bool Success { get; } = false;
        public string SuccessMessage { get; } = "";
        private readonly List<JsonResponseError> _errors = new List<JsonResponseError>();
        public IEnumerable<JsonResponseError> Errors => _errors;
        public IEnumerable<string> ErrorsMessages => Errors.Select(e => e.Message);
        public string ErrorMessage => ErrorsMessages.FirstOrDefault();
        public object Data { get; } = null;

        public JsonResponse(bool success)
        {
            Success = success;
        }

        public JsonResponse(bool success, object data) : this(success)
        {
            Data = data;
        }

        public JsonResponse(bool success, string message) : this(success)
        {
            if (success)
            {
                SuccessMessage = message;
            }
            else
            {
                _errors.Add(new JsonResponseError(message));
            }
        }

        public JsonResponse(bool success, string message, object data) : this(success, message)
        {
            Data = data;
        }

        public JsonResponse(bool success, IEnumerable<JsonResponseError> errors) : this(success)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            _errors = errors.ToList();
        }

        public JsonResponse(bool success, IEnumerable<JsonResponseError> errors, object data) : this(success, errors)
        {
            Data = data;
        }
    }
}
