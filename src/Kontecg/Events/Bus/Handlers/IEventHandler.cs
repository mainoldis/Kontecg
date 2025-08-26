namespace Kontecg.Events.Bus.Handlers
{
    /// <summary>
    /// Defines the base interface for all event handlers in the Kontecg event bus system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// IEventHandler is the root interface that all event handlers must implement. It serves
    /// as a marker interface to identify classes that can handle events in the event bus system.
    /// This interface is intentionally empty and is used primarily for type identification and
    /// generic constraints.
    /// </para>
    /// <para>
    /// <strong>Important Note:</strong> This interface should not be implemented directly.
    /// Instead, implement one of the specific handler interfaces:
    /// <list type="bullet">
    /// <item><description><see cref="IEventHandler{TEventData}"/> for synchronous event handling</description></item>
    /// <item><description><see cref="IAsyncEventHandler{TEventData}"/> for asynchronous event handling</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Purpose:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Type Identification:</strong> Allows the event bus to identify handler types</description></item>
    /// <item><description><strong>Generic Constraints:</strong> Used in generic methods and collections</description></item>
    /// <item><description><strong>Factory Pattern:</strong> Enables factory-based handler creation</description></item>
    /// <item><description><strong>Polymorphism:</strong> Provides common base for all handler types</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Usage in Event Bus:</strong> The event bus uses this interface to:
    /// <list type="bullet">
    /// <item><description>Store handlers in generic collections</description></item>
    /// <item><description>Create handlers through factories</description></item>
    /// <item><description>Validate handler types during registration</description></item>
    /// <item><description>Manage handler lifecycle</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Guidelines:</strong>
    /// <list type="bullet">
    /// <item><description>Do not implement this interface directly</description></item>
    /// <item><description>Use IEventHandler&lt;TEventData&gt; for synchronous handlers</description></item>
    /// <item><description>Use IAsyncEventHandler&lt;TEventData&gt; for asynchronous handlers</description></item>
    /// <item><description>Ensure handlers are thread-safe if used in multi-threaded scenarios</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IEventHandler
    {
    }
}
