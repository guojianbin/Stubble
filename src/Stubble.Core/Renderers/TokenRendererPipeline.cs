﻿// <copyright file="TokenRendererPipeline.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Stubble.Core.Imported;
using Stubble.Core.Renderers.Interfaces;
using Stubble.Core.Tokens;

namespace Stubble.Core.Renderers
{
    /// <summary>
    /// An internal pipeline of token renderers for use by all render operations
    /// </summary>
    public class TokenRendererPipeline
    {
        private readonly OrderedList<ITokenRenderer> tokenRenderers;

        private readonly ConcurrentDictionary<Type, ITokenRenderer> renderersPerType
            = new ConcurrentDictionary<Type, ITokenRenderer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRendererPipeline"/> class
        /// with some inital token renderers
        /// </summary>
        /// <param name="initalRenderers">The renderers to initalise with</param>
        public TokenRendererPipeline(List<ITokenRenderer> initalRenderers)
        {
            tokenRenderers = new OrderedList<ITokenRenderer>(initalRenderers);
        }

        /// <summary>
        /// Tries to get a token renderer for a given tag type
        /// </summary>
        /// <typeparam name="T">The type of the tag</typeparam>
        /// <param name="renderer">The base renderer being used to render</param>
        /// <param name="obj">The tag to get the renderer for</param>
        /// <returns>The tag renderer that matches or null</returns>
        public ITokenRenderer TryGetTokenRenderer<T>(RendererBase renderer, T obj)
            where T : MustacheToken
        {
            var objectType = obj.GetType();

            if (!renderersPerType.TryGetValue(objectType, out ITokenRenderer tokenRenderer))
            {
                for (int i = 0; i < tokenRenderers.Count; i++)
                {
                    var testRenderer = tokenRenderers[i];
                    if (testRenderer.Accept(renderer, obj))
                    {
                        renderersPerType[objectType] = tokenRenderer = testRenderer;
                        break;
                    }
                }
            }

            return tokenRenderer;
        }
    }
}
