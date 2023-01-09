﻿using System;
using System.Threading;

using Xabbo.Messages;
using Xabbo.Extension;

using Xabbo.Core.GameData;

using Xabbo.Scripter.Scripting;

namespace Xabbo.Scripter.Services;

public interface IScriptHost
{
    /// <summary>
    /// Gets whether scripts may execute or not.
    /// </summary>
    bool CanExecute { get; }

    /// <summary>
    /// Gets the UI context of the application.
    /// </summary>
    IUiContext UiContext { get; }

    /// <summary>
    /// Gets the UI manager of the application.
    /// </summary>
    IUiManager UiManager { get; }

    /// <summary>
    /// Provides an interface to the message manager.
    /// </summary>
    IMessageManager MessageManager { get; }

    /// <summary>
    /// Provides an interface to the interceptor.
    /// </summary>
    IRemoteExtension Extension { get; }

    /// <summary>
    /// Provides an interface to the game data manager.
    /// </summary>
    IGameDataManager GameDataManager { get; }

    /// <summary>
    /// Provides an interface to the game manager.
    /// </summary>
    IGameManager GameManager { get; }

    /// <summary>
    /// Provides an interface to the JSON serializer.
    /// </summary>
    IJsonSerializer JsonSerializer { get; }

    /// <summary>
    /// Provides an interface to an object formatter.
    /// </summary>
    IObjectFormatter ObjectFormatter { get; }

    /// <summary>
    /// Provides the global variables for the scripts.
    /// </summary>
    GlobalVariables GlobalVariables { get; }

    /// <summary>
    /// Gets the cancellation token for all scripts.
    /// </summary>
    CancellationToken CancellationToken { get; }
}
