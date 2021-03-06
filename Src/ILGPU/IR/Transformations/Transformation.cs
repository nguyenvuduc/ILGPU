﻿// -----------------------------------------------------------------------------
//                                    ILGPU
//                     Copyright (c) 2016-2020 Marcel Koester
//                                www.ilgpu.net
//
// File: Transformation.cs
//
// This file is part of ILGPU and is distributed under the University of
// Illinois Open Source License. See LICENSE.txt for details
// -----------------------------------------------------------------------------

using ILGPU.IR.Analyses;
using System;
using System.Runtime.CompilerServices;
#if !VERIFICATION
using System.Threading.Tasks;
#endif

namespace ILGPU.IR.Transformations
{
    /// <summary>
    /// Represents a generic transformation.
    /// </summary>
    public abstract class Transformation
    {
        #region Nested Types

        /// <summary>
        /// Represents an abstract transform execution driver closure.
        /// </summary>
        protected internal interface ITransformExecutor
        {
            /// <summary>
            /// Executes the current transformation.
            /// </summary>
            /// <param name="builder">The current method builder.</param>
            /// <returns>True, if the transformation could be applied.</returns>
            bool Execute(Method.Builder builder);
        }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new transformation.
        /// </summary>
        protected Transformation() { }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms all method in the given context.
        /// </summary>
        /// <param name="methods">The methods to transform.</param>
        public abstract void Transform<TPredicate>(
            MethodCollection<TPredicate> methods)
            where TPredicate : IMethodCollectionPredicate;

