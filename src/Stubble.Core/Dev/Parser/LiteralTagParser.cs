﻿// <copyright file="LiteralTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// Parses literal characters
    /// </summary>
    public class LiteralTagParser
    {
        /// <summary>
        /// Parse a literal tag from the slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>The result of the match</returns>
        public LiteralTagResult Match(Processor processor, ref StringSlice slice)
        {
            var c = slice.CurrentChar;

            var start = slice.Start;
            var index = slice.Start;

            var tag = new LiteralTag
            {
                TagStartPosition = index,
                ContentStartPosition = index
            };
            processor.CurrentTag = tag;

            while (c != '\0')
            {
                if (slice.Match(processor.CurrentTags.StartTag))
                {
                    if (tag.TagStartPosition == slice.Start)
                    {
                        return LiteralTagResult.NoContent;
                    }

                    tag.TagEndPosition = slice.Start;
                    tag.ContentEndPosition = slice.Start;
                    tag.Content = new StringSlice(slice.Text, tag.ContentStartPosition, tag.ContentEndPosition - 1).ToString();
                    tag.IsClosed = true;
                    return LiteralTagResult.TagStart;
                }

                // If this is whitespace then increase the start pointer by one
                if (c.IsWhitespace())
                {
                    if (!processor.HasSeenNonSpaceOnLine)
                    {
                        processor.LineIndent++;
                    }

                    start++;
                }
                else
                {
                    processor.HasSeenNonSpaceOnLine = true;
                }

                if (slice.IsNewLine())
                {
                    int endIndex;
                    if (c == '\n')
                    {
                        endIndex = slice.Start + 1;
                    }
                    else
                    {
                        endIndex = slice.Start + 2;
                    }

                    tag.TagEndPosition = endIndex;
                    tag.ContentEndPosition = endIndex;
                    tag.Content = new StringSlice(slice.Text, tag.ContentStartPosition, tag.ContentEndPosition - 1).ToString();
                    tag.IsClosed = true;
                    return LiteralTagResult.NewLine;
                }

                c = slice.NextChar();
            }

            if (tag.TagStartPosition == slice.Start)
            {
                return LiteralTagResult.NoContent;
            }

            tag.TagEndPosition = slice.Start;
            tag.ContentEndPosition = slice.Start;
            tag.Content = new StringSlice(slice.Text, tag.ContentStartPosition, tag.ContentEndPosition - 1).ToString();
            tag.IsClosed = true;

            return LiteralTagResult.EndOfFile;
        }
    }
}