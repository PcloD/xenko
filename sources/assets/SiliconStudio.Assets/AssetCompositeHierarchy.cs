﻿// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using SiliconStudio.Assets.Analysis;
using System.Diagnostics.Contracts;
using SiliconStudio.Core;
using SiliconStudio.Core.Annotations;

namespace SiliconStudio.Assets
{
    public abstract class AssetCompositeHierarchy<TAssetPartDesign, TAssetPart> : AssetComposite
        where TAssetPartDesign : class, IAssetPartDesign<TAssetPart>
        where TAssetPart : class, IIdentifiable
    {
        /// <summary>
        /// Gets or sets the container of the hierarchy of asset parts.
        /// </summary>
        [DataMember(100)]
        [NotNull]
        [Display(Browsable = false)]
        public AssetCompositeHierarchyData<TAssetPartDesign, TAssetPart> Hierarchy { get; set; } = new AssetCompositeHierarchyData<TAssetPartDesign, TAssetPart>();

        /// <summary>
        /// Gets the parent of the given part.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The part that is the parent of the given part, or null if the given part is at the root level.</returns>
        /// <remarks>Implementations of this method should not rely on the <see cref="Hierarchy"/> property to determine the parent.</remarks>
        /// <exception cref="ArgumentNullException">The given part is null.</exception>
        [CanBeNull]
        public abstract TAssetPart GetParent([NotNull] TAssetPart part);

        /// <summary>
        /// Gets the index of the given part in the child list of its parent, or in the list of root if this part is a root part.
        /// </summary>
        /// <param name="part">The part for which to retrieve the index.</param>
        /// <returns>The index of the part, or a negative value if the part is an orphan part that is not a member of this asset.</returns>
        /// <exception cref="ArgumentNullException">The given part is null.</exception>
        [Pure]
        public abstract int IndexOf([NotNull] TAssetPart part);

        /// <summary>
        /// Gets the child of the given part that matches the given index.
        /// </summary>
        /// <param name="part">The part for which to retrieve a child.</param>
        /// <param name="index">The index of the child to retrieve.</param>
        /// <returns>The the child of the given part that matches the given index.</returns>
        /// <exception cref="ArgumentNullException">The given part is null.</exception>
        /// <exception cref="IndexOutOfRangeException">The given index is out of range.</exception>
        [Pure]
        public abstract TAssetPart GetChild([NotNull] TAssetPart part, int index);

        /// <summary>
        /// Gets the number of children in the given part.
        /// </summary>
        /// <param name="part">The part for which to retrieve the number of children.</param>
        /// <returns>The number of children in the given part.</returns>
        /// <exception cref="ArgumentNullException">The given part is null.</exception>
        [Pure]
        public abstract int GetChildCount([NotNull] TAssetPart part);

        /// <summary>
        /// Enumerates parts that are children of the given part.
        /// </summary>
        /// <param name="part">The part for which to enumerate child parts.</param>
        /// <param name="isRecursive">If true, child parts will be enumerated recursively.</param>
        /// <returns>A sequence containing the child parts of the given part.</returns>
        /// <remarks>Implementations of this method should not rely on the <see cref="Hierarchy"/> property to enumerate.</remarks>
        [NotNull, Pure]
        public abstract IEnumerable<TAssetPart> EnumerateChildParts([NotNull] TAssetPart part, bool isRecursive);

        /// <summary>
        /// Enumerates design parts that are children of the given design part.
        /// </summary>
        /// <param name="partDesign">The design part for which to enumerate child parts.</param>
        /// <param name="hierarchyData">The hierarchy data object in which the design parts can be retrieved.</param>
        /// <param name="isRecursive">If true, child design parts will be enumerated recursively.</param>
        /// <returns>A sequence containing the child design parts of the given design part.</returns>
        [NotNull, Pure]
        public IEnumerable<TAssetPartDesign> EnumerateChildPartDesigns([NotNull] TAssetPartDesign partDesign, AssetCompositeHierarchyData<TAssetPartDesign, TAssetPart> hierarchyData, bool isRecursive)
        {
            return EnumerateChildParts(partDesign.Part, isRecursive).Select(e => hierarchyData.Parts[e.Id]);
        }
        
        /// <inheritdoc/>
        [NotNull]
        public override IEnumerable<AssetPart> CollectParts()
        {
            return Hierarchy.Parts.Select(x => new AssetPart(x.Part.Id, x.Base, newBase => x.Base = newBase));
        }

        /// <inheritdoc/>
        [CanBeNull]
        public override IIdentifiable FindPart(Guid partId)
        {
            return Hierarchy.Parts.FirstOrDefault(x => x.Part.Id == partId)?.Part;
        }

        /// <inheritdoc/>
        public override bool ContainsPart(Guid id)
        {
            return Hierarchy.Parts.ContainsKey(id);
        }

        /// <inheritdoc/>
        public override Asset CreateDerivedAsset(string baseLocation, out Dictionary<Guid, Guid> idRemapping)
        {
            var newAsset = (AssetCompositeHierarchy<TAssetPartDesign, TAssetPart>)base.CreateDerivedAsset(baseLocation, out idRemapping);

            var instanceId = Guid.NewGuid();
            foreach (var part in Hierarchy.Parts)
            {
                var newPart = newAsset.Hierarchy.Parts[idRemapping[part.Part.Id]];
                newPart.Base = new BasePart(new AssetReference(Id, baseLocation), part.Part.Id, instanceId);
            }

            return newAsset;
        }

        /// <inheritdoc />
        public override void RemapIdentifiableIds(Dictionary<Guid, Guid> remapping)
        {

        }

        /// <summary>
        /// Generates a hierarchy object from the given part that is compatible with the given asset.
        /// </summary>
        /// <typeparam name="TAssetPartDesign">The type of part design for this asset.</typeparam>
        /// <typeparam name="TAssetPart">The type of part for this asset.</typeparam>
        /// <param name="partDesign">The root part design for the hierarchy to generate.</param>
        /// <returns>A hierarchy containing the given part as root and all its children.</returns>
        /// <remarks>
        /// The given part design does not need to be a member of the given asset for this method to work.
        /// </remarks>
        [NotNull]
        public AssetCompositeHierarchyData<TAssetPartDesign, TAssetPart> GenerateHierarchyFromPart([NotNull] TAssetPartDesign partDesign)
        {
            if (partDesign == null) throw new ArgumentNullException(nameof(partDesign));
            var result = new AssetCompositeHierarchyData<TAssetPartDesign, TAssetPart>();
            foreach (var child in EnumerateChildPartDesigns(partDesign, Hierarchy, true))
            {
                result.Parts.Add(child);
            }
            result.Parts.Add(partDesign);
            result.RootParts.Add(partDesign.Part);
            return result;
        }
    }
}
