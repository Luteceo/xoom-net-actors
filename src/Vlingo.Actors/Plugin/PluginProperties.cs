﻿// Copyright (c) 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Globalization;

namespace Vlingo.Actors.Plugin
{
    public class PluginProperties
    {
        private readonly Properties properties;

        public PluginProperties(string name, Properties properties)
        {
            Name = name;
            this.properties = properties;
        }

        public string Name { get; }

        public Boolean GetBoolean(string key, bool defaultValue)
        {
            var value = GetString(key, defaultValue.ToString());
            return bool.Parse(value);
        }

        public float GetFloat(string key, float defaultValue)
        {
            var value = GetString(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public int GetInteger(string key, int defaultValue)
        {
            var value = GetString(key, defaultValue.ToString());
            return int.Parse(value);
        }

        public string? GetString(string key, string? defaultValue) => properties.GetProperty(Key(key), defaultValue);

        private string Key(string key) => $"plugin.{Name}.{key}";
    }
}