        /// <summary>
        /// Transforms the given method using the provided builder.
        /// </summary>
        /// <param name="builder">The current method builder.</param>
        /// <param name="executor">The desired transform executor.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static bool ExecuteTransform<TExecutor>(
            Method.Builder builder,
            in TExecutor executor)
            where TExecutor : struct, ITransformExecutor
        {
            var result = executor.Execute(builder);
            if (result)
                builder.Method.AddTransformationFlags(MethodTransformationFlags.Dirty);
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Represents a generic transformation that can be applied in an unordered manner.
    /// </summary>
    /// <remarks>Note that this transformation is applied in parallel to all methods.</remarks>
    public abstract class UnorderedTransformation : Transformation
    {
        #region Nested Types

        /// <summary>
        /// Represents an unordered executor.
        /// </summary>
        private readonly struct Executor : ITransformExecutor
        {
            /// <summary>
            /// Constructs a new executor.
            /// </summary>
            /// <param name="parent">The parent transformation.</param>
            public Executor(UnorderedTransformation parent)
            {
                Parent = parent;
            }

            /// <summary>
            /// The associated parent transformation.
            /// </summary>
            public UnorderedTransformation Parent { get; }

            /// <summary cref="Transformation.ITransformExecutor.Execute(Method.Builder)"/>
            public bool Execute(Method.Builder builder) =>
                Parent.PerformTransformation(builder);
        }

        #endregion

        #region Instance

        private readonly Action<Method> transformerDelegate;

        /// <summary>
        /// Constructs a new transformation.
        /// </summary>
        protected UnorderedTransformation()
        {
            transformerDelegate = (Method method) =>
            {
                var executor = new Executor(this);
                using (var builder = method.CreateBuilder())
                    ExecuteTransform(builder, executor);
            };
        }

        #endregion

        #region Methods

        /// <summary cref="Transformation.Transform{TPredicate}(MethodCollection{TPredicate})"/>
        public override void Transform<TPredicate>(
            MethodCollection<TPredicate> methods)
        {
#if VERIFICATION
            foreach (var method in methods)
                transformerDelegate(method);
#else
            Parallel.ForEach(methods, transformerDelegate);
#endif
        }

        /// <summary>
        /// Transforms the given method using the provided builder.
        /// </summary>
        /// <param name="builder">The current method builder.</param>
        protected abstract bool PerformTransformation(Method.Builder builder);

        #endregion
    }

    /// <summary>
    /// Represents a generic transformation that can be applied in an unordered manner.
    /// </summary>
    /// <typeparam name="TIntermediate">The type of the intermediate values.</typeparam>
    public abstract class UnorderedTransformation<TIntermediate> : Transformation
    {
        #region Nested Types

        /// <summary>
        /// Represents an unordered executor.
        /// </summary>
        private readonly struct Executor : ITransformExecutor
        {
            /// <summary>
            /// Constructs a new executor.
            /// </summary>
            /// <param name="parent">The parent transformation.</param>
            /// <param name="intermediate">The intermediate value.</param>
            public Executor(
                UnorderedTransformation<TIntermediate> parent,
                TIntermediate intermediate)
            {
                Parent = parent;
                Intermediate = intermediate;
            }

            /// <summary>
            /// The associated parent transformation.
            /// </summary>
            public UnorderedTransformation<TIntermediate> Parent { get; }

            /// <summary>
            /// Returns the associated intermediate value.
            /// </summary>
            public TIntermediate Intermediate { get; }

            /// <summary cref="Transformation.ITransformExecutor.Execute(Method.Builder)"/>
            public bool Execute(Method.Builder builder) =>
                Parent.PerformTransformation(builder, Intermediate);
        }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new transformation.
        /// </summary>
        protected UnorderedTransformation() { }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new intermediate value.
        /// </summary>
        /// <returns>The resulting intermediate value.</returns>
        protected abstract TIntermediate CreateIntermediate();

        /// <summary>
        /// Is invoked after all methods have been transformed.
        /// </summary>
        /// <param name="intermediate">The current intermediate value.</param>
        protected abstract void FinishProcessing(TIntermediate intermediate);

        /// <summary cref="Transformation.Transform{TPredicate}(MethodCollection{TPredicate})"/>
        public override void Transform<TPredicate>(
            MethodCollection<TPredicate> methods)
        {
            var intermediate = CreateIntermediate();

            // Apply transformation to all methods
            foreach (var method in methods)
            {
                var executor = new Executor(this, intermediate);
                using (var builder = method.CreateBuilder())
                    ExecuteTransform(builder, executor);
            }

            FinishProcessing(intermediate);
        }

        /// <summary>
        /// Transforms the given method using the provided builder.
        /// </summary>
        /// <param name="builder">The current method builder.</param>
        /// <param name="intermediate">The intermediate value.</param>
        protected abstract bool PerformTransformation(
            Method.Builder builder,
            TIntermediate intermediate);

        #endregion
    }

    /// <summary>
    /// Represents a generic transformation that will be applied in the post order
    /// of the induced call graph.
    /// </summary>
    public abstract class OrderedTransformation : Transformation
    {
        #region Nested Types

        /// <summary>
        /// Represents an ordered executor.
        /// </summary>
        private readonly struct Executor : ITransformExecutor
        {
            /// <summary>
            /// Constructs a new executor.
            /// </summary>
            /// <param name="parent">The parent transformation.</param>
            /// <param name="landscape">The current landscape.</param>
            /// <param name="entry">The current landscape entry.</param>
            /// <param name="cachedScopeProvider">The cached scope provider.</param>
            public Executor(
                OrderedTransformation parent,
                Landscape landscape,
                Landscape.Entry entry,
                CachedScopeProvider cachedScopeProvider)
            {
                Parent = parent;
                Landscape = landscape;
                Entry = entry;
                CachedScopeProvider = cachedScopeProvider;
            }

            /// <summary>
            /// The associated parent transformation.
            /// </summary>
            public OrderedTransformation Parent { get; }

            /// <summary>
            /// Returns the current landscape.
            /// </summary>
            public Landscape Landscape { get; }

            /// <summary>
            /// Returns the current entry.
            /// </summary>
            public Landscape.Entry Entry { get; }

            /// <summary>
            /// Returns the scope provider.
            /// </summary>
            public CachedScopeProvider CachedScopeProvider { get; }

            /// <summary cref="Transformation.ITransformExecutor.Execute(Method.Builder)"/>
            public bool Execute(Method.Builder builder) =>
                Parent.PerformTransformation(
                    builder,
                    Landscape,
                    Entry,
                    CachedScopeProvider);
        }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new transformation.
        /// </summary>
        protected OrderedTransformation() { }

        #endregion

        #region Methods

        /// <summary cref="Transformation.Transform{TPredicate}(MethodCollection{TPredicate})"/>
        public sealed override void Transform<TPredicate>(
            MethodCollection<TPredicate> methods)
        {
            var landscape = Landscape.Create<MethodCollection<TPredicate>, TPredicate>(methods);
            if (landscape.Count < 1)
                return;

            var scopeProvider = new CachedScopeProvider();
            foreach (var entry in landscape)
            {
                var executor = new Executor(this, landscape, entry, scopeProvider);
                using (var irBuilder = entry.Method.CreateBuilder())
                    ExecuteTransform(irBuilder, executor);
            }
        }

        /// <summary>
        /// Transforms the given method using the provided builder.
        /// </summary>
        /// <typeparam name="TScopeProvider">The provider to resolve methods to scopes.</typeparam>
        /// <param name="builder">The current method builder.</param>
        /// <param name="landscape">The global processing landscape.</param>
        /// <param name="current">The current landscape entry.</param>
        /// <param name="scopeProvider">Resolves methods to scopes.</param>
        protected abstract bool PerformTransformation<TScopeProvider>(
            Method.Builder builder,
            Landscape landscape,
            Landscape.Entry current,
            TScopeProvider scopeProvider)
            where TScopeProvider : IScopeProvider;

        #endregion
    }
}
