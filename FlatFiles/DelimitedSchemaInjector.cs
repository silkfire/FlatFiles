﻿using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of the data being written.
    /// </summary>
    public sealed class DelimitedSchemaInjector
    {
        private readonly List<SchemaMatcher> matchers = [];
        private SchemaMatcher? defaultMatcher;

        /// <summary>
        /// Initializes a new instance of a DelimitedSchemaInjector.
        /// </summary>
        public DelimitedSchemaInjector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IDelimitedSchemaInjectorWhenBuilder When(Func<object?[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new DelimitedSchemaInjectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public void WithDefault(DelimitedSchema? schema)
        {
            if (schema == null)
            {
                defaultMatcher = null;
            }
            else
            {
                defaultMatcher = new SchemaMatcher(schema, static _ => true);
            }
        }

        private void Add(DelimitedSchema schema, Func<object?[], bool> predicate)
        {
            var matcher = new SchemaMatcher(schema, predicate);
            matchers.Add(matcher);
        }

        internal DelimitedSchema? GetSchema(object?[] values)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(values))
                {
                    return matcher.Schema;
                }
            }
            if (defaultMatcher != null && defaultMatcher.Predicate(values))
            {
                return defaultMatcher.Schema;
            }
            throw new FlatFileException(Resources.MissingMatcher);
        }

        private sealed class SchemaMatcher
        {
            public SchemaMatcher(DelimitedSchema schema, Func<object?[], bool> predicate)
            {
                Schema = schema;
                Predicate = predicate;
            }

            public DelimitedSchema Schema { get; }

            public Func<object?[], bool> Predicate { get; }
        }

        private sealed class DelimitedSchemaInjectorWhenBuilder : IDelimitedSchemaInjectorWhenBuilder
        {
            private readonly DelimitedSchemaInjector injector;
            private readonly Func<object?[], bool> predicate;

            public DelimitedSchemaInjectorWhenBuilder(DelimitedSchemaInjector injector, Func<object?[], bool> predicate)
            {
                this.injector = injector;
                this.predicate = predicate;
            }

            public void Use(DelimitedSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                injector.Add(schema, predicate);
            }
        }
    }
}
