﻿using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the `format` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name} Value={Value.Key}")]
	public class FormatKeyword : IJsonSchemaKeyword, IEquatable<FormatKeyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - actual
		/// - format
		/// - isKnownFormat
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "{{actual}} is not in an acceptable {{format}} format.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "format";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions => Formats.GetFormat(Value)?.SupportedBy ?? JsonSchemaVersion.None;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Format;

		/// <summary>
		/// The string format for this keyword.
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		[DeserializationUseOnly]
		[UsedImplicitly]
		public FormatKeyword() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		/// <summary>
		/// Creates an instance of the <see cref="FormatKeyword"/>.
		/// </summary>
		public FormatKeyword(string value)
		{
			Value = value;
		}

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context)
				{
					AnnotationValue = Value
				};

			if (!context.Options.ValidateFormatKeyword)
			{
				Log.Schema(() => "Options indicate skipping format validation");
				return results;
			}

			var validator = Formats.GetFormat(Value);
			if (validator == null)
			{
				results.AdditionalInfo["actual"] = context.Instance;
				results.AdditionalInfo["format"] = Value;
				results.AdditionalInfo["isKnownFormat"] = false;

				if (context.Options.AllowUnknownFormats)
					results.IsValid = true;
				else
				{
					results.IsValid = false;
					results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
				}
			}
			else if (!validator.Validate(context.Instance))
			{
				results.IsValid = false;
				results.AdditionalInfo["actual"] = context.Instance;
				results.AdditionalInfo["format"] = Value;
				results.AdditionalInfo["isKnownFormat"] = true;
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="context">The context object.</param>
		public void RegisterSubschemas(SchemaValidationContext context) { }
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <param name="supportedVersions">Indicates the root schema's supported versions.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri, JsonSchemaVersion supportedVersions)
		{
			return null;
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.String;

			var validator = Formats.GetFormat(Value);

			if (validator == null && JsonSchemaOptions.Default.ValidateFormatKeyword && !JsonSchemaOptions.Default.AllowUnknownFormats)
				throw new JsonSerializationException("Unknown format specifier found.  Either allow unknown formats or disable format validation in the default JsonSchemaOptions.");
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(FormatKeyword? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword? other)
		{
			return Equals(other as FormatKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as FormatKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}