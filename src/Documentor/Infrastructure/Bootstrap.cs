using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public static class Bootstrap
    {
        private static readonly string _validInput = "is-valid";
        private static readonly string _invalidInput = "is-invalid";
        private static readonly string _invalidMessage = "invalid-feedback";
        private static readonly string _validMessage = _invalidMessage;
        private static readonly string _invalidValidationSummary = HtmlHelper.ValidationSummaryCssClassName + " alert alert-danger";

        public static void Configure()
        {
            SetPublicFieldValue<HtmlHelper>(nameof(HtmlHelper.ValidationInputCssClassName), _invalidInput);
            SetPublicFieldValue<HtmlHelper>(nameof(HtmlHelper.ValidationInputValidCssClassName), _validInput);
            SetPublicFieldValue<HtmlHelper>(nameof(HtmlHelper.ValidationMessageCssClassName), _invalidMessage);
            SetPublicFieldValue<HtmlHelper>(nameof(HtmlHelper.ValidationMessageValidCssClassName), _validMessage);
            SetPublicFieldValue<HtmlHelper>(nameof(HtmlHelper.ValidationSummaryCssClassName), _invalidValidationSummary);
        }

        private static void SetPublicFieldValue<T>(string fieldName, object fieldValue)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo == null)
                throw new NullReferenceException(string.Format("Static field '{0}' not found in '{1}' class", fieldName, nameof(T)));

            fieldInfo.SetValue(null, fieldValue);
        }
    }
}
