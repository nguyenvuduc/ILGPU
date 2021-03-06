﻿// -----------------------------------------------------------------------------
//                                    ILGPU
//                     Copyright (c) 2016-2020 Marcel Koester
//                                www.ilgpu.net
//
// File: INodeVisitor.cs
//
// This file is part of ILGPU and is distributed under the University of
// Illinois Open Source License. See LICENSE.txt for details
// -----------------------------------------------------------------------------

using System.Diagnostics;

namespace ILGPU.IR.Values
{
    /// <summary>
    /// A generic interface to visit values in the IR.
    /// </summary>
    public interface IValueVisitor
    {
        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="methodCall">The node.</param>
        void Visit(MethodCall methodCall);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="phiValue">The node.</param>
        void Visit(PhiValue phiValue);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="parameter">The node.</param>
        void Visit(Parameter parameter);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(UnaryArithmeticValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(BinaryArithmeticValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(TernaryArithmeticValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(CompareValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(ConvertValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(PointerCast value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(AddressSpaceCast value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(ViewCast value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(FloatAsIntCast value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(IntAsFloatCast value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="predicate">The node.</param>
        void Visit(Predicate predicate);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="atomic">The node.</param>
        void Visit(GenericAtomic atomic);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="atomicCAS">The node.</param>
        void Visit(AtomicCAS atomicCAS);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="alloca">The node.</param>
        void Visit(Alloca alloca);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="barrier">The node.</param>
        void Visit(MemoryBarrier barrier);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="load">The node.</param>
        void Visit(Load load);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="store">The node.</param>
        void Visit(Store store);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(SubViewValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(LoadElementAddress value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(LoadFieldAddress value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(NewView value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GetViewLength value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(PrimitiveValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(StringValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(NullValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(SizeOfValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GetField value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(SetField value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GetElement value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(SetElement value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GridIndexValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GroupIndexValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GridDimensionValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(GroupDimensionValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(WarpSizeValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="value">The node.</param>
        void Visit(LaneIdxValue value);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="barrier">The node.</param>
        void Visit(PredicateBarrier barrier);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="barrier">The node.</param>
        void Visit(Barrier barrier);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="broadcast">The node.</param>
        void Visit(Broadcast broadcast);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="shuffle">The node.</param>
        void Visit(WarpShuffle shuffle);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="shuffle">The node.</param>
        void Visit(SubWarpShuffle shuffle);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="undefined">The node.</param>
        void Visit(UndefinedValue undefined);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="handle">The node.</param>
        void Visit(HandleValue handle);

        // Debug operations

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="debug">The node.</param>
        void Visit(DebugOperation debug);

        // Terminators

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="returnTerminator">The node.</param>
        void Visit(ReturnTerminator returnTerminator);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="branch">The node.</param>
        void Visit(UnconditionalBranch branch);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="branch">The node.</param>
        void Visit(ConditionalBranch branch);

        /// <summary>
        /// Visits the node.
        /// </summary>
        /// <param name="branch">The node.</param>
        void Visit(SwitchBranch branch);
    }

    /// <summary>
    /// Extensions for a <see cref="IValueVisitor"/>.
    /// </summary>
    public static class ValueVisitorExtensions
    {
        /// <summary>
        /// Visits all child nodes.
        /// </summary>
        /// <typeparam name="T">The visitor type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <param name="node">The current node to visit.</param>
        public static void VisitChildren<T>(this T visitor, Value node)
            where T : IValueVisitor
        {
            Debug.Assert(node != null, "Invalid node");
            foreach (var childNode in node)
                childNode.Accept(visitor);

            foreach (var childNode in node)
                visitor.VisitChildren(childNode);
        }
    }
}
