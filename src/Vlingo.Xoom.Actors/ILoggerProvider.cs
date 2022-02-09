﻿// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Vlingo.Xoom.Actors.Plugin.Logging.Console;
using Vlingo.Xoom.Actors.Plugin.Logging.NoOp;

namespace Vlingo.Xoom.Actors
{
    public interface ILoggerProvider
    {
        void Close();
        ILogger? Logger { get; }
    }

    internal static class LoggerProvider
    {
        internal static ILoggerProvider NoOpLoggerProvider() 
            => new NoOpLoggerProvider();

        internal static ILoggerProvider StandardLoggerProvider(World world, string name)
            => ConsoleLoggerPlugin.RegisterStandardLogger(name, world);
    }
}
