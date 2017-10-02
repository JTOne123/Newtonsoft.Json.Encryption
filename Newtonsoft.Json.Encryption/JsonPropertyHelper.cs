﻿using System;
using System.Reflection;
using Newtonsoft.Json.Encryption;
using Newtonsoft.Json.Serialization;

static class JsonPropertyHelper
{
    public static void Manipulate(
        MemberInfo member,
        Encrypter encrypter,
        JsonProperty property)
    {
        if (member.ContainsEncryptAttribute())
        {
            return;
        }
        var memberType = member.GetUnderlyingType();

        if (memberType == typeof(string))
        {
            VerifyConverterIsNull(property, member);
            property.Converter = new StringConverter(encrypter);
            property.MemberConverter = new StringConverter(encrypter);
            return;
        }
        if (memberType.IsStringDictionary())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new StringConverter(encrypter);
            return;
        }
        if (memberType.IsStringEnumerable())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new StringConverter(encrypter);
            return;
        }

        if (memberType == typeof(Guid))
        {
            VerifyConverterIsNull(property, member);
            property.Converter = new GuidConverter(encrypter);
            property.MemberConverter = new GuidConverter(encrypter);
            return;
        }
        if (memberType.IsGuidDictionary())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new GuidConverter(encrypter);
            return;
        }
        if (memberType.IsGuidEnumerable())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new GuidConverter(encrypter);
            return;
        }

        if (memberType == typeof(byte[]))
        {
            VerifyConverterIsNull(property, member);
            property.Converter = new ByteArrayConverter(encrypter);
            property.MemberConverter = new ByteArrayConverter(encrypter);
            return;
        }
        if (memberType.IsByteArrayDictionary())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new ByteArrayConverter(encrypter);
            return;
        }
        if (memberType.IsByteArrayEnumerable())
        {
            VerifyItemConverterIsNull(property, member);
            property.ItemConverter = new ByteArrayConverter(encrypter);
            return;
        }
        throw new Exception("Expected a string, a IDictionary<T,string>, a IEnumerable<string>, a Guid, a IDictionary<T,Guid>, a IEnumerable<Guid>, a byte[], a IDictionary<T,byte[]>, or a IEnumerable<byte[]>.");
    }

    static void VerifyItemConverterIsNull(JsonProperty property, MemberInfo member)
    {
        if (property.ItemConverter != null)
        {
            throw new Exception($"Expected JsonProperty.ItemConverter to be null. Property: {member.FriendlyName()}");
        }
    }

    static void VerifyConverterIsNull(JsonProperty property, MemberInfo member)
    {
        if (property.MemberConverter != null)
        {
            throw new Exception($"Expected JsonProperty.MemberConverter to be null. Property: {member.FriendlyName()}");
        }
        if (property.Converter != null)
        {
            throw new Exception($"Expected JsonProperty.Converter to be null. Property: {member.FriendlyName()}");
        }
    }

    static bool ContainsEncryptAttribute(this MemberInfo member)
    {
        return member.GetCustomAttribute<EncryptAttribute>() == null;
    }
    static string FriendlyName(this MemberInfo member)
    {
        return $"{member.DeclaringType.FullName}.{member.Name}";
    }
}