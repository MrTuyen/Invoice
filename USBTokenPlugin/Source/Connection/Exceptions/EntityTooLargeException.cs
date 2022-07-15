﻿using System;
using System.Runtime.Serialization;

namespace WebSockets.Exceptions
{
    [Serializable]
    public class EntityTooLargeException : Exception
    {
        public EntityTooLargeException() : base()
        {
            
        }

        /// <summary>
        /// Http header too large to fit in buffer
        /// </summary>
        public EntityTooLargeException(string message) : base(message)
        {
            
        }

        public EntityTooLargeException(string message, Exception inner) : base(message, inner)
        {

        }

        public EntityTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
