// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using ExtCore.Infrastructure;

namespace ExtCore.Events;

/// <summary>
/// Represents an event that can be broadcasted and handled by the corresponding event handlers
/// specified by the <see cref="TEventHandler">TEventHandler</see> type parameter. The event handlers
/// might be located in this or any other extension and will be resolved automatically by ExtCore.
/// </summary>
/// <typeparam name="TEventHandler">Defines the type of the event handlers that will handle the event.</typeparam>
public static class Event<TEventHandler> where TEventHandler : IEventHandler
{
    /// <summary>
    /// Broadcasts the event to all the event handlers that are resolved automatically by ExtCore.
    /// </summary>
    /// <returns>Resolved event handlers that have handled the event.</returns>
    public static IEnumerable<TEventHandler> Broadcast()
    {
        IEnumerable<TEventHandler> eventHandlers = ExtensionManager.GetInstances<TEventHandler>().OrderBy(eh => eh.Priority);

        foreach (TEventHandler eventHandler in eventHandlers)
            eventHandler.HandleEvent();

        return eventHandlers;
    }
}

/// <summary>
/// Represents an event that can be broadcasted and handled by the corresponding event handlers
/// specified by the <see cref="TEventHandler">TEventHandler</see> type parameter. The event handlers
/// might be located in this or any other extension and will be resolved automatically by ExtCore.
/// </summary>
/// <typeparam name="TEventHandler">Defines the type of the event handlers that will handle the event.</typeparam>
/// <typeparam name="TEventArgument">Defines the type of the argument that will be passed to the event handlers.</typeparam>
public static class Event<TEventHandler, TEventArgument> where TEventHandler : IEventHandler<TEventArgument>
{
    /// <summary>
    /// Broadcasts the event to all the event handlers that are resolved automatically by ExtCore.
    /// </summary>
    /// <param name="argument">The event argument.</param>
    /// <returns>Resolved event handlers that have handled the event.</returns>
    public static IEnumerable<TEventHandler> Broadcast(TEventArgument argument)
    {
        IEnumerable<TEventHandler> eventHandlers = ExtensionManager.GetInstances<TEventHandler>().OrderBy(eh => eh.Priority);

        foreach (TEventHandler eventHandler in eventHandlers)
            eventHandler.HandleEvent(argument);

        return eventHandlers;
    }
}