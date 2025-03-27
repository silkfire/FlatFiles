﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal sealed class MemberLookup
    {
        private readonly Dictionary<string, IMemberMapping> lookup = new();
        private readonly Dictionary<Type, object?> factories = new();
        private int ignoredCount;

        public int LogicalCount => lookup.Count - ignoredCount;

        public TMemberMapping GetOrAddMember<TMemberMapping>(IMemberAccessor member, Func<int, int, TMemberMapping> factory)
            where TMemberMapping : IMemberMapping
        {
            return GetOrAddMember(member.Name, factory);
        }

        public CustomMapping<TEntity> GetOrAddCustomMapping<TEntity>(string name, Func<int, int, CustomMapping<TEntity>> factory)
        {
            var key = $"@Custom_{name}";
            return GetOrAddMember(key, factory);
        }

        private TMapping GetOrAddMember<TMapping>(string key, Func<int, int, TMapping> factory)
            where TMapping : IMemberMapping
        {
            if (lookup.TryGetValue(key, out var mapping))
            {
                return (TMapping)mapping;
            }

            var physicalIndex = lookup.Count;
            var logicalIndex = physicalIndex - ignoredCount;
            var newMapping = factory(physicalIndex, logicalIndex);
            lookup.Add(key, newMapping);
            return newMapping;
        }

        public IgnoredMapping AddIgnored()
        {
            var column = new IgnoredColumn();
            var mapping = new IgnoredMapping(column, lookup.Count);
            var key = $"@Ignored_{mapping.PhysicalIndex}";
            lookup.Add(key, mapping);
            ++ignoredCount;
            return mapping;
        }

        public IMemberMapping[] GetMappings()
        {
            return lookup.Values.OrderBy(static m => m.PhysicalIndex).ToArray();
        }

        public Func<TEntity>? GetFactory<TEntity>()
        {
            if (factories.TryGetValue(typeof(TEntity), out var factory))
            {
                if (factory is Func<TEntity> entityFactory)
                {
                    return entityFactory;
                }
                if (factory is Func<object> objectFactory)
                {
                    return () => (TEntity)objectFactory();
                }
            }
            return null;
        }

        public void SetFactory<TEntity>(Func<TEntity> factory)
        {
            factories.Add(typeof(TEntity), factory);
        }

        public void SetFactory(Type entityType, Func<object> factory)
        {
            factories.Add(entityType, factory);
        }
    }
}
