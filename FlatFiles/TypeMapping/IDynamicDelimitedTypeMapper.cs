﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Supports reading to and writing from flat files for a runtime type. 
    /// </summary>
    public interface IDynamicDelimitedTypeMapper : IDynamicDelimitedTypeConfiguration
    {
        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<object> Read(TextReader reader, DelimitedOptions? options = null);

        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <returns>An asynchronous enumerable over the entities.</returns>
        IAsyncEnumerable<object> ReadAsync(TextReader reader, DelimitedOptions? options = null);

        /// <summary>
        /// Gets a typed reader to read entities from the underlying document.
        /// </summary>
        /// <param name="reader">A reader over the delimited document.</param>
        /// <param name="options">The options controlling how the delimited document is read.</param>
        /// <returns>A typed reader.</returns>
        IDelimitedTypedReader<object> GetReader(TextReader reader, DelimitedOptions? options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the delimited document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<object> entities, DelimitedOptions? options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the delimited document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        Task WriteAsync(TextWriter writer, IEnumerable<object> entities, DelimitedOptions? options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the delimited document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        Task WriteAsync(TextWriter writer, IAsyncEnumerable<object> entities, DelimitedOptions? options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the delimited document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<object> GetWriter(TextWriter writer, DelimitedOptions? options = null);
    }
}
