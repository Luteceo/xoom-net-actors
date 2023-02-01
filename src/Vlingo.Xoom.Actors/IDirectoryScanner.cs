﻿// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Actors;

public interface IDirectoryScanner
{
    ICompletes<T> ActorOf<T>(IAddress address);
    ICompletes<T> ActorOf<T>(IAddress address, Definition definition);
    ICompletes<Optional<T>> MaybeActorOf<T>(IAddress address);
}