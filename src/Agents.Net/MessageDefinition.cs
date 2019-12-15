#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Tobias Wilker and contributors
//  This file is licensed under MIT
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Agents.Net
{
    public class MessageDefinition : IEquatable<MessageDefinition>
    {
        public MessageDefinition(string category)
        {
            Category = category;
        }

        public string Category { get; }

        public override string ToString()
        {
            return $"{nameof(Category)}: {Category}";
        }

        public bool Equals(MessageDefinition other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Category == other.Category;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MessageDefinition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Category.GetHashCode() * 397);
            }
        }

        public static bool operator ==(MessageDefinition left, MessageDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MessageDefinition left, MessageDefinition right)
        {
            return !Equals(left, right);
        }
    }
}