﻿// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Actors.Tests.Supervision
{
    public class PingActor : Actor, IPing
    {
        public static ThreadLocal<PingActor> Instance = new ThreadLocal<PingActor>();

        private readonly PingTestResults _testResults;

        public PingActor(PingTestResults testResults)
        {
            _testResults = testResults;
            Instance.Value = this;
        }

        public void Ping()
        {
            _testResults.Access.WriteUsing("pingCount", 1);
            throw new ApplicationException("Intended Ping failure.");
        }

        public override void Stop()
        {
            base.Stop();
            _testResults.Access.WriteUsing("stopCount", 1);
        }

        public class PingTestResults
        {
            public readonly AtomicInteger PingCount = new AtomicInteger(0);
            public readonly AtomicInteger StopCount = new AtomicInteger(0);

            public AccessSafely Access { get; private set; }

            public PingTestResults()
            {
                Access = AfterCompleting(0);
            }

            public AccessSafely AfterCompleting(int times)
            {
                Access = AccessSafely
                    .AfterCompleting(times)
                    .WritingWith("pingCount", (int increment) => PingCount.Set(PingCount.Get() + increment))
                    .ReadingWith("pingCount", () => PingCount.Get())
                    .WritingWith("stopCount", (int increment) => StopCount.Set(StopCount.Get() + increment))
                    .ReadingWith("stopCount", () => StopCount.Get());
                return Access;
            }
        }
    }
}
