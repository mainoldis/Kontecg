using Kontecg.Events.Bus;
using Kontecg.MassTransit.Abstractions;
using System.Collections.Generic;
using System;
using System.Linq;
using Kontecg.Dependency;

namespace Kontecg.MassTransit.Strategies
{
    /// <summary>
    /// Convention-based strategy for determining which events to publish.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy is useful when you want to apply rules based on naming patterns
    /// without needing to decorate each class with attributes.
    /// </para>
    /// <para>
    /// <strong>Default conventions:</strong>
    /// <list type="bullet">
    /// <item><description>Events ending in "IntegrationEvent" are published</description></item>
    /// <item><description>Events ending in "DomainEvent" are published</description></item>
    /// <item><description>Events in namespaces containing "Integration" are published</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class ConventionBasedPublishingStrategy : IEventPublishingStrategy, ITransientDependency
    {
        private readonly HashSet<string> _publishSuffixes;
        private readonly HashSet<string> _localOnlySuffixes;
        private readonly HashSet<string> _publishNamespacePatterns;

        /// <summary>
        /// Initializes the strategy with default conventions.
        /// </summary>
        public ConventionBasedPublishingStrategy()
        {
            _publishSuffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "IntegrationEvent",
                "DomainEvent",
                "ExternalEvent",
                "SharedEvent"
            };

            _localOnlySuffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LocalEvent",
                "UIEvent",
                "InternalEvent"
            };

            _publishNamespacePatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Integration",
                "External",
                "Shared",
                "Events.Domain"
            };
        }

        /// <summary>
        /// Customizes suffixes that determine events to publish.
        /// </summary>
        /// <param name="suffixes">Suffixes that indicate the event should be published</param>
        /// <returns>This instance for fluent configuration</returns>
        public ConventionBasedPublishingStrategy WithPublishSuffixes(params string[] suffixes)
        {
            _publishSuffixes.Clear();
            foreach (string suffix in suffixes)
            {
                _publishSuffixes.Add(suffix);
            }
            return this;
        }

        /// <summary>
        /// Customizes namespace patterns that determine events to publish.
        /// </summary>
        /// <param name="patterns">Namespace patterns</param>
        /// <returns>This instance for fluent configuration</returns>
        public ConventionBasedPublishingStrategy WithNamespacePatterns(params string[] patterns)
        {
            _publishNamespacePatterns.Clear();
            foreach (string pattern in patterns)
            {
                _publishNamespacePatterns.Add(pattern);
            }
            return this;
        }

        /// <inheritdoc />
        public bool ShouldPublishToMassTransit<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return eventData != null && ShouldPublishToMassTransit(eventData.GetType());
        }

        /// <inheritdoc />
        public bool ShouldPublishToMassTransit(Type eventType)
        {
            if (eventType == null) return false;

            string typeName = eventType.Name;
            string typeNamespace = eventType.Namespace ?? string.Empty;

            // Check exclusion suffixes first
            if (_localOnlySuffixes.Any(suffix => typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Check publish suffixes
            if (_publishSuffixes.Any(suffix => typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Check namespace patterns
            if (_publishNamespacePatterns.Any(pattern =>
                typeNamespace.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
    }
}
